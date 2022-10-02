using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[]  m_enemy;
    public GameObject[] EnemySpawnerPoint;
    public GameObject EnemySpawnerparticle;

    void Spawner(Vector3 spawnPosition)
    {
        for(int i = 0; i< EnemySpawnerPoint.Length; i++)
        {
            Vector3 origin = EnemySpawnerPoint[i].transform.position;
            Instantiate(EnemySpawnerparticle, origin, Quaternion.identity);
            Instantiate(m_enemy[i], origin, Quaternion.identity);
            Debug.Log(i);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Spawner(gameObject.gameObject.transform.position);
        }
    }
}
