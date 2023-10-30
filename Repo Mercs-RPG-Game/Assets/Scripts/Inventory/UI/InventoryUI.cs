using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    int selectedItem = 0;

    const int itemsInViewport = 8;

    List<ItemSlotUI> slotUIList;

    Inventory inventory;
    RectTransform itemListRect;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach(Transform child in itemList.transform)
            Destroy(child.gameObject); // This is how you Destroy all the children of a Game Object in Unity
    
        slotUIList = new List<ItemSlotUI>();
        foreach(var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform); // Instantiate the prefab and Add it as a child of itemList Object
            slotUIObj.setData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack)
    {
        int prevSelection = selectedItem;

        if(Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        if(Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0 , inventory.Slots.Count - 1);

        if(prevSelection != selectedItem)
            UpdateItemSelection();

        if(Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if(i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Descrption;

        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport/2, 0, selectedItem) * slotUIList[0].Height; // Get the scroll position (multiply it by the slot's height, 50 in this case)
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    
        bool showUpArrow = selectedItem > itemsInViewport/2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewport/2 < slotUIList.Count; // if it's true, there are more items below that can be scrolled
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
