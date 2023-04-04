using System;
using UnityEngine;
namespace NPC.Data
{
    /// <summary>
    /// 同一个NPC 有不同的行为字典
    /// 什么时间是什么季节，做什么
    /// </summary>
    [Serializable]
    public class ScheduleDetails : IComparable <ScheduleDetails>
    {
        /// <summary>
        /// 什么时候做什么事
        /// </summary>
        public int hour, minute, day;
        /// <summary>
        /// 优先级越小越先执行
        /// </summary>
        public int priority;
        /// <summary>
        /// 不同季节
        /// </summary>
        public Season season;
        /// <summary>
        /// 去哪个场景
        /// </summary>
        [SceneName]public string targetScene;
        public Vector2Int targetGridPosition;
        /// <summary>
        /// 播放什么动画
        /// </summary>
        public AnimationClip clipAtStop;
        /// <summary>
        /// 是否是可以互动的
        /// 移动中可以交互否
        /// </summary>
        public bool interactable;

        public int realTime
        {
            get
            {
                return hour * 100 + minute;
            }
        }

        public ScheduleDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable)
        {
            this.hour = hour;
            this.minute = minute;
            this.day = day;
            this.priority = priority;
            this.season = season;
            this.targetScene = targetScene;
            this.targetGridPosition = targetGridPosition;
            this.clipAtStop = clipAtStop;
            this.interactable = interactable;
        }
        public int CompareTo(ScheduleDetails other)
        {
            if (realTime == other.realTime)
            {
                if (priority > other.priority)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }else if (realTime > other.realTime)
            {
                return 1;
            }else if (realTime < other.realTime)
            {
                return -1;
            }
            return 0;
        }
    }
}