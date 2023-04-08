using System;
using System.Collections.Generic;
using SaveLoad.Data;
using SaveLoad.Logic;
using UnityEngine;
using Utility;
namespace Time.Logic
{
    public class TimeManager : Singleton<TimeManager>,ISaveAble
    {
        /// <summary>
        /// 基础时间单位
        /// </summary>
        private int _gameSecond, _gameMinute, _gameHour, _gameDay, _gameMonth, _gameYear;
        /// <summary>
        /// 季节
        /// </summary>
        private Season _gameSeason = Season.Spring;
        /// <summary>
        /// 季节有3个月
        /// </summary>
        private int _mouthInSeason = 3;

        /// <summary>
        /// 时间暂停
        /// true 暂停
        /// false 进行
        /// </summary>
        private bool _gameClockPause;

        private float _tikTime;

        public TimeSpan GameTime
        {
            get
            {
                return new TimeSpan(_gameHour, _gameMinute, _gameSecond);
            }
        }

        /// <summary>
        /// 灯光时间差
        /// </summary>
        private float _timeDifference;
        private string _guid;
        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            MyEventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            MyEventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            MyEventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;

        }
        private void OnUpdateGameStateEvent(GameState obj)
        {
            _gameClockPause = obj == GameState.Pause;
        }
        private void OnAfterSceneLoadEvent()
        {
            _gameClockPause = false;
            
            MyEventHandler.CallGameDateEvent(_gameHour,_gameDay,_gameMonth,_gameYear,_gameSeason);
            MyEventHandler.CallGameMinuteEvent(_gameMinute,_gameHour,_gameDay,_gameSeason);
            
            MyEventHandler.CallLightShiftChangeEvent(_gameSeason,getCurrentLightShift(),_timeDifference);
        }
        private void OnBeforeSceneUnloadEvent()
        {
            _gameClockPause = true;
        }


        protected override void Awake()
        {
            base.Awake();
            InitGameTime();
        }
  
        private void Start()
        {
            // 第一次登陆的时候需要初始化一下时间
            MyEventHandler.CallGameDateEvent(_gameHour,_gameDay,_gameMonth,_gameYear,_gameSeason);
            MyEventHandler.CallGameMinuteEvent(_gameMinute,_gameHour,_gameDay,_gameSeason);
        
            MyEventHandler.CallLightShiftChangeEvent(_gameSeason,getCurrentLightShift(),_timeDifference);
        }

        private void Update()
        {
            if (!_gameClockPause)
            {
                // 做一个计时器
                _tikTime += UnityEngine.Time.deltaTime;
                if (_tikTime >= Settings.secondThreshould)
                {
                    _tikTime = 0;
                    UpdateGameTime();
                }
            }
        #region 作弊代码
            if (Input.GetKey(KeyCode.T))
            {
                for (int i = 0; i < 60; i++)
                {
                    UpdateGameTime();
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                _gameDay++;
                MyEventHandler.CallGameDayEvent(_gameDay,_gameSeason);
                MyEventHandler.CallGameDateEvent(_gameHour,_gameDay,_gameMonth,_gameYear,_gameSeason);
            }
        #endregion
        }

        /// <summary>
        /// 一个计时的方法
        /// </summary>
        private void UpdateGameTime()
        {
            // 秒
            _gameSecond++;
            if (_gameSecond >= Settings.secondHold)
            {
                // 分
                _gameMinute++;
                _gameSecond = 0;
                if (_gameMinute >= Settings.minutedHold)
                {
                    // 时
                    _gameHour++;
                    _gameMinute = 0;
                    if (_gameHour >= Settings.hourHold)
                    {
                        // 天
                        _gameDay ++;
                        _gameHour = 0;
                        if (_gameDay >= Settings.dayHold)
                        {
                            // 月
                            _gameMonth++;
                            _gameDay = 1;
                            if (_gameMonth > 12)
                            {
                                _gameMonth = 1;
                            }
                        
                            // 过了一个月了
                            _mouthInSeason--;
                            if (_mouthInSeason <= 0) // 过季节了
                            {
                                // 季
                                // 1个季节3个月
                                _mouthInSeason = 3;
                            
                                // 当前的季节是哪个 ==== >   0   1   2   3 
                                int seasonNumber = (int)_gameSeason;
                                seasonNumber++;
                                // +1 后季节的
                                if (seasonNumber > Settings.seasonHold)
                                {
                                    // 年
                                    seasonNumber = 0;
                                    _gameYear++;
                                }

                                _gameSeason = (Season)seasonNumber;
                            
                            }
                        }
                    
                        // 每过一天，需要刷新作物的信息
                        MyEventHandler.CallGameDayEvent(_gameDay,_gameSeason);
                    }
                    // 避免冲突调用2次的话，可以启用else
                    MyEventHandler.CallGameDateEvent(_gameHour,_gameDay,_gameMonth,_gameYear,_gameSeason);
                    //else
                    {
                        //MyEvnetHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
                    }
                }
                MyEventHandler.CallGameMinuteEvent(_gameMinute,_gameHour,_gameDay,_gameSeason);
                //else
                //{
                //MyEvnetHandler.CallGameMinuteEvent(gameMinute,gameHour);
                //}
                MyEventHandler.CallLightShiftChangeEvent(_gameSeason,getCurrentLightShift(),_timeDifference);
            }
        
            //Debug.Log($"{gameSecond} : {gameMinute}");
        }

        /// <summary>
        /// 初始化时间
        /// </summary>
        private void InitGameTime()
        {
            _gameSecond = 0;
            _gameMinute = 0;
            _gameHour = 7;
            _gameDay = 1;
            _gameMonth = 1;
            _gameYear = 2023;
            _gameSeason = Season.Spring;
        }

        private LightShift getCurrentLightShift()
        {
            if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
            {
                // 白天这个时间段
                // 距离早上5点过去了多少分
                _timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
                return LightShift.Morning;
            }
            else if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime )
            {
                // 黎明
                _timeDifference = Mathf.Abs((float)(GameTime - Settings.morningTime).TotalMinutes);
                return LightShift.Night;
            }
            return LightShift.Morning;
        }
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.timeDict = new Dictionary<string, int>
            {
                {
                    "Second", _gameSecond
                },
                {
                    "Minute", _gameMinute
                },
                {
                    "Hour", _gameHour
                },
                {
                    "Day", _gameDay
                },
                {
                    "Month", _gameMonth
                },
                {
                    "Year", _gameYear
                },
                {
                    "Season", (int)_gameSeason
                }
            };
            return saveData;
        }
        public void RestoreData(GameSaveData saveData)
        {
            _gameSecond = saveData.timeDict["Second"];
            _gameMinute = saveData.timeDict["_gameMinute"];
            _gameHour = saveData.timeDict["Hour"];
            _gameDay = saveData.timeDict["Day"];
            _gameMonth = saveData.timeDict["Month"];
            _gameYear = saveData.timeDict["Year"];
            _gameSeason = (Season)saveData.timeDict["Season"];
        }
        string ISaveAble.GUID
        {
            get
            {
                return GetComponent<DataGUID>()?.guid;
            }
        }
    }
}
