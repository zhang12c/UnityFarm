using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float speed;
    private float _inputX;
    private float _inputY;
    // 是否移动了
    private bool _isMoving;
    // 动画参数
    private float _mouseX;
    private float _mouseY;
    // 是否在使用工具的状态下
    //private bool _useTool;
    
    // xy 合成的一个向量，人物朝向
    private Vector2 _movementInput;

    private Animator[] _animators;

    /// <summary>
    /// 切换场景的时候，不允许移动
    /// </summary>
    private bool inputDisable;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
    }

    private void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();
        }
        else
        {
            // 不可输入的时候就不要跑步了
            _isMoving = false;
        }
        SwitchAnimator();
    }
    
    // 物理行为都在这个函数中执行
    // 修复一个Bug
    // 切换场景的时候不可以输入的时候，人物一直往下跑
    private void FixedUpdate()
    {
        if (!inputDisable)
        {
            Movement();
        }
    }
    private void OnEnable()
    {
        MyEventHandler.AfterSceneLoadEvent += OnSceneLoad;
        MyEventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnload;
        MyEventHandler.MoveToPos += OnMoveToPos;
        MyEventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }
    private void OnDisable()
    {
        MyEventHandler.AfterSceneLoadEvent -= OnSceneLoad;
        MyEventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnload;
        MyEventHandler.MoveToPos -= OnMoveToPos;
        MyEventHandler.MouseClickedEvent -= OnMouseClickedEvent;

    }
    
    private void OnMoveToPos(Vector3 obj)
    {
        transform.position = obj;
    }
    private void OnBeforeSceneUnload()
    {
        inputDisable = true;
    }
    private void OnSceneLoad()
    {
        inputDisable = false;
    }

    /// <summary>
    /// 获取wasd的输入
    /// </summary>
    private void PlayerInput()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");
        
        // 正常情况这个对象移动方向要慢一些，不能是原来的1
        if (_inputY != 0 && _inputX != 0)
        {
            _inputX *= 0.6f;
            _inputY *= 0.6f;
        }

        // 按下shift 的时候慢走
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _inputX *= 0.5f;
            _inputY *= 0.5f;
        }
        
        _movementInput = new Vector2(_inputX , _inputY);

        _isMoving = _movementInput != Vector2.zero;
    }

    /// <summary>
    /// 刚体移动，人物位置向一个方向移动一定位置，并且考虑帧率
    /// </summary>
    void Movement()
    {
        _rb.MovePosition(_rb.position + _movementInput * speed * Time.deltaTime);
    }

    void SwitchAnimator()
    {
        foreach (var animator in _animators)
        {
            if (_isMoving)
            {
                animator.SetFloat("InputX",_inputX);
                animator.SetFloat("InputY",_inputY);
            }
            animator.SetBool("isMoving",_isMoving);
            
            animator.SetFloat("mouseX",_mouseX);
            animator.SetFloat("mouseY",_mouseY);

        }
    }
    
    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        // 切换玩家的动作
        if (itemDetails.itemType != ItemType.seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            var position = transform.position;
            _mouseX = mouseWorldPos.x - position.x;
            _mouseY = mouseWorldPos.y - (position.y + Settings.PLAYER_SIZE_HALF) ; // 这里有个Bug ，鼠标的y 值永远是大于 人物脚底的 值。导致人物一直会得出 _mouseY > 0 ，面朝上方。所以加了人物的半身长
            
            // 斜方向上的优先选择
            // 如果 比较偏x 那就按x 左右选择
            if (Mathf.Abs(_mouseX) > Mathf.Abs(_mouseY))
            {
                _mouseY = 0;
            }
            else
            {
                _mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            // 播放动画之后
            MyEventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
        
    }
    public IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //_useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var ai in _animators)
        {
            ai.SetTrigger("useTool");
            // 使人物也能面向动作的方向去
            ai.SetFloat("InputX",_mouseX);
            ai.SetFloat("InputY",_mouseY);
        }
        // TODO:优化 这里可以改成帧事件
        yield return new WaitForSeconds(0.5f);
        MyEventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.2f);
        // 动画结束之后
        //_useTool = false;
        inputDisable = false;

    }

}
