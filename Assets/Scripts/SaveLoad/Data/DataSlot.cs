using System;
using System.Collections.Generic;
using SaveLoad.Logic;
using Time.Logic;
using Transition;
namespace SaveLoad.Data
{
    /// <summary>
    /// 存档 内容
    /// UI 里存档的每一行格子
    /// String GUID
    /// </summary>
    public class DataSlot
    {
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        /// <summary>
        /// 用于UI显示时间
        /// </summary>
        public string DataTime
        {
            get
            {
                var key = ((ISaveAble)TimeManager.Instance).GUID;
                if (dataDict.TryGetValue(key,out GameSaveData saveData))
                {
                    string season = saveData.timeDict["Season"] switch
                    {
                        0 => "春天",
                        1 => "夏天",
                        2 => "秋天",
                        3 => "冬天",
                        _ => "",
                    };
                    return saveData.timeDict["Year"] + "年 " + saveData.timeDict["Month"] + "月 " + saveData.timeDict["Day"] + "日 " + season;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string DataScene
        {
            get
            {
                var key = ((ISaveAble)TransitionManager.Instance).GUID;
                if (dataDict.TryGetValue(key,out GameSaveData saveData))
                {
                    return saveData.dataSceneName switch
                    {
                        "01_Field" => "农场",
                        "02_Home" => "小木屋",
                        "03_Stall" => "市场",
                        "04_Sea" => "海边",
                        _ => ""
                    };
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}