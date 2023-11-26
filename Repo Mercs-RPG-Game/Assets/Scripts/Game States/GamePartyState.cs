using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class GamePartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    public static GamePartyState i { get; private set;}

    private void Awake() {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnPokemonSelected(int selection)
    {
        if(gc.StateMachine.GetPrevState() == InventoryState.i)
        {
            // Use Item
            Debug.Log($"Use Item");
        }
        else
        {
            // Todo: Open summary screen
            Debug.Log($"Selected pokemon at index { selection}");
        }
    }

    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
