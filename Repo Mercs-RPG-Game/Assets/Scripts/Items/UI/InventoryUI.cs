using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy }

public class InventoryUI : SelectionUI<TextSlot>
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Action<ItemBase> onItemUsed;
    int selectedCategory = 0;

    MoveBase moveToLearn;

    InventoryUIState state;

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

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach(Transform child in itemList.transform)
            Destroy(child.gameObject); // This is how you Destroy all the children of a Game Object in Unity
    
        slotUIList = new List<ItemSlotUI>();
        foreach(var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform); // Instantiate the prefab and Add it as a child of itemList Object
            slotUIObj.setData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());

        UpdateSelectionInUI();
    }

    public override void HandleUpdate()
    {
        int prevCategory = selectedCategory;

        if(Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedCategory;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedCategory;

        // clamping (you can not go beyond right or left when you reach the end) or rotating selection. Do it with Category before clamping hte items
        // selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1); //Clamping // we have 3 category of items on our inventory
        // Rotating Selection
        if(selectedCategory > Inventory.ItemCategories.Count - 1)
            selectedCategory = 0;
        else if(selectedCategory < 0)
            selectedCategory = Inventory.ItemCategories.Count - 1;
        
        if(prevCategory != selectedCategory)
        {
            ResetSelection();
            categoryText.text = Inventory.ItemCategories[selectedCategory];
            UpdateItemList();
        }

        base.HandleUpdate();
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if(GameController.Instance.State == GameState.Shop)
        {
            onItemUsed?.Invoke(item);
            state = InventoryUIState.ItemSelection;
            yield break;
        }

        if(GameController.Instance.State == GameState.Battle)
        {
            // In Battle
            if(!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item can not be used in battle");
                state = InventoryUIState.ItemSelection;
                yield break; // 'break' to stop this coroutine, so we don't execute de rest of the code
            }
        }
        else
        {
            // Outside Battle
            if(!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"This item can not be used outside battle");
                state = InventoryUIState.ItemSelection;
                yield break; // 'break' to stop this coroutine, so we don't execute de rest of the code
            }
        }
        if(selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if(item is TmItem)
                partyScreen.ShowTmIsUsable(item as TmItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var pokemon = partyScreen.SelectedMember;

        // Handle Evolution Items
        if(item is EvolutionItem)
        {
            var evolution = pokemon.CheckForEvolution(item);
            if(evolution != null)
            {
                yield return EvolutionManager.i.Evolve(pokemon, evolution);
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
                ClosePartyScreen();
                yield break;
            }
        }

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if(usedItem != null)
        {
            if(usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}"); // you can add a new field on ItemBase.cs for customized messages
            
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if(selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem; // "as" to cast the value that is returned from that function into a Tm item. It will return null if the item is actually not a Tm item
        if(tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if(pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}");
            yield break;
        }

        if(!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}");
            yield break;
        }

        if(pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.Name}");
            yield return DialogManager.Instance.ShowDialogText($"but it can not learn more than {PokemonBase.MaxNumOfMoves} moves");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Choose a move you want to forget", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove); // using linq we convert a list of Move class into a list of MoveBase class
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        var slots = inventory.GetSlotsByCategory(selectedCategory);

        if(slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
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

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);
        if(moveIndex == PokemonBase.MaxNumOfMoves)
        {
             // Don't learn the new move
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}");
        }
        else
        {
            // Forget the selected move and learn new move
            var selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}");

            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
