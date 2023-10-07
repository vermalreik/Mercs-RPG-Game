using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy } // Busy: when the player and the enemy are attacking
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }


public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        //StartCoroutine(dialogBox.TypeDialog($"A wild {playerUnit.Pokemon.Base.Name} appeared."));
        // In C# if you start a string with a dollar sign then you can add values of different variables inside the string
        // it's called string interpolation

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

        // Si no estuvieran dentro de una rutina las llamaria de esta forma:
        // StartCoroutine(xD);
        // pero al estar dentro de una rutina las llamo asi:
        // yield return , esto esperara a k esta rutina se complete y solo despues de eso the execution will come down

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // The player pokemon perform the attack or move and the enemy pokemon will take damage of it
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy; // because if the state is still in PlayerMove the player will still be able to change the value of the current move

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");

        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if(damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted");
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if(damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} fainted");
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");
        if (damageDetails.TypeEffectiveveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        if (damageDetails.TypeEffectiveveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentAction < 1)
                ++currentAction;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(currentAction == 0)
            {
                // Fight
                PlayerMove();
            }
            else if(currentAction == 1)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currentMove > 0)
                --currentMove;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentMove > 1)
                currentMove -=2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if(Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
