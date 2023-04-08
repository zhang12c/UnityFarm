using System;
using System.Collections.Generic;
using NPC.Data;
using NPC.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
namespace AStar
{
    public class AStarTest : MonoBehaviour
    {
        [Header("！！！测试用！！！")]
        private AStar aStar;
        public Vector2Int startPos;
        public Vector2Int endPos;
        public Tilemap displayMap;
        public TileBase displayTile;
        public bool displayStartAndFinish;
        public bool displayPath;

        private Stack<MovementStep> npcMovementSteps;

        [Header("测试移动NPC")]
        public NPCMovement npcMovement;
        public bool moveNPC;
        [SceneName]public string targetScene;
        public Vector2Int targetPos;
        public AnimationClip stopClip;
        
        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementSteps = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();

            if (moveNPC)
            {
                moveNPC = false;
                var schedule = new ScheduleDetails(0, 0, 0, 0, Season.Spring, targetScene, targetPos, stopClip, true);
                npcMovement.BuildPath(schedule);
            }
        }

        private void ShowPathOnGridMap()
        {
            if (displayMap && displayTile)
            {
                if (displayStartAndFinish)
                {
                    displayMap.SetTile((Vector3Int)startPos,displayTile);
                    displayMap.SetTile((Vector3Int)endPos,displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos,null);
                    displayMap.SetTile((Vector3Int)endPos,null);
                }

                if (displayPath)
                {
                    aStar.BuildPath(SceneManager.GetActiveScene().name,startPos,endPos,npcMovementSteps);
                    if (npcMovementSteps.Count > 0)
                    {
                        foreach (var step in npcMovementSteps)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate,displayTile);
                        }
                    }
                }
                else
                {
                    if (npcMovementSteps.Count > 0)
                    {
                        foreach (var step in npcMovementSteps)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate,null);
                        }
                    }
                }
            }
        }
    }
}