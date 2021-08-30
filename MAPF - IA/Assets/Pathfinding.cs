using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    public Transform agents;
    public Transform goals;

    private void Awake() {
        grid = GetComponent<Grid>();
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        int index = 0;
        Transform[] ends = goals.GetComponentsInChildren<Transform>();
        foreach(Transform agent in agents) {
            // Debug.Log(agent.position);
            FindPath(agent.position, ends[index].position, index);
            MoveAgents(agent, index);
            index++;
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void MoveAgents(Transform agent, int i) {
        StartCoroutine(MoveAgentsCoroutine(agent, i));
        // Debug.Log("chamou para " + i);
    }

    IEnumerator MoveAgentsCoroutine(Transform agent, int i) {
        foreach(Node path in grid.finalPath[i]) {
            Vector3 position = path.position;
            while(Vector3.Distance(agent.position, position) > .0001) {
                agent.position = Vector3.MoveTowards(agent.position, position, 20f * Time.deltaTime);
                yield return null;
            }
        }
    }

    void FindPath(Vector3 _start, Vector3 _end, int index) {
        // Debug.Log("inicio pathfinding " + index);

        Node startNode = grid.NodeFromWorldPosition(_start);
        Node endNode = grid.NodeFromWorldPosition(_end);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);
        
        while(openList.Count > 0) {
            Node currentNode = openList[0];
            for(int i = 1; i < openList.Count; i++) {
                if(openList[i].FCost <= currentNode.FCost && openList[i].hCost < currentNode.hCost) {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode == endNode) {
                GetFinalPath(startNode, endNode, index);
                openList = new List<Node>();
                closedList = new HashSet<Node>();
            } else {
                foreach(Node neighborNode in grid.GetNeighboringNodes(currentNode)) {
                    if(!neighborNode.obstructed || closedList.Contains(neighborNode)) {
                        continue;
                    }

                    int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighborNode);

                    if(moveCost < neighborNode.gCost || !openList.Contains(neighborNode)) {
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
    
    void GetFinalPath(Node _startNode, Node _endNode, int index) {
        List<Node> finalPath = new List<Node>();
        Node currentNode = _endNode;

        while(currentNode != _startNode) {
            finalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        finalPath.Reverse();
        grid.finalPath[index] = finalPath;

        // Debug.Log("fim pathfinding " + index);
    }

    int GetManhattanDistance(Node _nodeA, Node _nodeB) {
        int iX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int iY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);
        int iZ = Mathf.Abs(_nodeA.gridZ - _nodeB.gridZ);
        
        return iX + iY + iZ;
    }
}
