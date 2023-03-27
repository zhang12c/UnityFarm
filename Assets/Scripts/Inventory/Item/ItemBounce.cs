using System;
using UnityEngine;
namespace Inventory.Item
{
    public class ItemBounce : MonoBehaviour
    {
        /// <summary>
        /// 实体物品的图片
        /// </summary>
        private Transform _spriteTrans;
        //// <summary>
        //// 阴影的图片
        //// </summary>
        //private SpriteRenderer _shadowRenderer;
        /// <summary>
        /// 在飞的过程中，就不要有碰撞器了
        /// </summary>
        private BoxCollider2D _boxCollider2D;
        /// <summary>
        /// 飞行的G值
        /// </summary>
        public float gravity = 0.3f;
        /// <summary>
        /// 是否到达地面
        /// </summary>
        private bool _isGround;
        /// <summary>
        /// 飞行的距离
        /// </summary>
        private float _distance;
        /// <summary>
        /// 飞行方向向量
        /// </summary>
        private Vector2 _direction;
        /// <summary>
        /// 飞行的终点
        /// </summary>
        private Vector3 _targetPos;
        private void Awake()
        {
            _spriteTrans = transform.GetChild(0);
            //_shadowRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _boxCollider2D.enabled = false;
        }

        private void Update()
        {
            DoBounce();
        }

        public void InitBounceItem(Vector3 target, Vector2 dir)
        {
            _boxCollider2D.enabled = false;
            _targetPos = target;
            _distance = Vector3.Distance(target,transform.position);
            _direction = dir;

            // 丢出去，图片从头顶飞出
            _spriteTrans.position += Vector3.up * 1.5f;

        }

        private void DoBounce()
        {
            _isGround = _spriteTrans.position.y <= transform.position.y;
            if (Vector3.Distance(transform.position, _targetPos) > 0.1f)
            {
                transform.position += (Vector3)_direction * _distance * -gravity * Time.deltaTime;
            }

            if (!_isGround)
            {
                // 只在做上抛运动 Y方向的移动
                _spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                _spriteTrans.position = transform.position;
                //_shadowRenderer.enabled = false;
                _boxCollider2D.enabled = true;
            }
        }

    }
}
