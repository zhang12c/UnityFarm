using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;
    // 是否移动了
    private bool isMoving;
    // xy 合成的一个向量，人物朝向
    private Vector2 movementInput;

    private Animator[] _animators;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
    }

    private void Update()
    {
        PlayerInput();
        SwitchAnimator();
    }
    
    // 物理行为都在这个函数中执行
    private void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// 获取wasd的输入
    /// </summary>
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        
        // 正常情况这个对象移动方向要慢一些，不能是原来的1
        if (inputY != 0 && inputX != 0)
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }

        // 按下shift 的时候慢走
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }
        
        movementInput = new Vector2(inputX , inputY);

        isMoving = movementInput != Vector2.zero;
    }

    /// <summary>
    /// 刚体移动，人物位置向一个方向移动一定位置，并且考虑帧率
    /// </summary>
    void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    void SwitchAnimator()
    {
        foreach (var animator in _animators)
        {
            if (isMoving)
            {
                animator.SetFloat("InputX",inputX);
                animator.SetFloat("InputY",inputY);
            }
            
            animator.SetBool("isMoving",isMoving);

        }
    }

}
