using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class EnemySpawner : MonoBehaviourPun
{
    [Header("Spawn Centers (Empty GameObjects)")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Spawn Area Settings")]
    public float spawnDiameter = 20f;

    [Header("Spawn Timing")]
    public float timeBetweenSpawns = 0.6f;

    [Header("Test Spawn List")]
    public List<EnemySpawnEntry> testSpawnList = new List<EnemySpawnEntry>();

    [System.Serializable]
    public struct EnemySpawnEntry
    {
        public GameObject prefab;
        public int count;
    }

    public void StartSpawningEnemies(List<EnemySpawnEntry> spawnList)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (spawnList == null || spawnList.Count == 0)
            return;

        StartCoroutine(SpawnOverTimeCoroutine(spawnList));
    }

    private IEnumerator SpawnOverTimeCoroutine(List<EnemySpawnEntry> spawnList)
    {
        foreach (EnemySpawnEntry entry in spawnList)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnSingleEnemy(entry.prefab);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    }

    private void SpawnSingleEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Spawn prefab is null!");
            return;
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        Transform chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        Vector3 spawnPos = GetRandomPointInsideCircle(chosenPoint.position, spawnDiameter);

        Quaternion spawnRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        object[] instantiationData = new object[] { photonView.ViewID };

        PhotonNetwork.Instantiate(prefab.name, spawnPos, spawnRot, 0, instantiationData);
    }

    private Vector3 GetRandomPointInsideCircle(Vector3 center, float diameter)
    {
        float radius = diameter * 0.5f;

        Vector2 randomPoint = Random.insideUnitCircle * radius;

        return new Vector3(
            center.x + randomPoint.x,
            center.y,
            center.z + randomPoint.y
        );
    }

    // Draw spawn areas in Scene view
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.red;
        float radius = spawnDiameter * 0.5f;

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, radius);
            }
        }
    }
}