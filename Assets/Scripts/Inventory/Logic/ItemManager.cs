using System;
using Inventory.Item;
using UnityEngine;
namespace Inventory.Logic
{
    public class ItemManager : MonoBehaviour
    {
        public GameObject itemPerfab;
        
        private Transform itemParent;
        private void Start()
        {
            itemParent = GameObject.FindWithTag("ItemOnWorldParent")?.transform;
        }

        private void OnEnable()
        {
            MyEvnetHandler.CloneSlotInWorld += CloneSlotByItemPerfab;
        }
        private void OnDisable()
        {
            MyEvnetHandler.CloneSlotInWorld -= CloneSlotByItemPerfab;
        }
        private void CloneSlotByItemPerfab(int itemID, Vector3 Pos)
        {
            var item = Instantiate(itemPerfab, Pos, Quaternion.identity,itemParent);
            ItemOnWorld itemOnWorld = item.GetComponent<ItemOnWorld>();
            itemOnWorld.CloneItem(itemID);
        }
    }
}