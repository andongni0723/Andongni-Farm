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

        /// <summary>
        /// Build the path, and update the stack
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStep"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStep)
        {
            pathFound = false;

            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                // Find the shortest path
                if (FindShortestPath())
                {
                    // Build the NPC move path
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStep);
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

        /// <summary>
        /// Find the shortest path, and add to "closedNodeList"
        /// </summary>
        /// <returns>find the path done</returns>
        private bool FindShortestPath()
        {
            // Add the start node
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                // Node sort
                openNodeList.Sort();

                // Get the main node
                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);

                closedNodeList.Add(closeNode);

                // Is the end position?
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                // Find the another main node
                // Calculate around 8 Nodes and added to OpenList 
                EvaluateNeighborNodes(closeNode);
            }
            return pathFound;
        }

        /// <summary>
        /// Evaluate the neighbor nodes, and set the cost var
        /// </summary>
        /// <param name="currentNode">currentNode</param>
        private void EvaluateNeighborNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node vaildNeighborNode;

            // Get neighbor nodes
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    vaildNeighborNode = GetVaildNeighborNode(currentNodePos.x + x, currentNodePos.y +y);

                    if(vaildNeighborNode != null)
                    {
                        if(!openNodeList.Contains(vaildNeighborNode))
                        {
                            // Set var
                            vaildNeighborNode.gCost = currentNode.gCost + GetDistance(currentNode, vaildNeighborNode);
                            vaildNeighborNode.hCost = GetDistance(vaildNeighborNode, targetNode);

                            vaildNeighborNode.parentNode = currentNode;

                            openNodeList.Add(vaildNeighborNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get vaild neighbor node
        /// </summary>
        /// <param name="x">position x</param>
        /// <param name="y">position y</param>
        /// <returns>A vaild neighbor node</returns>
        private Node GetVaildNeighborNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;

            Node neighborNode = gridNodes.GetGridNode(x, y);

            if (neighborNode.isObstacle || closedNodeList.Contains(neighborNode))
                return null;
            else
                return neighborNode;
        }

        /// <summary>
        ///  Get the distance of two nodes
        /// </summary>
        /// <param name="nodeA">nodeA</param>
        /// <param name="nodeB">nodeB</param>
        /// <returns>distance of two nodes</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if(xDistance > yDistance)
                return 14 * yDistance + 10 * (xDistance - yDistance);

            return 14 * yDistance + 10 * (yDistance - xDistance);
        }
    
        /// <summary>
        /// Update the position and scene name of all path
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStep"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;

            while(nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoodinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

                // Push to stack
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}
