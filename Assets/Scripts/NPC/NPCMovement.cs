using System;
using System.Collections.Generic;
using AStar;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
namespace NPC
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class NPCMovement : MonoBehaviour
    {
        private string _currentScene;
        private string _targetScene;

        private Vector3Int _currentGridPosition;
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

        private Stack<MovementStep> _movementSteps;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
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
            CheckVisiable();
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
        
    }
}