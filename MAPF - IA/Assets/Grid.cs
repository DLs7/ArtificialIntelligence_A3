using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe responsável pela inicialização do nosso grid
public class Grid : MonoBehaviour
{
    public LayerMask obstructMask;  // Camada de objetos obstáculo
    public Vector3 gridWorldSize;   // Tamanho físico do grid
    public Transform agents;        // Transform contendo todos os agentes

    Node[,,] grid;                  // Nosso grid, em um array de 3 dimensões
    public List<Node>[] finalPath;  // Lista de caminhos de cada agente

    float nodeRadius;               // Raio de cada nó
    float nodeDiameter;             // Diâmetro de cada nó
    int gridSizeX, gridSizeY, gridSizeZ;

    // Co-rotina responsável por criar o grid no início da execução do código.
    IEnumerator Start() {
        // Esperamos 1 segundo para garantir que ele carregue os agentes e os destinos apropriadamente.
        yield return new WaitForSeconds(1);

        nodeRadius = 0.5f;
        nodeDiameter = 1;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z);

        // Nossa quantidade de caminhos vai ser igual o número de agentes
        finalPath = new List<Node>[agents.hierarchyCount];

        CreateGrid();
    }

    // Criamos um grid de tamanho x, y, z
    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 bottomLeft = Vector3.zero;
        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                for(int z = 0; z < gridSizeZ; z++) {
                    Vector3 worldPoint = new Vector3(x * nodeDiameter + nodeRadius, z * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius);

                    grid[x, y, z] = new Node(true, worldPoint, x, y, z);
                }
            }
        }
    }

    // Retornamos um nó a partir de uma posição no espaço
    public Node NodeFromWorldPosition(Vector3 _worldPosition) {
        return grid[(int)_worldPosition.x, 
                    (int)_worldPosition.z, 
                    (int)_worldPosition.y];
    }

    // Função responsável por retornar os nós vizinho
    public List<Node> GetNeighboringNodes(Node _node) {
        List<Node> neighboringNodes = new List<Node>();

        // Retorna os vizinhos em x
        for(int x = -1; x <= 1; x = x + 2) {
            if(_node.gridX + x >= 0 && _node.gridX + x < gridSizeX)
                neighboringNodes.Add(grid[_node.gridX + x, _node.gridY, _node.gridZ]);
        }

        // Retorna os vizinhos em y
        for(int y = -1; y <= 1; y = y + 2) {
            if(_node.gridY + y >= 0 && _node.gridY + y < gridSizeY)
                neighboringNodes.Add(grid[_node.gridX, _node.gridY + y, _node.gridZ]);
        }

        // Retorna os vizinhos em z
        for(int z = -1; z <= 1; z = z + 2) {
            if(_node.gridZ + z >= 0 && _node.gridZ + z < gridSizeZ)
                neighboringNodes.Add(grid[_node.gridX, _node.gridY, _node.gridZ + z]);
        }
        return neighboringNodes;
    }

    // Função responsável por desenhar o tamanho do cubo na edição da tela
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(new Vector3(gridWorldSize.x/2, gridWorldSize.z/2, gridWorldSize.y/2), new Vector3(gridWorldSize.x, gridWorldSize.z, gridWorldSize.y));
        if(grid != null) {
            foreach(Node node in grid) {
                if(node.obstructed) {
                    Gizmos.color = Color.clear;
                } else {
                    Gizmos.color = Color.yellow;
                }

                foreach(List<Node> path in finalPath) {
                    if(path != null) {
                        if(path.Contains(node)) {
                            Gizmos.color = Color.red;
                        }
                    }
                }

                Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter));
            }
        }
    }
}
