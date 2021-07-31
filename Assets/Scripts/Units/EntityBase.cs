using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum Team
{
    TeamA,
    TeamB,
}

public abstract class EntityBase : MonoBehaviour
{
    [HideInInspector]
    public UnitsMenager Major;

    protected abstract float BaseMaxHealth { get; }
    public float MaxHealth { get; protected set; }

    protected abstract float BaseMaxMana { get; }
    public float MaxMana { get; protected set; }

    protected abstract float BaseArmor { get; }
    public float Armor { get; protected set; }

    protected abstract float BaseAttackPower { get; }
    public float AttackPower { get; protected set; }

    public bool IsKilled => Health <= 0;

    public virtual void OnBirth()
    {
        Health = MaxHealth;
        Mana = MaxMana;
    }
    public virtual void OnDeath()
    {
        Debug.Log($"Unit: {this} has been killed.");
        Major.RemoveUnit(this);
    }

    public float DamageDealt { get; private set; }
    public float DemageHealed { get; private set; }

    public float ManaRestored { get; private set; }
    public float ManaUsed { get; private set; }

    public float Health;
    public float Mana;

    public struct Attacker
    {
        public EntityBase buller;
        public float amount;

        public Attacker(EntityBase buller, float amount)
        {
            this.buller = buller;
            this.amount = amount;
        }
    }

    public LinkedList<Attacker> attackers = new LinkedList<Attacker>();

    public bool WasAttacedBy(EntityBase bullerCandidate)
    {
        return attackers.Any(attacker => attacker.buller == bullerCandidate);
    }

    public void AddDamge(EntityBase dealer, float demage)
    {
        if (dealer.team == this.team) return; //Cannot harm your team.

       // print($"{dealer} attacks for {demage}");
        attackers.AddLast(new Attacker(dealer, demage));

        if (demage > 0)
        {
            DamageDealt += demage;
        }
        else
        {
            DemageHealed -= demage;
        }
    }
    public void AddMana(float mana)
    {
        if (mana > 0)
        {
            ManaRestored += mana;
        }
        else
        {
            ManaUsed -= mana;
        }
    }


    public Team team;

    public EffectMenager Effects = new EffectMenager();

    public void ResetMaxStatsCalc()
    {
        MaxHealth = BaseMaxHealth;
        MaxMana = BaseMaxMana;
        Armor = BaseArmor;
        AttackPower = BaseAttackPower;
    }

    public float ArmorLogicFunction(float armor_points)
    {
        return 1.0f / (1 + Mathf.Exp(0.1f * armor_points - 3f));
    }

    private void ApplyDamage()
    {
        var changeHp = (DamageDealt - DemageHealed);
        changeHp -= changeHp * ArmorLogicFunction(Armor);
        Health = Mathf.Clamp(Health - changeHp, 0, MaxHealth);

        var changeMn = ManaUsed - ManaRestored;
        Mana = Mathf.Clamp(Mana - changeMn, 0, MaxMana);

        DamageDealt = 0;
        DemageHealed = 0;

        ManaUsed = 0;
        ManaRestored = 0;

        if (Health <= 0)
        {
            OnDeath();
        }
    }

    protected void Update()
    {

        ResetMaxStatsCalc();

        Effects.ApplyEffects(this);

        ApplyDamage();

        Effects.TimeUpdate();
    }
}
