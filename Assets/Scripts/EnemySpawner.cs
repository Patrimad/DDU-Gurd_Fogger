using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class EnemySpawner : MonoBehaviourPun
{
    public List<Collider> spawnAreas = new List<Collider>();

    public float timeBetweenSpawns = 0.6f;

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
        {
            return;
        }

        if (spawnList == null || spawnList.Count == 0)
        {
            return;
        }

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
        if (spawnAreas.Count == 0)
        {
            Debug.LogError("No spawn areas assigned!");
            return;
        }

        Collider chosenArea = spawnAreas[Random.Range(0, spawnAreas.Count)];
        Vector3 spawnPos = GetRandomPointInsideCollider(chosenArea);

        Quaternion spawnRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        object[] instantiationData = new object[] { photonView.ViewID };

        PhotonNetwork.Instantiate(prefab.name, spawnPos, spawnRot, 0, instantiationData);
    }

    private Vector3 GetRandomPointInsideCollider(Collider col)
    {
        if (col == null) return transform.position;
        if (col is SphereCollider sphere)
        {
            Vector3 center = col.transform.TransformPoint(sphere.center);
            float radius = sphere.radius;
            return center + Random.insideUnitSphere * radius;
        }

        Bounds b = col.bounds;
        return new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y), Random.Range(b.min.z, b.max.z));
    }
}