using SaveLoad.Data;
namespace SaveLoad.Logic
{
    public interface ISaveAble
    {
        /// <summary>
        /// 建立存档的方法
        /// </summary>
        /// <returns></returns>
        GameSaveData GenerateSaveData();
        /// <summary>
        /// 使用存档的方法
        /// </summary>
        /// <param name="saveData"></param>
        void RestoreData(GameSaveData saveData);

        /// <summary>
        /// 实例出来，把自己注册到SaveLoadManager的List中
        /// </summary>
        void RegisterSaveAble()
        {
            SaveLoadManager.Instance.RegisterSaveAble(this);
        }

        protected string GUID { get; }
    }
}