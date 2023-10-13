using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver } // Busy: when the player and the enemy are attacking
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }


public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty; // use this. pbecause the name of the parameter and the variable is the same
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        //StartCoroutine(dialogBox.TypeDialog($"A wild {playerUnit.Pokemon.Base.Name} appeared."));
        // In C# if you start a string with a dollar sign then you can add values of different variables inside the string
        // it's called string interpolation

        partyScreen.Init();

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

        // Si no estuvieran dentro de una rutina las llamaria de esta forma:
        // StartCoroutine(xD);
        // pero al estar dentro de una rutina las llamo asi:
        // yield return , esto esperara a k esta rutina se complete y solo despues de eso the execution will come down

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if(playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    void BattleOver(bool won) // its a function that triggers the battle over state
    {
        state = BattleState.BattleOver;  
        playerParty.Pokemons.ForEach(p => p.OnBattleOver()); // call  OnBattleOver of all the pokemons
        OnBattleOver(won); // It's an event that Notifies the Game Controller that the battle is over
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        //StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        //print("Party Screen");
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // The player pokemon perform the attack or move and the enemy pokemon will take damage of it
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove; // because if the state is still in MoveSelection the player will still be able to change the value of the current move

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);
        
        // If the battle stat was not changed by RunMove, then go to the next step
        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

         // If the battle stat was not changed by RunMove, then go to the next step
        if(state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if(!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break; // to stop the Coroutine
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if(CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(0.5f);
            targetUnit.PlayHitAnimation();

            if(move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach(var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if(rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }

            if(targetUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} fainted");
                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(1f);

                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed");
        }

        // Statuses like burn or psn will hurt the pokemon after the turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();
        if(sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} fainted");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);

            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if(effects.Boosts != null)
        {
            if(moveTarget == MoveTarget.Self)
                source.ApplyBoost(effects.Boosts);
            else
                target.ApplyBoost(effects.Boosts);
        }

        // Status Condition
        if (effects.Status != ConditionID.none)
         {
            target.SetStatus(effects.Status);
        }

        // Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
         {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while(pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if(faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon != null)
                OpenPartyScreen();
            else    
                BattleOver(false); // player lost battle
        }
        else
            BattleOver(true);
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

    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if(state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentAction-= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if(currentAction == 1)
            {
                // Bag
            }
            else if(currentAction == 2)
            {
                // Pokemon
                OpenPartyScreen();
            }
            else if(currentAction == 3)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentMove-= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentMember-= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if(selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if(selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }

    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;

        if(playerUnit.Pokemon.HP > 0)
        {
            currentPokemonFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}.");

        if(currentPokemonFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyMove());

    }
}
