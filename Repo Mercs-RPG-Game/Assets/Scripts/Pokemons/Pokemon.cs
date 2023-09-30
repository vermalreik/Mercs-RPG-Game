using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon // This is going to be plain C#, thats why we dont inherit from MonoBeahaviour
{
    public PokemonBase Base{ get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;

        // This code will generate de moves of pokemons based on its level
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }
        // :D hasta aki
    }

    public int Attack
    {
        get { return Mathf.FloorToInt(Base.Attack * Level / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt(Base.Defense * Level / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt(Base.Speed * Level / 100f) + 5; }
    }

    public int MaxHp
    {
        get
        {
            return Mathf.FloorToInt(Base.Speed * Level / 100f) + 10;
        }
    }
}
