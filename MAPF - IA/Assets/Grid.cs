using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform startPosition;
    public LayerMask obstructMask;
    public Vector2 gridWorldSize;

    public float nodeRadius;
    public float distance;

    Node[,] grid;
    public List<Node> finalPath;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // Start is called before the first frame update
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool obstruct = true;

                if (Physics.CheckSphere(worldPoint, nodeRadius, obstructMask)) {
                    obstruct = false;
                }

                grid[x, y] = new Node(obstruct, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 _worldPosition) {
        float xPoint = ((_worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x);
        float yPoint = ((_worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y);

        xPoint = Mathf.Clamp01(xPoint);
        yPoint = Mathf.Clamp01(yPoint);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPoint);

        return grid[x, y];
    }

    public List<Node> GetNeighboringNodes(Node _node) {
        List<Node> neighboringNodes = new List<Node>();
        int xCheck;
        int yCheck;

        xCheck = _node.gridX + 1;
        yCheck = _node.gridY;

        if(xCheck >= 0 && xCheck < gridSizeX) {
            if(yCheck >= 0 && yCheck < gridSizeY) {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        xCheck = _node.gridX -1;
        yCheck = _node.gridY;

        if(xCheck >= 0 && xCheck < gridSizeX) {
            if(yCheck >= 0 && yCheck < gridSizeY) {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        xCheck = _node.gridX;
        yCheck = _node.gridY + 1;

        if(xCheck >= 0 && xCheck < gridSizeX) {
            if(yCheck >= 0 && yCheck < gridSizeY) {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        xCheck = _node.gridX;
        yCheck = _node.gridY - 1;

        if(xCheck >= 0 && xCheck < gridSizeX) {
            if(yCheck >= 0 && yCheck < gridSizeY) {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        return neighboringNodes;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid != null) {
            foreach(Node node in grid) {
                if(node.obstructed) {
                    Gizmos.color = Color.white;
                } else {
                    Gizmos.color = Color.yellow;
                }

                if(finalPath != null) {
                    if(finalPath.Contains(node)) {
                        Gizmos.color = Color.red;
                    }
                }

                Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - distance));
            }
        }
    }
}
