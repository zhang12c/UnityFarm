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

    }
}
