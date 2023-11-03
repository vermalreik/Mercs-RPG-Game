using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> pokeballSlots;
    [SerializeField] List<ItemSlot> tmSlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake() {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "POKEBALLS", "TMs & HMs"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon)
    {
        var item = slots[itemIndex].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if(itemUsed)
        {
            RemoveItem(item);
            return item; // return the item so we can use it in the dialog
        }

        return null; // indicates that the item is not used
    }

    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if(itemSlot.Count == 0)
            slots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

}

[Serializable] // Since we have a List<ItemSlot>, the ItemSlot should be [Serializable] in order ofr it to be shown in the Inspector
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item => item;
    public int Count{
        get => count;
        set => count = value;
    }
}
