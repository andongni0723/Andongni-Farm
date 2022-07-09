using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnFarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; // Grid position
        public int gCost = 0; // Distance of grid to start
        public int hCost = 0; // Distance of grid to end
        public int FCost => gCost + hCost; // The point of the grid

        public bool isObstacle = false;
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            // Choose the smallest one, result -1, 0, 1
            int result = FCost.CompareTo(other.FCost);

            if(result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }

            return result;
        }
    }
}