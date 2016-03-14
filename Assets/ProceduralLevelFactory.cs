using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralLevelFactory : MonoBehaviour
{
    public class Connection
    {
        public Vector3 origin;
        public Vector3 direction;
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
            newConnection.origin = newPiece.center + newPiece.edges[i].direction * newPiece.edges[i].distance;
            newConnection.direction = newPiece.edges[i].direction;
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
        LevelPiece newPiece = GameObject.Instantiate(pieces[Random.Range(0, pieces.Length)]).GetComponent<LevelPiece>();
        newPiece.transform.parent = this.transform;
        // If the connection was always index = 0, the levels would quickly become predictable.
        int firstConnectionIndex = Random.Range(0, newPiece.edges.Length);
        newPiece.transform.localPosition = otherConnection.origin + otherConnection.direction * newPiece.edges[firstConnectionIndex].distance;
        // TODO Fix new piece's rotation by the difference in the first Connection's direction and the 
        for (int i = 0; i < newPiece.edges.Length; i++)
        {
            if (i == firstConnectionIndex) continue;
            Connection newConnection = new Connection();
            newConnection.origin = newPiece.center + newPiece.edges[i].direction * newPiece.edges[i].distance;
            newConnection.direction = newPiece.edges[i].direction;
            connections.Enqueue(newConnection);
        }
    }
}
