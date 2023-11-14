//using System;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

// :D En esta clase calculamos los stats de cada Pokemon basandonos en su nivel, mediante formulas obtenidas de la Bulbapedia
[System.Serializable]
public class Pokemon // This is going to be plain C#, thats why we dont inherit from MonoBeahaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

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

    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; } // Son los moves que tiene un pokemon en concreto
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats {get; private set;} // Dictionary in C# to store the value of the stats at the current level
    // Dictionary in C# is kind of like a list
    // but the difference is that in list we just store a list of values
    // but in Dictionaries, along with the value, we also store a key

    public Dictionary<Stat, int> StatBoosts{ get; private set;}
    public Condition Status{ get; private set;}
    public int StatusTime{ get; set;}
    public Condition VolatileStatus{ get; private set;}
    public int VolatileStatusTime{ get; set;}

    public Queue<string> StatusChanges{ get; private set; }
    // Using a queue, the first messages that we add to the queue should be shown first in our dialogue box
    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged; // Observer Pattern

    public void Init() // Init stands for Initialization
    {
        // Generate Moves of pokemons based on its level
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base)); // Here we create a move :D usamos la clase Move.cs

            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
                break; // exit the loop calling break
        }
        // :D hasta aki

        Exp = Base.GetExpForLevel(Level);

        CalculateStats();
        HP = MaxHp;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetObjectByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        if(saveData.statusId != null)
            Status = ConditionsDB.Conditions[saveData.statusId.Value];
        else
            Status = null;

        // Restore moves
        Moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt(Base.Defense * Level / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5);

        MaxHp = Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10 + Level;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {   // initialize boost value for all of the stats to 0 
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0}
        };
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
        
            if(boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
                // Enqueue to add :D y Dequeue to remove
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public bool CheckForLevelUp()
    {
        if(Exp > Base.GetExpForLevel(level + 1)) // + 1 because it's for the next level
        {
            ++level;
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
        // If the list is empty then "FirstOrDefault()" will return null
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if(Moves.Count > PokemonBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel == level);
    }

    public void Evolve(Evolution evolution)
    {
        _base = evolution.EvolvesInto;
        CalculateStats();
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

        DecreaseHP(damage);

        return damageDetails;;

    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void SetStatus(ConditionID conditionId)
    {
        if(Status != null) return;

        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if(VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];

        // if the enemy doesn't have any move with PP, then this list will be empty
        // and we'll get a null reference 0
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if(Status?.OnBeforeMove != null)
        {
            if(!Status.OnBeforeMove(this))
                canPerformMove = false;
        }

        if(VolatileStatus?.OnBeforeMove != null)
        {
            if(!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        // Null Conditional Operator
        // It'll only execute the code to the right of it, if the item on which its supplied is not null
        // we only call the OnAfterTurn if it's not null :D so we don't have a null reference error
    
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveveness { get; set; }
}

[System.Serializable]
public class PokemonSaveData
{
    public string name; // We use the name as an ID to identify the PokemonBase
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}
