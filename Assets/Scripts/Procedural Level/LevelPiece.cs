using UnityEngine;
using System.Collections;

public class LevelPiece : MonoBehaviour
{
    public Vector3 center
    {
        get
        {
            return this.transform.position;
        }
    }

    [System.Serializable]
    public class Edge
    {
        public float distance;
        public Vector3 localDirection;
    }

    public Edge[] edges;
    public float spawnRange;
    public Transform[] monsterPrefabs;
    public int maxNumberOfMobs;
    public int minNumberOfMobs;
    public Transform[] decorationPrefabs;
    public int maxNumberOfDecorations;
    public int minNumberOfDecorations;

    void Start()
    {
        for(int i = 0; i < Random.Range(minNumberOfMobs, maxNumberOfMobs); i++)
        {
            Transform newMob = GameObject.Instantiate(monsterPrefabs[Random.Range(0, monsterPrefabs.Length)]);
            newMob.transform.position = this.transform.position + new Vector3(Random.Range(-spawnRange, spawnRange), 1, Random.Range(-spawnRange, spawnRange));
        }

        for (int i = 0; i < Random.Range(minNumberOfDecorations, maxNumberOfDecorations); i++)
        {
            Transform newDeco = GameObject.Instantiate(decorationPrefabs[Random.Range(0, decorationPrefabs.Length)]);
            newDeco.transform.position = this.transform.position + new Vector3(Random.Range(-spawnRange, spawnRange), 1, Random.Range(-spawnRange, spawnRange));
        }
    }

    void OnDrawGizmos()
    {
        for(int i = 0; i < edges.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.transform.position + edges[i].localDirection * edges[i].distance, 0.5f);
        }
    }
}
