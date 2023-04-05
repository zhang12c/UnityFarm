using System.Collections;
using System.Collections.Generic;
using Audio.Data;
using Audio.Logic;
using UnityEngine.Pool;
using UnityEngine;
using Utility;
namespace ObjectPool
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        public List<GameObject> poolPrefabs;
        //
        private readonly List<ObjectPool<GameObject>> _poolEffectList = new List<ObjectPool<GameObject>>();

        private Queue<GameObject> soundQueue = new Queue<GameObject>();

        private void OnEnable()
        {
            MyEventHandler.ParticleEffectEvent += OnParticleEffectEvent;
            MyEventHandler.InitSoundEffect += InitSoundEffect;
        }

        private void OnDisable()
        {
            MyEventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
            MyEventHandler.InitSoundEffect += InitSoundEffect;
        }

        private void Start()
        {
            CreatePool();
        }
        //
        private void CreatePool()
        {
            foreach (GameObject item in poolPrefabs)
            {
                // 让粒子都不太分散 用一个新物体装
                Transform parent = new GameObject(item.name).transform;
                parent.SetParent(transform);

                // 创建 获得 刷新 摧毁 
                var newPool = new ObjectPool<GameObject>(() => Instantiate(item, parent),OnTakeFromPool,OnReturnedToPool,OnDestroyPoolObject);
                
                _poolEffectList.Add(newPool);
            }
        }

        /// <summary>
        /// 当实例从池中取出时调用。
        /// </summary>
        private void OnTakeFromPool(GameObject o)
        {
            o.SetActive(true);
        }
        /// <summary>
        /// 当实例返回到池时调用。这可用于清理或禁用实例。
        /// </summary>
        private void OnReturnedToPool(GameObject o)
        {
            o.SetActive(false);
        }
        /// <summary>
        /// 当元素因池达到最大大小而无法返回池时调用。
        /// </summary>
        private void OnDestroyPoolObject(GameObject o)
        {
            Destroy(o);
        }
        /// <summary>
        /// 生成特效
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        private void OnParticleEffectEvent(ParticleEffectType type, Vector3 pos)
        {
            // 潜规则，默认第一个是tree 1 的落叶事件
            ObjectPool<GameObject> objPool = type switch
            {
                ParticleEffectType.None => null,
                ParticleEffectType.LeavesFalling01 => _poolEffectList[0],
                ParticleEffectType.LeavesFalling02 => _poolEffectList[1],
                ParticleEffectType.Rock => _poolEffectList[2],
                ParticleEffectType.ReapableScenery => _poolEffectList[3],
                _ => null,
            };

            if (objPool != null)
            {
                GameObject obj = objPool.Get();
                obj.transform.position = pos;
                StartCoroutine(ReleaseRoutine(objPool, obj));
            }
            
        }

        /// <summary>
        /// 计时器 2s 后将GameObject 丢回对象池
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
        {
            yield return new WaitForSeconds(2f);
            pool.Release(obj);
        }

        /// <summary>
        /// 播放音效的主函数
        /// </summary>
        /// <param name="soundDetails"></param>
        // private void InitSoundEffect(SoundDetails soundDetails)
        // {
        //     ObjectPool<GameObject> pool = _poolEffectList[4];
        //     var obj = pool.Get();
        //
        //     obj.GetComponent<Sound>().SetSound(soundDetails);
        //     StartCoroutine(DisableSound(pool, obj, soundDetails));
        // }
        //
        // private IEnumerator DisableSound(ObjectPool<GameObject> pool, GameObject obj, SoundDetails soundDetails)
        // {
        //     yield return new WaitForSeconds(soundDetails.soundClip.length);
        //     pool.Release(obj);
        // }

        private void CreateSoundPool()
        {
            var parent = new GameObject(poolPrefabs[4].name).transform;
            parent.SetParent(transform);

            for (int i = 0; i < 10; i++)
            {
                GameObject obj = Instantiate(poolPrefabs[4], parent);
                obj.SetActive(false);
                soundQueue.Enqueue(obj);
            }
        }

        private GameObject GetPoolObject()
        {
            if (soundQueue.Count < 2)
            {
                CreateSoundPool();
            }
            return soundQueue.Dequeue();
        }

        private void InitSoundEffect(SoundDetails soundDetails)
        {
            var obj = GetPoolObject();
            obj.GetComponent<Sound>().SetSound(soundDetails);
            obj.SetActive(true);

            StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
        }

        private IEnumerator DisableSound(GameObject obj, float duration)
        {
            yield return new WaitForSeconds(duration);
            obj.SetActive(false);
            soundQueue.Enqueue(obj);
        }
    }
}