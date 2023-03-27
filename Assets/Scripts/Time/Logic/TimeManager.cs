using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class TimeManager : MonoBehaviour
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

    private void Awake()
    {
        InitGameTime();
    }

    private void Start()
    {
        // 第一次登陆的时候需要初始化一下时间
        MyEventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        MyEventHandler.CallGameMinuteEvent(gameMinute,gameHour);
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
        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }
    }

    /// <summary>
    /// 一个计时的方法
    /// </summary>
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond >= Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;
            if (gameMinute >= Settings.minutedHold)
            {
                gameHour++;
                gameMinute = 0;
                if (gameHour >= Settings.hourHold)
                {
                    gameDay ++;
                    gameHour = 0;
                    if (gameDay >= Settings.dayHold)
                    {
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
                            // 1个季节3个月
                            mouthInSeason = 3;
                            
                            // 当前的季节是哪个 ==== >   0   1   2   3 
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;
                            // +1 后季节的
                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }

                            gameSeason = (Season)seasonNumber;
                            
                        }
                    }
                }
                // 避免冲突调用2次的话，可以启用else
                MyEventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
                //else
                {
                    //MyEvnetHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
                }
            }
            MyEventHandler.CallGameMinuteEvent(gameMinute,gameHour);
            //else
            {
                //MyEvnetHandler.CallGameMinuteEvent(gameMinute,gameHour);
            }
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
}
