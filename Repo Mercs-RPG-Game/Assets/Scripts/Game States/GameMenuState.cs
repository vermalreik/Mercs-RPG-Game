using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    public static GameMenuState i { get; private set; }

    private void Awake() {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;
    }

    public override void Execute()
    {
        if(Input.GetKeyDown(KeyCode.X))
            gc.StateMachine.Pop();
    }
}
