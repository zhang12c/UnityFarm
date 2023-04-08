using System;
using System.Collections.Generic;
using System.IO;
using SaveLoad.Data;
using Newtonsoft.Json;
using UnityEngine;
using Utility;
namespace SaveLoad.Logic
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveAble> _saveAbleList = new List<ISaveAble>();

        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);

        public string jsonFolder;

        private int _currentDataIndex;
        protected override void Awake()
        {
            base.Awake();
            jsonFolder = Application.persistentDataPath + "/savedata/";
            ReadSaveData();
        }

        private void OnEnable()
        {
            MyEventHandler.StartNewGameEvent += OnStartNewGameEvent;
            MyEventHandler.EndGameEvent += OnEndGameEvent;


        }

        private void OnDisable()
        {
            MyEventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            MyEventHandler.EndGameEvent -= OnEndGameEvent;


        }
        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("保存了" + _currentDataIndex + "进度");
                Save(_currentDataIndex);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                Load(_currentDataIndex);
            }
        }

        /// <summary>
        /// 注册SaveAble List .
        /// </summary>
        /// <param name="saveAble"></param>
        public void RegisterSaveAble(ISaveAble saveAble)
        {
            if (!_saveAbleList.Contains(saveAble))
            {
                _saveAbleList.Add(saveAble);
            }
        }

        private void Save(int index)
        {
            DataSlot data = new DataSlot();
            foreach (ISaveAble saveAble in _saveAbleList)
            {
                data.dataDict.Add(saveAble.GUID,saveAble.GenerateSaveData());
            }
            dataSlots[index] = data;

            string resultPath = jsonFolder + "data" + index + ".json";
            string jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);

            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }
            File.WriteAllText(resultPath,jsonData);
        }

        public void Load(int index)
        {
            _currentDataIndex = index;
            
            string resultPath = jsonFolder + "data" + index + ".json";
            string stringData = File.ReadAllText(resultPath);
            DataSlot jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            foreach (ISaveAble saveAble in _saveAbleList)
            {
                saveAble.RestoreData(jsonData.dataDict[saveAble.GUID]);
            }
        }

        /// <summary>
        /// 游戏开始的时候，去读取一下游戏已有的数据进度
        /// </summary>
        private void ReadSaveData()
        {
            if (Directory.Exists(jsonFolder))
            {
                for (int i = 0; i < dataSlots.Count; i++)
                {
                    string resultPath = jsonFolder + "data" + i + ".json";
                    if (File.Exists(resultPath))
                    {
                        string stringData = File.ReadAllText(resultPath);
                        DataSlot jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        dataSlots[i] = jsonData;
                    }
                }
            }
        }
        
        private void OnStartNewGameEvent(int obj)
        {
            _currentDataIndex = obj;
        }
        
        private void OnEndGameEvent()
        {
            Save(_currentDataIndex);
        }
        
    }
}