using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    /// <summary>
    /// 序列化属性的场景索引.
    /// </summary>
    private int sceneIndex = -1;
    /// <summary>
    /// 所有场景名的GUIContent实例.
    /// 注意:场景索引就是EditorBuildSettings.scenes索引,因此这里的索引和场景名一一对应.
    /// </summary>
    private GUIContent[] sceneNames;
    //private readonly string[] scenePathSplit = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GetSceneNameArray(property);
        //base.OnGUI(position, property, label);
        if (EditorBuildSettings.scenes.Length==0)
        {
            return;
        }

        if (sceneIndex==-1)
        {
            GetSceneNameArray(property);
        }

        int oldIndex = sceneIndex;
        sceneIndex=EditorGUI.Popup(position, label,sceneIndex, sceneNames);
        if (oldIndex!=sceneIndex)
        {
            property.stringValue = sceneNames[sceneIndex].text;
        }

    }

    /// <summary>
    /// 填充sceneNames,根据property.stringvalue设置sceneIndex.
    /// </summary>
    /// <param name="property"></param>
    private void GetSceneNameArray(SerializedProperty property)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        sceneNames = new GUIContent[scenes.Length];
        for (int i = 0; i < sceneNames.Length; i++)
        {
            string path = scenes[i].path;
            string sceneName = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = "(DeletedScene)";
            }

            sceneNames[i] = new GUIContent(sceneName);
        }

        if (EditorBuildSettings.scenes.Length==0)
        {
            sceneNames=new GUIContent[]
            {
                new GUIContent("Check your Build Settings!"),
            };
        }

        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text==property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }

            if (!nameFound)//输入的名字与场景名不匹配
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