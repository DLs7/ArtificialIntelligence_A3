using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Classe responsável pelo pathfinding A* e pela movimentação dos agentes
public class Pathfinding : MonoBehaviour
{
    Grid grid;                // Referência interna do grid
    public Transform agents;  // Transform contendo todos os agentes
    public Transform goals;   // Transform contendo todos os destinos

    // Pegamos a referência do grid no início da execução desta classe
    private void Awake() {
        grid = GetComponent<Grid>();
    }

    // Co-rotina responsável por calcular todos os caminhos e chamar outra co-rotina para movimentar os agentes
    IEnumerator Start() {
        // Esperamos 1 segundo para garantir que ele carregue os agentes e os destinos apropriadamente.
        yield return new WaitForSeconds(1);

        // Cronômetro para calcular a execução de todos A*
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Para cada agente calculamos seu caminho em seu respectivo destino
        int index = 0;
        Transform[] ends = goals.GetComponentsInChildren<Transform>();
        foreach(Transform agent in agents) {
            // Debug.Log(agent.position);
            FindPath(agent.position, ends[index].position, index);
            MoveAgents(agent, index);
            index++;
        }

        // Paramos o cronômetro e printamos o resultado
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
    }

    // Chama uma co-rotina que move os agentes
    void MoveAgents(Transform agent, int i) {
        StartCoroutine(MoveAgentsCoroutine(agent, i));
        // Debug.Log("chamou para " + i);
    }

    // Co-rotina responsável por mover os agentes
    IEnumerator MoveAgentsCoroutine(Transform agent, int i) {
        foreach(Node path in grid.finalPath[i]) {
            Vector3 position = path.position;

            // Movemos até o meio de um nó, então passamos para o próximo
            while(Vector3.Distance(agent.position, position) > .0001) {
                agent.position = Vector3.MoveTowards(agent.position, position, 20f * Time.deltaTime);
                yield return null;
            }
        }
    }

    // Executa o A*
    void FindPath(Vector3 _start, Vector3 _end, int index) {
        // Recebemos o nó inicial e o nó destino
        Node startNode = grid.NodeFromWorldPosition(_start);
        Node endNode = grid.NodeFromWorldPosition(_end);

        // Criamos uma lista aberta e um hash fechado (hash não guarda a informação do nó, só sua referência, o que poupa a gente um pouco de memória)
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        // Adicionamos o nodo inicial à lista aberta
        openList.Add(startNode);
        
        // Enquanto houver nodos na lista aberta executamos o pathfinding
        while(openList.Count > 0) {
            // Comparamos o primeiro nó da lista
            Node currentNode = openList[0];
            // Com todos os outros nós
            for(int i = 1; i < openList.Count; i++) {
                // E vemos se o FCost e o hCost desses nós são menores que o nó atual
                if(openList[i].FCost <= currentNode.FCost && openList[i].hCost < currentNode.hCost) {
                    // Se sim, realizamos a troca entre eles.
                    currentNode = openList[i];
                }
            }

            // Removemos o nó da lista aberta e colocamos ele na lista fechada
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Se o nó atual for igual ao nó destino
            if(currentNode == endNode) {
                // Pegamos o caminho final e limpamos as listas para parar a execução do algoritmo
                GetFinalPath(startNode, endNode, index);
                openList = new List<Node>();
                closedList = new HashSet<Node>();
            // Se não
            } else {
                // Percorremos todos os nós vizinhos ao nó atual
                foreach(Node neighborNode in grid.GetNeighboringNodes(currentNode)) {
                    // Se esse nó estiver obstruído ou já estiver na lista fechada
                    if(!neighborNode.obstructed || closedList.Contains(neighborNode)) {
                        // Não o verificamos
                        continue;
                    }

                    // Calculamos o custo de movimento do nó atual em relação ao nó analisado
                    int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighborNode);
                    
                    // Se o gCost do nó analisado for menor que o custo de movimento
                    if(moveCost < neighborNode.gCost || !openList.Contains(neighborNode)) {
                        // O nó analisado vira o próximo nodo atual
                        neighborNode.gCost = moveCost;
                        neighborNode.hCost = GetManhattanDistance(neighborNode, endNode);
                        neighborNode.parent = currentNode;

                        if(!openList.Contains(neighborNode)) {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
        }
    }
    
    // Função responsável por retornar uma lista o último nó e todos os seus pais
    void GetFinalPath(Node _startNode, Node _endNode, int index) {
        List<Node> finalPath = new List<Node>();
        Node currentNode = _endNode;

        // Enquanto o nó atual não for igual ao nó inicial
        while(currentNode != _startNode) {
            // Percorre o caminho até encontrá-lo
            finalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        finalPath.Reverse();
        grid.finalPath[index] = finalPath;
    }

    // Calcula a distância de Manhattan
    int GetManhattanDistance(Node _nodeA, Node _nodeB) {
        int iX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int iY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);
        int iZ = Mathf.Abs(_nodeA.gridZ - _nodeB.gridZ);
        
        return iX + iY + iZ;
    }
}
