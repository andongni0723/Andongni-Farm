using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnFarm.AStar
{
    public class GridNodes : MonoBehaviour
    {
        private int width;
        private int height;
        private Node[,] gridNode;

        /// <summary>
        /// Constructor initializes node-scoped array
        /// </summary>
        /// <param name="width">map width</param>
        /// <param name="height">map height</param>
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;
            
            gridNode = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridNode[x,y] = new Node(new Vector2Int(x,y));
                }
            }
        }

        public Node GetGridNode(int xPos, int yPos)
        {
            if(xPos < width && yPos < height)
            {
                return gridNode[xPos, yPos];
            }

            Debug.LogError("GridNodes: 超出網格範圍");
            return null;
        }
    }

}
