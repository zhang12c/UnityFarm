using SaveLoad.Data;
using SaveLoad.Logic;
using UnityEngine;
using UnityEngine.UI;
using Utility;
namespace Menu
{
    public class SaveSlotUI : MonoBehaviour
    {
        public Text dataTime, dataScene;
        
        private Button _currentBtn;

        private DataSlot _currentData;

        /// <summary>
        /// 在父物体中的索引
        /// </summary>
        private int Index
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }
 
        private void Awake()
        {
            _currentBtn = GetComponent<Button>();
            _currentBtn.onClick.AddListener(LoadGameData);
        }

        private void Start()
        {
            SetupSlotUI();
        }

        /// <summary>
        /// 对UI上格子的写入
        /// </summary>
        private void SetupSlotUI()
        {
            _currentData = SaveLoadManager.Instance.dataSlots[Index];
            if (_currentData != null)
            {
                dataTime.text = _currentData.DataTime;
                dataScene.text = _currentData.DataScene;
            }
            else
            {
                dataTime.text = "空";
                dataScene.text = "梦还没开始";
            }
        }

        private void LoadGameData()
        {
            if (_currentData != null)
            {
                // 有数据的
                SaveLoadManager.Instance.Load(Index);
            }
            else
            {
                Debug.Log("这玩意儿么有数据，新建立一个！");
                MyEventHandler.OnStartNewGameEvent(Index);
            }
        }
    }
}