using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    private int sceneIndex = -1;
    private GUIContent[] sceneNames;
    private readonly string[] scenePathSplit =
    {
        "/", ".unity"
    };
    // 属性的怎么展示，你来确定
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.Log("检查一下File>BuildSetting>Scene没添加");
            return;
        }
        if (sceneIndex == -1)  // 没有初始化
        {
            GetSceneNameArray(property);
        }

        int oldIndex = sceneIndex;
        // 弹出框需要展示什么内容
        // 返回我选中List 中的哪一个
        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if (sceneIndex != oldIndex)
        {
            property.stringValue = sceneNames[sceneIndex].text;
        }
    }
    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;
        sceneNames = new GUIContent[scenes.Length];
        for (int i = 0; i < sceneNames.Length; i++)
        {
            
            string path = scenes[i].path;
            // 返回结果数组中不包含空字符串
            var slitPath = path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);
            string sceneName = "";

            if (slitPath.Length > 0)
            {
                // C# 8.0 新特性 获得list 中的最后一个元素
                sceneName = slitPath[^1];
            }
            else
            {
                sceneName = "(Deleted Scene)";
            }
            sceneNames[i] = new GUIContent(sceneName);
        }
        if (sceneNames.Length == 0)
        {
            sceneNames = new[]
            {
                new GUIContent("检查一下File>BuildSetting>Scene没添加")
            };
        }

        /// 添加一个默认的
        /// 如果默认是空的时候
        if (!String.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }
            if (!nameFound)
            {
                sceneIndex = 0;
            }
        }
        else
        {
            sceneIndex = 0;
        }
        
        property.stringValue = sceneNames[sceneIndex].text;
    }
}
#endif