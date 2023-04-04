using System;
using UnityEngine;
using Utility;

public class TimeManager : Singleton<TimeManager>
{
    /// <summary>
    /// 基础时间单位
    /// </summary>
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    /// <summary>
    /// 季节
    /// </summary>
    private Season gameSeason = Season.Spring;
    /// <summary>
    /// 季节有3个月
    /// </summary>
    private int mouthInSeason = 3;

    /// <summary>
    /// 时间暂停
    /// </summary>
    private bool gameClockPause;

    private float tikTime;

    public TimeSpan GameTime
    {
        get
        {
            return new TimeSpan(gameHour, gameMinute, gameSecond);
        }
    }

    /// <summary>
    /// 灯光时间差
    /// </summary>
    private float _timeDifference;
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
    private void OnAfterSceneLoadEvent()
    {
        gameClockPause = false;
    }
    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }


    protected override void Awake()
    {
        base.Awake();
        InitGameTime();
    }

    private void Start()
    {
        // 第一次登陆的时候需要初始化一下时间
        MyEventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        MyEventHandler.CallGameMinuteEvent(gameMinute,gameHour,gameDay,gameSeason);
        
        MyEventHandler.CallLightShiftChangeEvent(gameSeason,getCurrentLightShift(),_timeDifference);
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            // 做一个计时器
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshould)
            {
                tikTime = 0;
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
            gameDay++;
            MyEventHandler.CallGameDayEvent(gameDay,gameSeason);
            MyEventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        }
        #endregion
    }

    /// <summary>
    /// 一个计时的方法
    /// </summary>
    private void UpdateGameTime()
    {
        // 秒
        gameSecond++;
        if (gameSecond >= Settings.secondHold)
        {
            // 分
            gameMinute++;
            gameSecond = 0;
            if (gameMinute >= Settings.minutedHold)
            {
                // 时
                gameHour++;
                gameMinute = 0;
                if (gameHour >= Settings.hourHold)
                {
                    // 天
                    gameDay ++;
                    gameHour = 0;
                    if (gameDay >= Settings.dayHold)
                    {
                        // 月
                        gameMonth++;
                        gameDay = 1;
                        if (gameMonth > 12)
                        {
                            gameMonth = 1;
                        }
                        
                        // 过了一个月了
                        mouthInSeason--;
                        if (mouthInSeason <= 0) // 过季节了
                        {
                            // 季
                            // 1个季节3个月
                            mouthInSeason = 3;
                            
                            // 当前的季节是哪个 ==== >   0   1   2   3 
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;
                            // +1 后季节的
                            if (seasonNumber > Settings.seasonHold)
                            {
                                // 年
                                seasonNumber = 0;
                                gameYear++;
                            }

                            gameSeason = (Season)seasonNumber;
                            
                        }
                    }
                    
                    // 每过一天，需要刷新作物的信息
                    MyEventHandler.CallGameDayEvent(gameDay,gameSeason);
                }
                // 避免冲突调用2次的话，可以启用else
                MyEventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
                //else
                {
                    //MyEvnetHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
                }
            }
            MyEventHandler.CallGameMinuteEvent(gameMinute,gameHour,gameDay,gameSeason);
            //else
            //{
                //MyEvnetHandler.CallGameMinuteEvent(gameMinute,gameHour);
            //}
            MyEventHandler.CallLightShiftChangeEvent(gameSeason,getCurrentLightShift(),_timeDifference);
        }
        
        //Debug.Log($"{gameSecond} : {gameMinute}");
    }

    /// <summary>
    /// 初始化时间
    /// </summary>
    private void InitGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2023;
        gameSeason = Season.Spring;
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
}
