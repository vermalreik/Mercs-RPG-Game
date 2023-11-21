using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        if(animator.IsSurfing || isJumpingToWater)
            yield break;
        
        yield return DialogManager.Instance.ShowDialogText("The water is deep blue!");
    
        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Surf"));
    
        if(pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {pokemonWithSurf.Base.Name} use surf?",
                choices: new List<string>() {"Yes", "No"},
                onChoiceSelected: (selection) => selectedChoice = selection );

            if(selectedChoice == 0)
            {
                // Yes
                 yield return DialogManager.Instance.ShowDialogText($"{pokemonWithSurf.Base.Name} used surf!");

                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingToWater = false;

                animator.IsSurfing = true;
            }
            
        }
    }

    public void onPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 10) // owo Escribo UnityEngine porque al importar using. System para usar eventos "Random" queda ambiguo por estar en las dos librerias
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.Water);
        }
    }
}
