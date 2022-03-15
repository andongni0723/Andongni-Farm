using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;

    // Default Item Icon
    private Sprite defultIcon;
    private VisualElement iconPreview;

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

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Get the Template Data
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        // Get Defult Icon
        defultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Game Assets/2D Assets/Art/Items/Icons/icon_M.png");

        // Set Variable
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");

        // Get Button
        root.Q<Button>("AddButton").clicked += OnAddButtonClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteButtonClicked;

        // Load Data
        LoadDataBase();

        // Clone the ListView
        GenericListView();
    }

    #region Button Event
    private void OnDeleteButtonClicked()
    {
        // Can Open this code if you want
        
        // itemList.Remove(activeItem);
        // itemListView.Rebuild();
        // itemDetailsSection.visible = false;
    }

    private void OnAddButtonClicked()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.itemName = "New Item";
        newItem.itemID = 1000 + itemList.Count + 1;
        itemList.Add(newItem);

        itemListView.Rebuild();
    }
    #endregion

    // Load Data From ItemDataList_SO
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

    // Show the Items on List
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

        itemListView.onSelectionChange += OnListSelectionChange;

        // Right Details Panel can't see on Bagin
        itemDetailsSection.visible = false;
    }

    // On List Selection Change
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }

    // Get Item Details
    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();

        #region Get All Item Datails 
        // Item ID
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemID;
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemID = evt.newValue;
        });

        // Item Name
        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();
        });

        // Item Icon
        iconPreview.style.backgroundImage = activeItem.itemIcon == null ? defultIcon.texture : activeItem.itemIcon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;

            activeItem.itemIcon = newIcon;

            iconPreview.style.backgroundImage = newIcon == null ? defultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });

        // Item World Sprite
        itemDetailsSection.Q<ObjectField>("ItemSprite").value = activeItem.itemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = (Sprite)evt.newValue;
        });

        // Item Type
        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.itemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        // Item Description
        itemDetailsSection.Q<TextField>("Description").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        // Item Use Radius
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        // Item Can Picked up
        itemDetailsSection.Q<Toggle>("CanPickedup").value = activeItem.canPickedup;
        itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedup = evt.newValue;
        });

        // Item Can Dropped
        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        // Item Can Carried
        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        // Item Price
        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        // Item Sell Percentage
        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
        #endregion 
    }
}