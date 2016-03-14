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
        public Vector3 direction;
    }

    public Edge[] edges;

    void OnDrawGizmos()
    {
        for(int i = 0; i < edges.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.transform.position + edges[i].direction * edges[i].distance, 0.5f);
        }
    }
}
