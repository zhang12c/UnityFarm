using System;
namespace Utility
{
    public abstract class Settings
    {
        public const float FADEIN_TIME = 0.35f;
        public const float TARGET_FADE_ALPHA = 0.5f;
        public const string DEFAULT_ICON_PATH = "Assets/GameARTRes/Art/Items/Icons/icon_Game.png";
        public const string ITEM_EDITOR_PATH = "Assets/Editor/UI Builder/ItemEditor.uxml";
        public static string MAKE_ITEM_PATH = "Assets/Editor/UI Builder/ItemRowTemplate.uxml";
    
        //  -----  时间 ----- //
        /// <summary>
        /// 秒间隙
        /// </summary>
        public const float secondThreshould = 0.05f;
        public const int secondHold = 59;
        public const int minutedHold = 59;
        public const int hourHold = 23;
        public const int dayHold = 30;
        /// <summary>
        /// 季
        /// </summary>
        public const int seasonHold = 3;

        /// <summary>
        /// 渐入UI的色变间隔
        /// </summary>
        public const float FADE_DURATION = 1.5f;

        /// <summary>
        /// 人物的一半身高
        /// </summary>
        public const float PLAYER_SIZE_HALF = 0.85f;

        /// <summary>
        /// 杂草最多可以割掉几个
        /// </summary>
        public const int REAP_MAX_COUNT = 2;

        /// <summary>
        /// 每一个网格的默认长度
        /// </summary>
        public const int GRID_CELL_DEFAULT_SIZE = 1;
        /// <summary>
        /// 每一个网格斜方向的默认长度
        /// </summary>
        public const float GRID_CELL_DEFAULT_DIAGONAL_SIZE = 1.4f;
        /// <summary>
        /// 
        /// </summary>
        public const float DEFAULT_PIXE_SIZE = 0.05f; // 图片的像素 20 * 20 占 一个格子 则 1像素占0.05个格子
        
        /// <summary>
        /// npc播放动画间隔
        /// </summary>
        public const float NPC_ANIMATON_PIXE = 10f;

        public const int MAX_GRID_SIZE = 9999;
        
        public const string USER_NAME = "JetKeey";

        /// <summary>
        /// 切换关照所需要的时间差
        /// 如果大于这个值，就慢慢切换
        /// 小于这个值，就马上切换
        /// </summary>
        public const float lightChangeDuration = 25f;
        /// <summary>
        /// 早晨的时间戳
        /// </summary>
        public static readonly TimeSpan morningTime = new TimeSpan(5, 0, 0);
        /// <summary>
        /// 晚上的时间戳
        /// </summary>
        public static readonly TimeSpan nightTime = new TimeSpan(19, 0, 0);

        /// <summary>
        /// 玩家的启使位置
        /// </summary>
        public static UnityEngine.Vector3 PLAYER_START_POS = new UnityEngine.Vector3(2.34f, -2.86f, 0);

        public static int START_MONEY = 100;
    }
}
