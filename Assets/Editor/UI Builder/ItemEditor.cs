using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    
    // Get Visual Element
    private ListView itemListView;

    [MenuItem("Andongni/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Get the Template Data
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        // Set Var
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");

        // Load Data
        LoadDataBase();

        // Clone the ListView
        GenericListView();
    }

    private void LoadDataBase()
    {
        var dataArray = AssetDatabase.FindAssets("t:ItemDataList_SO");

        if (dataArray.Length >= 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
        }

        itemList = dataBase.itemDetails;
        // Save Data
        EditorUtility.SetDirty(dataBase);
        //Debug.Log(itemList[0].itemID);
    }

    private void GenericListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                // Get icon
                if (itemList[i].itemIcon != null)
                    e.Q<VisualElement>("icon").style.backgroundImage = itemList[i].itemIcon.texture;

                // Get name
                e.Q<Label>("name").text = itemList[i] == null ? "NONE item" : itemList[i].itemName;
            }
        };

        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
    }
}