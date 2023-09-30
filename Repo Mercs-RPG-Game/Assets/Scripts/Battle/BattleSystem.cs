using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;

    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
    }
}
