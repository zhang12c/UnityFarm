using System.Collections.Generic;
using Inventory.Item;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Inventory.Logic
{
    public class ItemManager : MonoBehaviour
    {
        public GameObject itemPerfab;
        
        private Transform itemParent;

        /// <summary>
        /// 记录场景item
        /// 保存下来
        /// 下次切换地图的时候可以直接克隆出来
        /// string 场景名
        /// List 物品列表
        /// </summary>
        private Dictionary<string, List<SceneItemSave>> sceneItemDict = new Dictionary<string, List<SceneItemSave>>();

        private void OnEnable()
        {
            MyEvnetHandler.CloneSlotInWorld += CloneSlotByItemPerfab;
            MyEvnetHandler.AfterSceneLoadEvent += OnSceneLoad;
            MyEvnetHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;

        }
        private void OnDisable()
        {
            MyEvnetHandler.CloneSlotInWorld -= CloneSlotByItemPerfab;
            MyEvnetHandler.AfterSceneLoadEvent -= OnSceneLoad;
            MyEvnetHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;

        }
        private void CloneSlotByItemPerfab(int itemID, Vector3 Pos)
        {
            var item = Instantiate(itemPerfab, Pos, Quaternion.identity,itemParent);
            ItemOnWorld itemOnWorld = item.GetComponent<ItemOnWorld>();
            itemOnWorld.CloneItem(itemID);
        }

        private void OnSceneLoad()
        {
            itemParent = GameObject.FindWithTag("ItemOnWorldParent")?.transform;
            RecreateAllItems();
        }
        
        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }
        /// <summary>
        /// 对场景中的掉落物保存
        /// </summary>
        private void GetAllSceneItems()
        {
            // 当前场景物品列表
            List<SceneItemSave> currentSceneItemSaves = new List<SceneItemSave>();

            foreach (ItemOnWorld item in FindObjectsOfType<ItemOnWorld>())
            {
                SceneItemSave itemSave = new SceneItemSave()
                {
                    itemID = item.itemID,
                    itemSerializableVector3 = new SerializableVector3(item.transform.position)
                };
                currentSceneItemSaves.Add(itemSave);
            }
            // 当前激活着的界面
            var currentScene = SceneManager.GetActiveScene();
            
            if (sceneItemDict.ContainsKey(currentScene.name))
            {
                sceneItemDict[currentScene.name] = currentSceneItemSaves;
            }
            else
            {
                sceneItemDict.Add(currentScene.name,currentSceneItemSaves);
            }
        }
        /// <summary>
        /// 读取场景数据，再一次读取出来
        /// </summary>
        private void RecreateAllItems()
        {
            // 临时保存当下场景的掉落物数据
            List<SceneItemSave> currentSceneItemSaves = new List<SceneItemSave>();
            // 当前激活着的界面
            var currentScene = SceneManager.GetActiveScene();

            if (sceneItemDict.TryGetValue(currentScene.name,out currentSceneItemSaves))
            {
                if (currentSceneItemSaves != null)
                {
                    // 清空
                    foreach (ItemOnWorld item in FindObjectsOfType<ItemOnWorld>())
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in currentSceneItemSaves)
                    {
                        var obj = Instantiate(itemPerfab, item.itemSerializableVector3.ToVector3(),quaternion.identity);
                        ItemOnWorld itemOnWorld = obj.GetComponent<ItemOnWorld>();
                        itemOnWorld.CloneItem(item.itemID);
                    }
                }
                
            }
        }
    }
}