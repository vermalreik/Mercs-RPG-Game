using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// :D En esta clase calculamos los stats de cada Pokemon basandonos en su nivel, mediante formulas obtenidas de la Bulbapedia
[System.Serializable]
public class Pokemon // This is going to be plain C#, thats why we dont inherit from MonoBeahaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base{ 
        get{
            return _base;
        }
    }
    public int Level { 
        get{
            return level;
        }
     }

    public int HP { get; set; }
    public List<Move> Moves { get; set; } // Son los moves que tiene un pokemon en concreto

    public void Init() // Init stands for Initialization
    {
        HP = MaxHp;

        // This code will generate de moves of pokemons based on its level
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base)); // Here we create a move :D usamos la clase Move.cs

            if (Moves.Count >= 4)
                break; // exit the loop calling break
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

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        // critical hit will do double the damage of normal attacks
        if(Random.value * 100f <= 6.25f) // chances that this happens
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial) ? SpDefense : Defense;
        
        // Formula taken from 
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;;

    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveveness { get; set; }
}
