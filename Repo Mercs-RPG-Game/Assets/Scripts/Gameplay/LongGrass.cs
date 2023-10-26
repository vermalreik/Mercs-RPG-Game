using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void onPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 10) // owo Escribo UnityEngine porque al importar using. System para usar eventos "Random" queda ambiguo por estar en las dos librerias
            {
                //Debug.Log("Encountered a wild pokemon");
                player.Character.Animator.IsMoving = false;
                GameController.Instance.StartBattle();
            }
    }
}
