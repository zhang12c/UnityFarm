using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Utility;
namespace TimeLine
{
    public class TimeLineManager : Singleton<TimeLineManager>
    {
        public PlayableDirector startDirector;
        private PlayableDirector _currentDirector;
        private bool _isPause;
        private bool _isDone;
        public bool IsDone
        {
            set
            {
                _isDone = value;
            }
        }

        private void OnEnable()
        {
            // _currentDirector.played += TimeLinePlayed;
            // _currentDirector.stopped += TimeLineStopped;
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;

        }
        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;

        }

        protected override void Awake()
        {
            base.Awake();
            _currentDirector = startDirector;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _isPause && _isDone )
            {
                _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
                _isPause = false;
            }
        }

        /// <summary>
        /// 将timeLime的播放速度调整到0
        /// 达到暂停的效果
        /// </summary>
        /// <param name="playableDirector"></param>
        public void PauseTimeLime(PlayableDirector playableDirector)
        {
            _currentDirector = playableDirector;
            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            _isPause = true;
        }
        // /// <summary>
        // /// 当时间线运行的时候
        // /// 游戏时间要停止起来
        // /// </summary>
        // /// <param name="obj"></param>
        // private void TimeLinePlayed(PlayableDirector obj)
        // {
        //     if (obj != null)
        //     {
        //         MyEventHandler.CallUpdateGameStateEvent(GameState.pause);
        //     }
        // }
        //
        // /// <summary>
        // /// 当TimeLine 停止的时候
        // /// 游戏时间就运行
        // /// </summary>
        // /// <param name="obj"></param>
        // private void TimeLineStopped(PlayableDirector obj)
        // {
        //     if (obj != null)
        //     {
        //         MyEventHandler.CallUpdateGameStateEvent(GameState.gamePlay);
        //         obj.gameObject.SetActive(false);
        //     }
        // }

        private void OnAfterSceneLoadEvent()
        {
            if (SceneManager.GetActiveScene().name == "04_Sea")
            {
                _currentDirector = FindObjectOfType<PlayableDirector>();
                if (_currentDirector != null)
                {
                    _currentDirector.Play();
                }
            }
        }
    }
}