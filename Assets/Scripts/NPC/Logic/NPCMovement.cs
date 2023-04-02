using System;
using System.Collections.Generic;
using AStar;
using NPC.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
namespace NPC
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField]private string _currentScene;
        private string _targetScene;

        /// <summary>
        /// 起始点
        /// </summary>
        private Vector3Int _currentGridPosition;
        /// <summary>
        /// 目标点
        /// </summary>
        private Vector3Int _targetGridPosition;

        public string StartScene
        {
            set
            {
                _currentScene = value;
            }
        }

        [Header("移动属性")]
        public float normalSpeed = 2f;
        private float _minSpeed = 1;
        private float _maxSpeed = 3;
        private Vector2 dir;
        public bool isMoving;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;
        private Animator _animator;

        private Grid _grid;
        /// <summary>
        /// 用于判断是否已经加载过了的
        /// </summary>
        private bool _isLoaded;

        private Stack<MovementStep> _movementSteps;
        
        public ScheduleDataList_SO scheduleData;
        /// <summary>
        /// 这个结构是默认就会排序的
        /// </summary>
        public SortedSet<ScheduleDetails> _scheduleData;
        private ScheduleDetails _currentScheduleDetails;

        private TimeSpan gameTime ;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            gameTime = TimeManager.Instance.GameTime;
        }

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }
        private void OnAfterSceneLoadEvent()
        {
            _grid = FindObjectOfType<Grid>();
            CheckVisiable();
            if (!_isLoaded)
            {
                InitNPC();
                _isLoaded = false;
            }
        }

        private void CheckVisiable()
        {
            if (_currentScene != SceneManager.GetActiveScene().name)
            {
                SetInactiveInScene();
            }
            else
            {
                SetActiveInScene();
            }
        }
        private void SetActiveInScene()
        {
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
        }

        private void SetInactiveInScene()
        {
            _spriteRenderer.enabled = false;
            _boxCollider.enabled = false;
        }

        private void InitNPC()
        {
            _targetScene = _currentScene;
            
            // 保持npc移动的坐标是网格的中心点
            _currentGridPosition = _grid.WorldToCell(transform.position);
            transform.position = new Vector3(_currentGridPosition.x + Settings.GRID_CELL_DEFAULT_SIZE * 0.5f, _currentGridPosition.y + Settings.GRID_CELL_DEFAULT_SIZE * 0.5f, 0);

            _targetGridPosition = _currentGridPosition;
        }

        public void BuildPath(ScheduleDetails scheduleDetails)
        {
            _movementSteps.Clear();
            _currentScheduleDetails = scheduleDetails;

            if (scheduleDetails.targetScene == _currentScene)
            {
                /// 获得寻路的路径
                AStar.AStar.Instance.BuildPath(scheduleDetails.targetScene,(Vector2Int)_currentGridPosition,scheduleDetails.targetGridPosition,_movementSteps);
            }

            if (_movementSteps.Count > 1)
            {
                // 可以移动
                UpdateTimeOnPath();
            }
            
        }

        /// <summary>
        /// 更新走到每一个格子是几分几秒
        /// </summary>
        private void UpdateTimeOnPath()
        {
            // 是上一步的格子
            MovementStep preStep = new MovementStep();
            // 当前的游戏时间
            TimeSpan currentGameTime = gameTime;
            foreach (MovementStep step in _movementSteps)
            {
                
                preStep ??= step;
                // 第一步的时间挫
                step.hour = currentGameTime.Hours;
                step.minute = currentGameTime.Minutes;
                step.second = currentGameTime.Seconds;
                
                // 距离 / 速度 / 每一秒的间隔 = 时间 / 每一秒的间隔(游戏内定义的间隔) = 游戏内的时间 
                // 走每一格需要的游戏时间
                int preStepTime;

                if (MoveInDiagonal(step,preStep))
                {
                    // 斜方向
                    preStepTime = (int)(Settings.GRID_CELL_DEFAULT_DIAGONAL_SIZE / normalSpeed / Settings.secondThreshould);
                }
                else
                {
                    preStepTime = (int)(Settings.GRID_CELL_DEFAULT_SIZE / normalSpeed / Settings.secondThreshould);
                }
                TimeSpan gridMovementStepTime = new TimeSpan(0, 0, preStepTime);

                // 走了一步了，时间是什么时候
                currentGameTime = currentGameTime.Add(gridMovementStepTime);

                // 最后记录下走了哪一步
                preStep = step;
            }
        }

        /// <summary>
        /// 是否是协方向走
        /// </summary>
        /// <param name="currentStep"></param>
        /// <param name="nextStep"></param>
        /// <returns></returns>
        private bool MoveInDiagonal(MovementStep currentStep, MovementStep nextStep)
        {
            return (currentStep.gridCoordinate.x != nextStep.gridCoordinate.x && currentStep.gridCoordinate.y != nextStep.gridCoordinate.y);
        } 
    }
}