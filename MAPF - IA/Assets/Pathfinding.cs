using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    public Transform start;
    public Transform end;

    private void Awake() {
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        FindPath(start.position, end.position);
    }

    void FindPath(Vector3 _start, Vector3 _end) {
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
                GetFinalPath(startNode, endNode);
            }

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
    
    void GetFinalPath(Node _startNode, Node _endNode) {
        List<Node> finalPath = new List<Node>();
        Node currentNode = _endNode;

        while(currentNode != _startNode) {
            finalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        finalPath.Reverse();
        grid.finalPath = finalPath;
    }

    int GetManhattanDistance(Node _nodeA, Node _nodeB) {
        int iX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
        int iY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);
        
        return iX + iY;
    }
}
