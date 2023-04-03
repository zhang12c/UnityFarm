using System;
using System.Collections;
using System.Collections.Generic;
using AStar;
using NPC.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utility;
namespace NPC
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class NPCMovement : MonoBehaviour
    {
        /// <summary>
        /// 当前的场景名称
        /// </summary>
        [SerializeField]private string _currentScene;
        /// <summary>
        /// 目标场景名称
        /// </summary>
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

        // 身上的一些组件
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;
        private Animator _animator;
        // 场景中的Grid
        private Grid _grid;
        /// <summary>
        /// 用于判断场景是否已经加载过了的
        /// </summary>
        private bool _isLoaded;
        /// <summary>
        /// npc 是否在移动过程中
        /// 可能是一个冗余参数
        /// </summary>
        private bool _npcIsMoving;
        /// <summary>
        /// 下一步的临时坐标
        /// </summary>
        private Vector3 _nextstepWorldPosition;
        /// <summary>
        /// 场景是否加载完成了
        /// 场景没加载就不要动
        /// </summary>
        private bool _sceneIsLoaded;
        /// <summary>
        /// npc到达目的地的步数队列
        /// </summary>
        private Stack<MovementStep> _movementSteps;
        /// <summary>
        /// npc的行为列表
        /// </summary>
        [FormerlySerializedAs("scheduleData")]
        public ScheduleDataList_SO scheduleDataSO;
        /// <summary>
        /// 这个结构是默认就会排序的
        /// </summary>
        public SortedSet<ScheduleDetails> _scheduleData;
        private ScheduleDetails _currentScheduleDetails;
        /// <summary>
        /// 当前时间
        /// </summary>
        private TimeSpan gameTime
        {
            get
            {
                return TimeManager.Instance.GameTime;
            }
        }

        #region 动画的参数
        /// <summary>
        /// 动画可以循环播放的计时间隔
        /// </summary>
        private float _animationBreakTime;
        /// <summary>
        /// 可以播放动画
        /// </summary>
        private bool _canPlayStopAnimation;
        /// <summary>
        /// 停止的时候动作
        /// </summary>
        private AnimationClip _stopAnimationClip;
        /// <summary>
        /// Animation
        /// </summary>
        public AnimationClip _blankAnimationClip;
        /// <summary>
        /// 不通的事件创建不同的animator
        /// </summary>
        private AnimatorOverrideController _animatorOverrideController;
        #endregion

        /// <summary>
        /// 是否是可以互动的
        /// </summary>
        internal bool interactable;


        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            MyEventHandler.GameMinuteEvent += OnGameMinuteEvent;
        }
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            
            _movementSteps = new Stack<MovementStep>();
            _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            if (CheckHasStopClip())
            {
                _animator.runtimeAnimatorController = _animatorOverrideController;
            }
            
            _scheduleData = new SortedSet<ScheduleDetails>();
            foreach (ScheduleDetails scheduleData in scheduleDataSO.scheduleDetailsList)
            {
                _scheduleData.Add(scheduleData);
            }
        }
        
        private void Update()
        {
            if (_sceneIsLoaded)
            {
                SwitchAnimation();
            }

            _animationBreakTime -= Time.deltaTime;
            if (_animationBreakTime <= 0)
            {
                _canPlayStopAnimation = true;
            }
            else
            {
                _canPlayStopAnimation = false;
            }
        }
        private void FixedUpdate()
        {
            if (_sceneIsLoaded)
            {
                Movement();
            }
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            MyEventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        }
        private void OnGameMinuteEvent(int m, int h,int day, Season season)
        {
            int time = h * 100 + m;
            ScheduleDetails matchScheduleDetails = null;
            foreach (ScheduleDetails schedule in _scheduleData)
            {
                if (schedule.realTime == time)
                {
                    if (schedule.day != day && schedule.day != 0)
                    {
                        continue;
                    }
                    if (schedule.season != season)
                    {
                        continue;
                    }
                    matchScheduleDetails = schedule;
                }else if (schedule.realTime > time)
                {
                    // 有序的一个列表，因为他默认第一个为最先触发的行为
                    break;
                }
            }

            if (matchScheduleDetails != null)
            {
                // 有符合条件的行为逻辑
                BuildPath(matchScheduleDetails);
            }
        }
        private void OnBeforeSceneUnloadEvent()
        {
            _sceneIsLoaded = false;
        }
        
        private void OnAfterSceneLoadEvent()
        {
            _grid = FindObjectOfType<Grid>();
            CheckVisiable();
            if (!_isLoaded)
            {
                InitNPC();
                _isLoaded = true;
            }
            _sceneIsLoaded = true;
        }
        /// <summary>
        /// 判断npc显影逻辑
        /// </summary>
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
            // 影子
            transform.GetChild(0).gameObject.SetActive(true);
        }

        /// <summary>
        /// 判断NPC可见否可触动否
        /// 不可
        /// </summary>
        private void SetInactiveInScene()
        {
            _spriteRenderer.enabled = false;
            _boxCollider.enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }

        /// <summary>
        /// 初始化npc
        /// </summary>
        private void InitNPC()
        {
            //_targetScene = _currentScene;
            
            // 保持npc移动的坐标是网格的中心点
            _currentGridPosition = _grid.WorldToCell(transform.position);
            transform.position = new Vector3(_currentGridPosition.x + Settings.GRID_CELL_DEFAULT_SIZE * 0.5f, _currentGridPosition.y + Settings.GRID_CELL_DEFAULT_SIZE * 0.5f, 0);
            _targetGridPosition = _currentGridPosition;
        }

        /// <summary>
        /// 最主要的移动函数
        /// </summary>
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
                else if (!isMoving && _canPlayStopAnimation)
                {
                    // 已经停止了，到达了目标地点
                    StartCoroutine(FaceToCamera());
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
            _targetGridPosition = (Vector3Int)scheduleDetails.targetGridPosition;
            _stopAnimationClip = scheduleDetails.clipAtStop;
            _targetScene = scheduleDetails.targetScene;
            interactable = scheduleDetails.interactable;
            if (scheduleDetails.targetScene == _currentScene)
            {
                /// 获得寻路的路径
                AStar.AStar.Instance.BuildPath(scheduleDetails.targetScene,(Vector2Int)_currentGridPosition,scheduleDetails.targetGridPosition,_movementSteps);
            }
            else if (scheduleDetails.targetScene != _currentScene)
            {
                // 跨场景
                SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(_currentScene, _targetScene);
                if (sceneRoute != null)
                {
                    foreach (ScenePath scenePath in sceneRoute.scenePathList)
                    {
                        Vector2Int fromPos, toPos;
                        if (scenePath.fromGridCell.x >= Settings.MAX_GRID_SIZE)
                        {
                            // 9999 不管从哪里来
                            fromPos = (Vector2Int)_currentGridPosition;
                        }
                        else
                        {
                            // 到这个地方去
                            fromPos = scenePath.fromGridCell;
                        }
                        if (scenePath.toGridCell.x >= Settings.MAX_GRID_SIZE)
                        {
                            toPos = (Vector2Int)_targetGridPosition;
                        }
                        else
                        {
                            toPos = scenePath.toGridCell;
                        }
                        
                        AStar.AStar.Instance.BuildPath(scenePath.sceneName,fromPos,toPos,_movementSteps);
                    }
                }
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

        /// <summary>
        /// 偏移0.5
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        private Vector3 GetWorldPosition(Vector3Int gridPos)
        {
            Vector3 worldPos = _grid.WorldToCell(gridPos);
            float jianGe = Settings.GRID_CELL_DEFAULT_SIZE / 2f;
            return new Vector3(worldPos.x + jianGe, worldPos.y + jianGe, 0);
        }

        /// <summary>
        /// 动画状态机的切换
        /// </summary>
        private void SwitchAnimation()
        {
            isMoving = transform.position != GetWorldPosition(_targetGridPosition);
            
            _animator.SetBool("isMoving",isMoving);
            if (isMoving)
            {
                _animator.SetBool("Exit",true);
                _animator.SetFloat("DirX",dir.x);
                _animator.SetFloat("DirY",dir.y);
            }
            else
            {
                _animator.SetBool("Exit",false);
            }
        }
        /// <summary>
        /// 强制面向下，并且循环播放动作
        /// 用一个计时的方法
        /// </summary>
        /// <returns></returns>
        private IEnumerator FaceToCamera()
        {
            // 强制面向下
            _animator.SetFloat("DirX",0);
            _animator.SetFloat("DirY",-1);

            _animationBreakTime = Settings.NPC_ANIMATON_PIXE;

            if (_stopAnimationClip != null)
            {
                _animatorOverrideController[_blankAnimationClip] = _stopAnimationClip;
                _animator.SetBool("EventAnimation",true);
                yield return null;
                _animator.SetBool("EventAnimation",false);
            }
            else
            {
                _animatorOverrideController[_stopAnimationClip] = _blankAnimationClip;
                _animator.SetBool("EventAnimation",false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckHasStopClip()
        {
            foreach (ScheduleDetails schedule in scheduleDataSO.scheduleDetailsList)
            {
                if (schedule.clipAtStop != null)
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}