using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class EnemyStateMachine : MonoBehaviourPun
{
    [Header("Detection & Aggro")]
    public float detectionRange = 12f;
    public float attackRange = 2.2f;
    public float maxAggroRange = 25f;

    [Header("Attack")]
    public float attackCooldown = 1.8f;
    public int attackDamage = 15;
    public string attackTriggerName = "Attack";

    [Header("Movement")]
    public float moveSpeed = 3.2f;
    public float rotationSpeed = 5f;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    private enum EnemyState { Idle, Aware, Attacking, Dying, Dead }
    private EnemyState currentState = EnemyState.Idle;

    public Animator animator;
    public PhotonView photonView;

    private Transform currentTarget;
    private float lastAttackTime;
    private float lastTargetSearchTime;
    private const float search_interval = 0.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // Only Master Client runs AI (or single-player)
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }

        if (currentState == EnemyState.Dying || currentState == EnemyState.Dead)
        {
            return;
        }

        // Lose current target if dead or too far
        if (currentTarget != null)
        {
            if (!IsPlayerAlive(currentTarget) || Vector3.Distance(transform.position, currentTarget.position) > maxAggroRange)
            {
                currentTarget = null;
            }
        }

        // Find new target only when needed (performance friendly)
        if (currentTarget == null && Time.time >= lastTargetSearchTime + search_interval)
        {
            lastTargetSearchTime = Time.time;
            currentTarget = GetNearestAlivePlayer();
        }

        if (currentTarget == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        switch (currentState)
        {
            case EnemyState.Idle:
            case EnemyState.Aware:
                if (distanceToTarget <= detectionRange)
                {
                    if(attackRange <= distanceToTarget)
                    {
                        ChangeState(EnemyState.Attacking);
                    }
                    else
                    {
                        ChangeState(EnemyState.Aware);
                    }
                }
                else
                {
                    currentTarget = null;
                    ChangeState(EnemyState.Idle);
                }
                break;

            case EnemyState.Attacking:
                if (distanceToTarget > attackRange * 1.15f) // small hysteresis
                    ChangeState(EnemyState.Aware);
                else if (Time.time >= lastAttackTime + attackCooldown)
                    Attack();
                break;
        }

        switch (currentState)
        {
            case EnemyState.Aware:
                LookAtTarget();
                MoveTowardsTarget();
                break;

            case EnemyState.Attacking:
                LookAtTarget();
                // You can add a small "approach while attacking" here if you want
                break;
        }
    }

    private Transform GetNearestAlivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            if (p == null || !IsPlayerAlive(p.transform))
                continue;

            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist && dist <= detectionRange)
            {
                minDist = dist;
                nearest = p.transform;
            }
        }
        return nearest;
    }

    // ←←← CHANGE THIS IF YOU HAVE A PLAYER HEALTH SCRIPT ←←←
    private bool IsPlayerAlive(Transform playerTransform)
    {
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
            return false;

        // Example if you have a health script:
        // PlayerHealth health = playerTransform.GetComponent<PlayerHealth>();
        // return health != null && health.currentHealth > 0;

        return true;   // default = assume active players are alive
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Aware:
                animator.SetBool("IsMoving", true);
                animator.SetBool("InAttackRange", false);
                break;

            case EnemyState.Attacking:
                animator.SetBool("IsMoving", false);
                animator.SetBool("InAttackRange", true);
                break;

            case EnemyState.Dying:
                animator.SetTrigger("Die");
                StartCoroutine(DeathSequence());
                break;
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger(attackTriggerName);
        StartCoroutine(DoDamageAfterDelay(0.35f));
    }

    private IEnumerator DoDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState != EnemyState.Attacking || currentTarget == null) yield break;

        if (Vector3.Distance(transform.position, currentTarget.position) <= attackRange + 0.5f)
        {
            // TODO: Make this an RPC if damage can come from clients
            // Example:
            currentTarget.GetComponent<PlayerResourceSystem>()?.Takedamage(attackDamage);
            Debug.Log($"Enemy hit {currentTarget.name} for {attackDamage} damage!");
        }
    }

    public void TakeDamage(float amount)   // call this from bullets, melee, etc.
    {
        if (currentState == EnemyState.Dying || currentState == EnemyState.Dead)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            ChangeState(EnemyState.Dying);
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    private void LookAtTarget()
    {
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        dir.y = 0;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2.8f);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (currentTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
    private void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.InstantiationData == null || photonView.InstantiationData.Length == 0)
            return;

        int parentViewID = (int)photonView.InstantiationData[0];
        PhotonView parentPV = PhotonView.Find(parentViewID);

        if (parentPV != null)
        {
            // worldPositionStays = true → keeps the absolute world position we already set via PhotonNetwork.Instantiate
            transform.SetParent(parentPV.transform, true);
            Debug.Log($"Enemy {gameObject.name} parented to QuestManager");
        }
        else
        {
            Debug.LogWarning("Could not find parent PhotonView for enemy");
        }
    }
}