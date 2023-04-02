using System;
using System.Collections;
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
        /// <summary>
        /// 下一个网格的位置
        /// 记录用
        /// </summary>
        private Vector3Int _nextGridPosition;

        public string StartScene
        {
            set
            {
                _currentScene = value;
            }
        }

        [Header("移动属性")]
        public float normalSpeed = 1;
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

        private bool _npcIsMoving;

        /// <summary>
        /// 下一步的临时坐标
        /// </summary>
        private Vector3 _nextstepWorldPosition;

        /// <summary>
        /// 场景每加载就不要动
        /// </summary>
        private bool _sceneIsLoaded;

        private Stack<MovementStep> _movementSteps;
        
        public ScheduleDataList_SO scheduleData;
        /// <summary>
        /// 这个结构是默认就会排序的
        /// </summary>
        public SortedSet<ScheduleDetails> _scheduleData;
        private ScheduleDetails _currentScheduleDetails;

        private TimeSpan gameTime => TimeManager.Instance.GameTime ;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            
            //gameTime = TimeManager.Instance.GameTime;
            _movementSteps = new Stack<MovementStep>();
        }

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        }
        private void OnBeforeSceneUnloadEvent()
        {
            _sceneIsLoaded = false;
        }

        private void FixedUpdate()
        {
            if (_sceneIsLoaded)
            {
                Movement();
            }
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
            _sceneIsLoaded = true;
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
        /// <summary>
        /// 判断NPC可见否可触动否
        /// 可
        /// </summary>
        private void SetActiveInScene()
        {
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
        }

        /// <summary>
        /// 判断NPC可见否可触动否
        /// 不可
        /// </summary>
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

        private void Movement()
        {
            if (!_npcIsMoving)
            {
                if (_movementSteps.Count > 0)
                {
                    // 可以移动啦
                    MovementStep step = _movementSteps.Pop();

                    // 判断一下是否显示
                    _currentScene = step.sceneName;
                    CheckVisiable();

                    // 下一步的坐标
                    _nextGridPosition = (Vector3Int)step.gridCoordinate;
                
                    // 判断时间
                    TimeSpan timeSpan = new TimeSpan(step.hour, step.minute, step.second);

                    MoveToGridPosition(_nextGridPosition,timeSpan);
                }
            }
        }

        /// <summary>
        /// 移动到这个点
        /// </summary>
        /// <param name="gridPos"></param>
        /// <param name="stepTimes"></param>
        private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTimes)
        {
            StartCoroutine(MoveRoutine(gridPos, stepTimes));
        } 

        /// <summary>
        /// 移动一小步移动一小步
        /// </summary>
        /// <param name="gridPos">移动到这个位置</param>
        /// <param name="stepTimes">到这个点，时间是</param>
        /// <returns>下一次fixed update 再移动一点距离咯</returns>
        private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTimes)
        {
            _npcIsMoving = true;
            _nextstepWorldPosition = GetWorldPosition(gridPos);
            //gameTime = TimeManager.Instance.GameTime;
            if (stepTimes > gameTime)
            {
                // 可以移动的时间差
                float timeToMove = (float)(stepTimes.TotalSeconds - gameTime.TotalSeconds);
                // 实际移动距离
                float distance = Vector3.Distance(transform.position, _nextstepWorldPosition);
                // 移动的速度
                float speed = Mathf.Max(_minSpeed, (distance / timeToMove / Settings.secondThreshould));
                if (speed <= _maxSpeed)
                {
                    while (Vector3.Distance(transform.position,_nextstepWorldPosition) > Settings.DEFAULT_PIXE_SIZE)
                    {
                        // 移动的方向
                        dir = (_nextstepWorldPosition - transform.position).normalized;
                        // 移动的距离
                        Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                        _rigidbody2D.MovePosition(_rigidbody2D.position + posOffset);
                        // 下一次fixed update 再移动一点距离咯
                        yield return new WaitForFixedUpdate();
                    }
                }
                else
                    Debug.Log(speed > _maxSpeed);

            }
            // 时间到了，你还没到
            // 马上给我瞬移过去
            // 移动完了之后，那就结束移动吧

            _rigidbody2D.position = _nextstepWorldPosition;
            _currentGridPosition = gridPos;
            _npcIsMoving = false;
            
        }

        /// <summary>
        /// 根据Schedule构建路径
        /// </summary>
        /// <param name="scheduleDetails"></param>
        public void BuildPath(ScheduleDetails scheduleDetails)
        {
            _movementSteps.Clear();
            _currentScheduleDetails = scheduleDetails;

            if (scheduleDetails.targetScene == _currentScene)
            {
                /// 获得寻路的路径
                AStar.AStar.Instance.BuildPath(scheduleDetails.targetScene,(Vector2Int)_currentGridPosition,scheduleDetails.targetGridPosition,_movementSteps);
            }
            // TODO: 跨场景

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
            MovementStep preStep = null;
            // 当前的游戏时间
            TimeSpan currentGameTime = TimeManager.Instance.GameTime;
            foreach (MovementStep step in _movementSteps)
            {
                
                if (preStep == null)
                {
                    preStep = step;
                }
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

        private Vector3 GetWorldPosition(Vector3Int gridPos)
        {
            Vector3 worldPos = _grid.WorldToCell(gridPos);
            float jianGe = Settings.GRID_CELL_DEFAULT_SIZE / 2f;
            return new Vector3(worldPos.x + jianGe, worldPos.y + jianGe, 0);
        }
    }
}