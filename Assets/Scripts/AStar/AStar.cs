using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnFarm.Map;

namespace AnFarm.AStar
{
    public class AStar : MonoBehaviour
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList; // 8 points around the currently selected Node
        private HashSet<Node> closedNodeList; // all points by selected

        private bool pathFound;

        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            pathFound = false;

            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                // Find the shortest path
                if (FindShortestPath())
                {
                    // Build the NPC move path
                }
            }
        }


        /// <summary>
        /// Build mesh dimensions from scene name, output dimensions and origin
        /// </summary>
        /// <param name="sceneName">Scene Name</param>
        /// <param name="startPos">Start Position</param>
        /// <param name="endPos">End Position</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                // Build an list of grid mobile node dimensions from tilemap dimensions
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
                return false;

            // The range of gridNodes starts from 0,0, so you need to subtract the origin position to get the real position
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            // Check isn't obstacle
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    //var key = (x + originX) + "x" + (y + originY) + "y" + sceneName;

                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                }
            }

            return true;
        }

        private bool FindShortestPath()
        {
            // Add the start node
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                // Node sort
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);

                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                //Calculate around 8 Nodes and added to OpenList
            }
            return pathFound;
        }
    }
}
