using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class ItemEditor : EditorWindow
{
    // 是一个本地的数据文件
    private ItemDataList_SO _itemDataListSo;
    private List<ItemDetails> _itemList;
    // ListView的子节点数据
    private VisualTreeAsset _makeItemObj;
    // ListView 组件
    private ListView _listView;
    
    
    [MenuItem("Window/UI Toolkit/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        _makeItemObj = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        
        // 获取ListView
        var root2 = root.Q<VisualElement>("ItemList");
        _listView = root2.Q<ListView>("ListView");
        
        LoadDataBase();
        GenerateListView();
    }

    /// <summary>
    /// 加载 ScriptableObject 类型的文件
    /// </summary>
    private void LoadDataBase()
    {
        string[] array = AssetDatabase.FindAssets("ItemDataList_SO");
        if (array.Length > 0)
        {
            // TODO: 目前只有一个文件，写死访问第一个先
            string path = AssetDatabase.GUIDToAssetPath(array[0]);
            _itemDataListSo = AssetDatabase.LoadAssetAtPath<ItemDataList_SO>(path);
            if (!_itemDataListSo)
            {
                Debug.LogError("文件没找到");
            }
            else
            {
                _itemList = _itemDataListSo.itemDetailsList;
                Debug.Log("加载ScriptableObject数据成功");
            }
        }
        // ScriptableObject 修改了之后，unity不会立即保存修改。使用setDirty来保存数据
        // 相当于告诉unity "已修改"
        EditorUtility.SetDirty(_itemDataListSo);
    }

    private void GenerateListView()
    {
        // 子类对象
        Func<VisualElement> makeItem = () => _makeItemObj.CloneTree();
        // 子类初始化
        Action<VisualElement,int> bindItem = InitBindItem;
        
        // listView 初始化
        _listView.itemsSource = _itemList;
        _listView.makeItem = makeItem;
        _listView.bindItem = bindItem;

    }

    private void InitBindItem(VisualElement e, int i)
    {
        VisualElement icon = e.Q<VisualElement>("Icon"); 
        Label itemName = e.Q<Label>("Name");
        if (_itemList[i]?.itemIcon != null && _itemList[i]?.itemName != null)
        {
            icon.style.backgroundImage = _itemList[i].itemIcon.texture;
            itemName.text = _itemList[i].itemName;
        }
        else
        {
            itemName.text = "No Item";
        }
        
    }
}