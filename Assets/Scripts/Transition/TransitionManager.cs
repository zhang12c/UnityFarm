using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 控制场景的切换
/// </summary>
public class TransitionManager : MonoBehaviour
{
    [SceneName]
    public string startSceneName = String.Empty;

    /// <summary>
    /// 切换场景时候的渐入渐出UI
    /// </summary>
    private CanvasGroup fadeCanvasGroup;

    private bool isFading = false; 

    private IEnumerator Start()
    {
        fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        yield return LoadSceneSetActive(startSceneName);
        MyEvnetHandler.CallAfterSceneUnloadEvent();
    }

    private void OnEnable()
    {
        MyEvnetHandler.SceneTransitionEvent += SceneTransitionEven;
    }

    private void OnDisable()
    {
        MyEvnetHandler.SceneTransitionEvent -= SceneTransitionEven;
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
        MyEvnetHandler.CallBeforeSceneUnloadEvent();
        // 开始渐入
        yield return DoFade(1);
        
        // 这个项目是全部场景都在的场景中，激活的就是当前的地图
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        yield return StartCoroutine(LoadSceneSetActive(sceneName));
        
        MyEvnetHandler.CallMoveToPos(targetPos);
        // 场景切换成功后的事件
        MyEvnetHandler.CallAfterSceneUnloadEvent();
        // 退出渐入
        yield return DoFade(0);

    }
    private void SceneTransitionEven(string sceneName, Vector3 toPos)
    {
        if (!isFading)
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
        isFading = true;
        fadeCanvasGroup.blocksRaycasts = true;

        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.FADE_DURATION;
        while (!Mathf.Approximately(targetAlpha,fadeCanvasGroup.alpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false;
        isFading = false;
    }
}
