using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace AnFarm.AStar
{
    public class AStarTest : MonoBehaviour
    {
        [Header("Test")]
        private AStar aStar;

        public Vector2Int startPos;
        public Vector2Int endPos;

        public Tilemap displayTilemap;
        public TileBase displayTile;

        public bool displayStartAndEnd;
        public bool displayPath;

        private Stack<MovementStep> npcMovementStepStack;

        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementStepStack = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();
        }

        private void ShowPathOnGridMap()
        {
            if (displayTilemap != null && displayTile != null)
            {
                if (displayStartAndEnd)
                {
                    displayTilemap.SetTile((Vector3Int)startPos, displayTile);
                    displayTilemap.SetTile((Vector3Int)endPos, displayTile);
                }
                else
                {
                    displayTilemap.SetTile((Vector3Int)startPos, null);
                    displayTilemap.SetTile((Vector3Int)endPos, null);
                }

                if (displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;

                    aStar.BuildPath(sceneName, startPos, endPos, npcMovementStepStack);

                    foreach (var step in npcMovementStepStack)
                    {
                        displayTilemap.SetTile((Vector3Int)step.gridCoodinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovementStepStack.Count > 0)
                    {
                        foreach (var step in npcMovementStepStack)
                        {
                            displayTilemap.SetTile((Vector3Int)step.gridCoodinate, null);
                        }
                        npcMovementStepStack.Clear();
                    }
                }
            }
        }
    }
}
