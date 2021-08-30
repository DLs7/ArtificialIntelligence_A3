using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe responsável pela criação dos agentes e seus respectivos destinos.
public class CreateAgents : MonoBehaviour
{
    public Transform obj;     // Recebe o transform do nosso objeto
    public GameObject agent;  // Recebe o agente-modelo
    public GameObject goal;   // Recebe o destino-modelo
    public Mesh mesh;         // Recebe os vértices do nosso objeto
    public Vector3 size;      // Recebe o tamanho do nosso grid

    Vector3 origin;

    List<Vector3> vertices;
    List<Vector3> spawns;

    // Inicia a criação dos agentes e dos destinos
    void Start()
    {
        vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        spawns = new List<Vector3>();

        origin = Vector3.zero;

        // Para cada vértice, criamos um agente e um destino
        foreach(Vector3 vertice in vertices) {
            SpawnAgents();
            CreateGoals(vertice);
        }

        // Destruímos o agente e o destino modelo
        Destroy(agent);
        Destroy(goal);
    }

    // Cria os agentes a partir de um agente modelo em uma posição dentro do nosso grid
    void SpawnAgents() {
        Vector3 position = origin + new Vector3(Random.Range(1, size.x - 1),
                                                Random.Range(1, size.y - 1),
                                                Random.Range(1, size.z - 1));

        // Não cria se, por algum motivo, a posição de um deles for igual a um agente ou destino existente.
        if(!spawns.Contains(position) && !vertices.Contains(position)) {
            spawns.Add(position);

            var newAgent = Instantiate(agent, position, Quaternion.identity);
            newAgent.transform.parent = agent.transform.parent;
        }
    }

    // Cria os destinos a partir de um destino modelo
    void CreateGoals(Vector3 vertice) {
        Matrix4x4 matrix = Matrix4x4.TRS(obj.position, obj.rotation, obj.localScale);

        var newGoal = Instantiate(goal, matrix.MultiplyPoint(vertice), Quaternion.identity);
        newGoal.transform.parent = goal.transform.parent;
    }
}
