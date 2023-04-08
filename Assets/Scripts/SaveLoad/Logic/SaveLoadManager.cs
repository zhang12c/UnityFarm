using System.Collections.Generic;
namespace SaveLoad.Logic
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveAble> _saveAbleList = new List<ISaveAble>();


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
        
    }
}