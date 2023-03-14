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
    // xy 合成的一个向量，人物朝向
    private Vector2 movementInput;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerInput();
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
        
        movementInput = new Vector2(inputX , inputY);
    }

    /// <summary>
    /// 刚体移动，人物位置向一个方向移动一定位置，并且考虑帧率
    /// </summary>
    void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

}
