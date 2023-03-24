using Inventory.Item;
using UnityEngine;
namespace Inventory.Logic
{
    public class ItemManager : MonoBehaviour
    {
        public GameObject itemPerfab;
        
        private Transform itemParent;

        private void OnEnable()
        {
            MyEvnetHandler.CloneSlotInWorld += CloneSlotByItemPerfab;
            MyEvnetHandler.AfterSceneLoadEvent += OnSceneLoad;

        }
        private void OnDisable()
        {
            MyEvnetHandler.CloneSlotInWorld -= CloneSlotByItemPerfab;
            MyEvnetHandler.AfterSceneLoadEvent -= OnSceneLoad;

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
        }
    }
}