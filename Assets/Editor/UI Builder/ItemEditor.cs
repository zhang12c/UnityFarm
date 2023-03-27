using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;


public class ItemEditor : EditorWindow
{
    // 是一个本地的数据文件
    private ItemDataList_SO _itemDataListSo;
    private List<ItemDetails> _itemList;
    // ListView的子节点数据
    private VisualTreeAsset _makeItemObj;
    // ListView 组件
    private ListView _listView;
    // 右侧 scrollView
    private ScrollView _scrollView;
    // 左侧选中的item
    private ItemDetails _itemDetailSection;
    // 默认的Icon
    private Sprite _defaultIcon;
    // add Button
    private Button _addButton;
    // delete Button 
    private Button _deleteButton;
    
    
    [MenuItem("ItemDataTool/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // 默认Icon
        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>(Settings.DEFAULT_ICON_PATH);
        
        _makeItemObj = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.MAKE_ITEM_PATH);

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.ITEM_EDITOR_PATH);
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        
        // 获取ListView
        var root2 = root.Q<VisualElement>("ItemList");
        _listView = root2.Q<ListView>("ListView");
        
        // Get ScrollView 
        _scrollView = root.Q<ScrollView>("ItemDetails");
        
        // get Add Button
        _addButton = root.Q<Button>("AddButton");
        // add Event
        _addButton.clicked += OnAddButtonClick;
        // get delete Button
        _deleteButton = root.Q<Button>("DeleteButton");
        _deleteButton.clicked += OnDeleteButtonClick;
        
        
        LoadDataBase();
        GenerateListView();
    }
    private void OnAddButtonClick()
    {
        ItemDetails newItem = new ItemDetails
        {
            itemName = "New Item",
            itemID = 1001 + _itemList.Count
        };
        
        _itemList.Add(newItem);
        _listView.Rebuild();
    }
    
    private void OnDeleteButtonClick()
    {
        _itemList.Remove(_itemDetailSection);
        _listView.Rebuild();
        _scrollView.visible = false;
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

        // 初次默认不可见
        _scrollView.visible = false;
        // 添加一个item Click 的方法回调
        _listView.onSelectionChange += OnListSectionChange;

    }
    private void OnListSectionChange(IEnumerable<object> obj)
    {
        _itemDetailSection = obj.First() as ItemDetails;
        GetItemDetails();
        
        if (!_scrollView.visible)
        {
            _scrollView.visible = true;
        }
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

    private void GetItemDetails()
    {
        // 每次都会刷新数据
        _scrollView.MarkDirtyRepaint();

        IntegerField itemId = _scrollView.Q<IntegerField>("ItemID");
        itemId.value = _itemDetailSection.itemID;
        // 如果数据更新了，就修改数据值
        itemId.RegisterValueChangedCallback(evt => { _itemDetailSection.itemID = evt.newValue; });

        TextField itemName = _scrollView.Q<TextField>("ItemName");
        itemName.value = _itemDetailSection.itemName;
        itemName.RegisterValueChangedCallback(evt => { 
            _itemDetailSection.itemName = evt.newValue; 
            // 刷新一下左侧的内容
            _listView.Rebuild(); 
        });

        EnumField itemType = _scrollView.Q<EnumField>("ItemType");
        itemType.Init(_itemDetailSection.itemType);
        itemType.RegisterValueChangedCallback(evt => { _itemDetailSection.itemType = (ItemType)evt.newValue; });
        
        // 通用里的Icon 主显示内容
        VisualElement itemShowIcon = _scrollView.Q<VisualElement>("Icon");
        if (_itemDetailSection.itemIcon)
        {
            itemShowIcon.style.backgroundImage = _itemDetailSection.itemIcon.texture;
        }
        else
        {
            itemShowIcon.style.backgroundImage = _defaultIcon.texture;
        }
        
        // 物品的图片
        ObjectField itemIcon = _scrollView.Q<ObjectField>("ItemIcon");
        itemIcon.value = _itemDetailSection.itemIcon == null ? _defaultIcon : _itemDetailSection.itemIcon;
        
        itemIcon.RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = (Sprite)evt.newValue;
            _itemDetailSection.itemIcon = newIcon;
            // 更改主显示icon
            itemShowIcon.style.backgroundImage = newIcon == null ? _defaultIcon.texture : newIcon.texture;
            
            _listView.Rebuild();
        });
        
        // 物品的图片
        ObjectField onWorldSprite = _scrollView.Q<ObjectField>("ItemSprite");
        onWorldSprite.value = _itemDetailSection.itemOnWorldSprite == null ? _defaultIcon : _itemDetailSection.itemOnWorldSprite;
        
        onWorldSprite.RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = (Sprite)evt.newValue;
            _itemDetailSection.itemOnWorldSprite = newIcon;
        });
        
        

        TextField description = _scrollView.Q<TextField>("Description");
        description.value = _itemDetailSection.itemDescription;
        description.RegisterValueChangedCallback(evt => _itemDetailSection.itemDescription = evt.newValue);
        
        IntegerField itemUseR = _scrollView.Q<IntegerField>("ItemUseRedius");
        itemUseR.value = _itemDetailSection.itemUseRadius;
        itemUseR.RegisterValueChangedCallback(evt => _itemDetailSection.itemUseRadius = evt.newValue);

        Toggle canPickedup = _scrollView.Q<Toggle>("CanPickedup");
        canPickedup.value = _itemDetailSection.canPickedup;
        canPickedup.RegisterValueChangedCallback(evt => _itemDetailSection.canPickedup = evt.newValue);
        
        Toggle canDropped = _scrollView.Q<Toggle>("CanDropped");
        canDropped.value = _itemDetailSection.canDropped;
        canDropped.RegisterValueChangedCallback(evt => _itemDetailSection.canDropped = evt.newValue);
        
        Toggle canCarried = _scrollView.Q<Toggle>("CanCarried");
        canCarried.value = _itemDetailSection.canCarried;
        canCarried.RegisterValueChangedCallback(evt => _itemDetailSection.canCarried = evt.newValue);
        
        IntegerField price = _scrollView.Q<IntegerField>("Price");
        price.value = _itemDetailSection.itemPrice;
        price.RegisterValueChangedCallback(evt => _itemDetailSection.itemPrice = evt.newValue);

        Slider sellPercentage = _scrollView.Q<Slider>("SellPercentage");
        sellPercentage.value = _itemDetailSection.sellPercentage;
        sellPercentage.RegisterValueChangedCallback(evt => _itemDetailSection.sellPercentage = evt.newValue);
    }
}