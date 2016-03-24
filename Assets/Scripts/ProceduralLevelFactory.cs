using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralLevelFactory : MonoBehaviour
{
    // Editor variables
    public class Connection
    {
        public Vector3 origin;
        public Vector3 worldDirection;
    }
    public int levelSize;
    public Transform[] pieces;

    Queue<Connection> connections = new Queue<Connection>();

    void Start ()
    {
        StartCoroutine(MakeLevel());
	}

    IEnumerator MakeLevel(float time = 0)
    {
        LevelPiece newPiece = GameObject.Instantiate(pieces[Random.Range(0, pieces.Length)]).GetComponent<LevelPiece>();
        newPiece.transform.parent = this.transform;
        Connection newConnection;
        for (int i = 0; i < newPiece.edges.Length; i++)
        {
            newConnection = new Connection();
            newConnection.origin = newPiece.center + newPiece.transform.TransformDirection(newPiece.edges[i].localDirection) * newPiece.edges[i].distance;
            newConnection.worldDirection = newPiece.transform.TransformDirection(newPiece.edges[i].localDirection);
            connections.Enqueue(newConnection);
        }
        yield return new WaitForSeconds(time);

        for (int i = 0; i < levelSize && connections.Count > 0; i++)
        {
            CreateNewLevelPiece(connections.Dequeue());
            yield return new WaitForSeconds(time);
        }
        yield return 0;
    }
    
    void CreateNewLevelPiece(Connection otherConnection)
    {
        // Instantiate piece
        LevelPiece newPiece = GameObject.Instantiate(pieces[Random.Range(0, pieces.Length)]).GetComponent<LevelPiece>();
        newPiece.transform.parent = this.transform;

        // get connecting point of new piece
        int firstConnectionIndex = Random.Range(0, newPiece.edges.Length);

        // Establish new piece position based on the original dequeued connection and the connecting point of the new piece.
        newPiece.transform.localPosition = otherConnection.origin + otherConnection.worldDirection * newPiece.edges[firstConnectionIndex].distance;
        
        // Create rotation quaternion.
        Vector3 fromRot = newPiece.transform.TransformDirection(newPiece.edges[firstConnectionIndex].localDirection);
        fromRot.y = 0;
        fromRot = fromRot.normalized;
        Vector3 toRot = otherConnection.worldDirection;
        toRot.y = 0;
        toRot = toRot.normalized;
        if (fromRot != toRot && fromRot != Vector3.zero - toRot)
        {
            newPiece.transform.rotation *= Quaternion.FromToRotation(fromRot, Vector3.zero - toRot);
        }
        for (int i = 0; i < newPiece.edges.Length; i++)
        {
            if (i == firstConnectionIndex) continue;
            Connection newConnection = new Connection();
            newConnection.origin = newPiece.center + newPiece.transform.TransformDirection(newPiece.edges[i].localDirection) * newPiece.edges[i].distance;
            newConnection.worldDirection = newPiece.transform.TransformDirection(newPiece.edges[i].localDirection);
            bool found = false;
            foreach(Connection c in connections)
            {
                if (c.origin == newConnection.origin)
                {
                    found = true;
                    continue;
                }
            }
            if (found) continue;
            connections.Enqueue(newConnection);
        }
    }

    void OnDrawGizmos()
    {
        foreach(Connection c in connections)
        {
            Gizmos.DrawSphere(c.origin, 1f);
            Gizmos.DrawLine(c.origin, c.origin + c.worldDirection * 2);
        }
    }
}
