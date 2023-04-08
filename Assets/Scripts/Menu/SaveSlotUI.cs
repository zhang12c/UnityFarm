using UnityEngine;
using UnityEngine.UI;
namespace Menu
{
    public class SaveSlotUI : MonoBehaviour
    {
        public Text dataTime, dataScene;
        
        private Button _currentBtn;

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

        private void LoadGameData()
        {
            Debug.Log(Index);
        }
    }
}