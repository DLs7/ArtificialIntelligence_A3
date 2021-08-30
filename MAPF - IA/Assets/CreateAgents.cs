using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAgents : MonoBehaviour
{
    public Transform obj;
    public GameObject agent;
    public GameObject goal;
    public Mesh mesh;

    Vector3 origin;
    Vector3 size;

    List<Vector3> vertices;
    List<Vector3> spawns;

    // Start is called before the first frame update
    void Start()
    {
        vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        spawns = new List<Vector3>();

        origin = Vector3.zero;
        size = new Vector3(100f, 100f, 100f);

        foreach(Vector3 vertice in vertices) {
            SpawnAgents();
            CreateGoals(vertice);
        }

        Destroy(agent);
        Destroy(goal);
    }

    void SpawnAgents() {
        Vector3 position = origin + new Vector3(Random.Range(1, size.x - 1),
                                                Random.Range(1, size.y - 1),
                                                Random.Range(1, size.z - 1));

        if(!spawns.Contains(position) && !vertices.Contains(position)) {
            spawns.Add(position);

            var newAgent = Instantiate(agent, position, Quaternion.identity);
            newAgent.transform.parent = agent.transform.parent;
        }
    }

    void CreateGoals(Vector3 vertice) {
        Matrix4x4 matrix = Matrix4x4.TRS(obj.position, obj.rotation, obj.localScale);

        var newGoal = Instantiate(goal, matrix.MultiplyPoint(vertice), Quaternion.identity);
        newGoal.transform.parent = goal.transform.parent;
    }
}
