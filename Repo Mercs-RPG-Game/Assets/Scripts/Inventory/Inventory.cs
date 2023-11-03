using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Pokeballs, Tms } // Write it in the same order in whish you have it in the list

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

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);

        var item = currentSlots[itemIndex].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if(itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item; // return the item so we can use it in the dialog
        }

        return null; // indicates that the item is not used
    }

    public void RemoveItem(ItemBase item, int category)
    {
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if(itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

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
