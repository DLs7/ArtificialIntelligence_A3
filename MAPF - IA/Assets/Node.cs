using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe responsável por instanciar um nó
public class Node
{
    public int gridX;         // Posição x
    public int gridY;         // Posição y
    public int gridZ;         // Posição z
                              // de um nó no grid

    public bool obstructed;   // Indica se o nó está obstruído
    public Vector3 position;  // A posição física de um nó no espaço

    public Node parent;       // Pai deste nó

    public int gCost;         // gCost do A*
    public int hCost;         // hCost do A*
    public int FCost {        // FCost do A*
        get {
            return gCost + hCost;
        }
    }

    // Iniciamos um novo nó
    public Node(bool _obstructed, Vector3 _position, int _gridX, int _gridY, int _gridZ) {
        this.obstructed = _obstructed;
        this.position = _position;
        this.gridX = _gridX;
        this.gridY = _gridY;
        this.gridZ = _gridZ;
    }
}
