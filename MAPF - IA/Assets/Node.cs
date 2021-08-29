using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int gridX;
    public int gridY;

    public bool obstructed;
    public Vector3 position;

    public Node parent;

    public int gCost;
    public int hCost;
    public int FCost {
        get {
            return gCost + hCost;
        }
    }

    public Node(bool _obstructed, Vector3 _position, int _gridX, int _gridY) {
        this.obstructed = _obstructed;
        this.position = _position;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }
}
