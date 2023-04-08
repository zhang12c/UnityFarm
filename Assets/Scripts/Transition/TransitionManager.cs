using System;
using System.Collections;
using SaveLoad.Data;
using SaveLoad.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
namespace Transition
{
    /// <summary>
    /// 控制场景的切换
    /// </summary>
    public class TransitionManager : Singleton<TransitionManager>,ISaveAble
    {
        [SceneName]
        public string startSceneName = String.Empty;

        /// <summary>
        /// 切换场景时候的渐入渐出UI
        /// </summary>
        private CanvasGroup _fadeCanvasGroup;

        private bool _isFading = false;

        protected override void Awake()
        {
            // 打包的之后只会显示第一个场景，在这里先load出来UI
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }
        private void Start()
        {
            ISaveAble saveAble = this;
            saveAble.RegisterSaveAble();
            
            _fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        }

        private void OnEnable()
        {
            MyEventHandler.SceneTransitionEvent += SceneTransitionEven;
            MyEventHandler.StartNewGameEvent += OnStartNewGameEvent;

        }

        private void OnDisable()
        {
            MyEventHandler.SceneTransitionEvent -= SceneTransitionEven;
            MyEventHandler.StartNewGameEvent -= OnStartNewGameEvent;

        }
        private void OnStartNewGameEvent(int obj)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
            
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            /// load sceneName 的场景进入当前的场景
            yield return SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
            // 只可以用这个方式获得场景中有多少个场景
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                {
                    SceneManager.SetActiveScene(scene);
                    break;
                }
            }
        
        }
    
        /// <summary>
        /// 当协程有多个yield的时候，有先后的执行顺序
        /// 先是第一个条件满足了
        /// 再执行第二个条件
        /// 全部都执行完后
        /// 这个协程自动结束
        /// </summary>
        /// <param name="sceneName">新激活的场景</param>
        /// <param name="targetPos">切换到下个地图的具体位置</param>
        /// <returns></returns>
        private IEnumerator SceneTransition(string sceneName,Vector3 targetPos)
        {
            MyEventHandler.CallBeforeSceneUnloadEvent();
            // 开始渐入
            yield return DoFade(1);
        
            // 这个项目是全部场景都在的场景中，激活的就是当前的地图
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return StartCoroutine(LoadSceneSetActive(sceneName));
        
            MyEventHandler.CallMoveToPos(targetPos);
            // 场景切换成功后的事件
            MyEventHandler.CallAfterSceneUnloadEvent();
            // 退出渐入
            yield return DoFade(0);

        }
        private void SceneTransitionEven(string sceneName, Vector3 toPos)
        {
            if (!_isFading)
            {
                StartCoroutine(SceneTransition(sceneName, toPos));
            }
        }

        /// <summary>
        /// 做一个渐入渐出的协程
        /// </summary>
        /// <param name="targetAlpha"> 1 切换 0 未 </param>
        /// <returns></returns>
        private IEnumerator DoFade(float targetAlpha)
        {
            _isFading = true;
            _fadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(_fadeCanvasGroup.alpha - targetAlpha) / Settings.FADE_DURATION;
            while (!Mathf.Approximately(targetAlpha,_fadeCanvasGroup.alpha))
            {
                _fadeCanvasGroup.alpha = Mathf.MoveTowards(_fadeCanvasGroup.alpha, targetAlpha, speed * UnityEngine.Time.deltaTime);
                yield return null;
            }

            _fadeCanvasGroup.blocksRaycasts = false;
            _isFading = false;
        }
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName = SceneManager.GetActiveScene().name;
            return saveData;
        }
        public void RestoreData(GameSaveData saveData)
        {
            // 加载读取一个场景
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
        public string GUID
        {
            get
            {
                return GetComponent<DataGUID>().guid;
            }
        }

        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return DoFade(1f);
            if (SceneManager.GetActiveScene().name != "PersistentScene")
            {
                // 这个属于读存档时候，没有已经是具体场景名的时候
                MyEventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            yield return LoadSceneSetActive(sceneName);
            MyEventHandler.CallAfterSceneUnloadEvent();
            yield return DoFade(0);
        }
    }
}
