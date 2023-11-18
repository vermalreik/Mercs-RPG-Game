using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    int selectedItem;

    List<ItemBase> availableItems;

    List<ItemSlotUI> slotUIList;

    const int itemsInViewport = 8;

    RectTransform itemListRect;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> availableItems)
    {
        this.availableItems = availableItems;
        
        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void HandleUpdate()
    {
        var prevSelection = selectedItem;

        if(Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        if(selectedItem != prevSelection)
            UpdateItemSelection();
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach(Transform child in itemList.transform)
            Destroy(child.gameObject); // This is how you Destroy all the children of a Game Object in Unity
    
        slotUIList = new List<ItemSlotUI>();
        foreach(var item in availableItems)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform); // Instantiate the prefab and Add it as a child of itemList Object
            slotUIObj.SetNameAndPrice(item);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    void UpdateItemSelection()
    {
        // Clamp selectedItem so in case an item was remove we won't get the index out of range Exception
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count -1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if(i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if(availableItems.Count > 0)
        {
            var item = availableItems[selectedItem];
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Descrption;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if(slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport/2, 0, selectedItem) * slotUIList[0].Height; // Get the scroll position (multiply it by the slot's height, 50 in this case)
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    
        bool showUpArrow = selectedItem > itemsInViewport/2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewport/2 < slotUIList.Count; // if it's true, there are more items below that can be scrolled
        downArrow.gameObject.SetActive(showDownArrow);
    }

}
