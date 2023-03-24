using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 控制场景的切换
/// </summary>
public class TransitionManager : MonoBehaviour
{
    public string startSceneName = String.Empty;

    private void Start()
    {
        StartCoroutine(LoadSceneSetActive(startSceneName));
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
        // 这个项目是全部场景都在的场景中，激活的就是当前的地图
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
        yield return SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
    }

    private void OnEnable()
    {
        MyEvnetHandler.SceneTransitionEvent += SceneTransiTionEven;
    }

    private void OnDisable()
    {
        MyEvnetHandler.SceneTransitionEvent -= SceneTransiTionEven;
    }
    private void SceneTransiTionEven(string sceneName, Vector3 toPos)
    {
        StartCoroutine(SceneTransition(sceneName, toPos));
    }
}
