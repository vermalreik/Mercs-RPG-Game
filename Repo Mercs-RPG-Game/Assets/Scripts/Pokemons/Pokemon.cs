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
    public Dictionary<Stat, int> Stats {get; private set;} // Dictionary in C# to store the value of the stats at the current level
    // Dictionary in C# is kind of like a list
    // but the difference is that in list we just store a list of values
    // but in Dictionaries, along with the value, we also store a key

    public Dictionary<Stat, int> StatBoosts{ get; private set;}

    public void Init() // Init stands for Initialization
    {
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

        CalculateStats();
        HP = MaxHp;

        StatBoosts = new Dictionary<Stat, int>()
        {   // initialize boost value for all of the stats to 0 
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
        };
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt(Base.Defense * Level / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5);

        MaxHp = Mathf.FloorToInt(Base.Speed * Level / 100f) + 10;
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        
        // Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        
        return statVal;
    }

    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            // we have to camp it between -6 and 6, since the stat can only be boosted by six levels
        
            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    // Properties for each of the stats
    public int Attack
    {
        get { return GetStat(Stat.Attack); } // calculate the value of the stat at the current level of the pokemon
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp { get; private set; }

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

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;
        
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
