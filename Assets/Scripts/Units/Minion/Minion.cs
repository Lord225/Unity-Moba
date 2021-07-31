using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Minion : EntityAttackable
{
    public Transform MinionPrimaryDestination;

    protected override float BaseMaxHealth => 100;
    protected override float BaseMaxMana => 0;
    protected override float BaseArmor => 20;
    protected override float BaseAttackPower => 10;

    protected override double BaseMovmentSpeed => 0.8;

    public override float AttackSpeed => 1.2f+(UnityEngine.Random.value-0.5f);

    public override float AttackRange => 0.5f;

    public float DetectRange => 3.5f;

    private void Start()
    {
        SetStoppingDistance(AttackRange);
    }

    private new void Update()
    {
        base.Update();

        if (FindBestTargetToBull(out var to_attack))
        {
            this.Destination = to_attack.transform.position;
            this.Target = to_attack;
        }
        else
        {
            this.Destination = MinionPrimaryDestination.position;
            this.Target = to_attack;
        }

        if (Vector3.Distance(transform.position, MinionPrimaryDestination.position) < 1f)
        {
            Major.RemoveUnit(this);
        }

        attackers.Clear();
    }

    bool FindBestTargetToBull(out EntityBase to_attack)
    {
        float minDis = float.MaxValue;
        EntityBase closest = null;

        var myHero = Major.GetYourBoi(team);
        var myBull = Major.GetEnemy(team);

        if (!(myHero is null) && !(myBull is null))
        {
            if (myHero.WasAttacedBy(myBull))
            {
                to_attack = myBull;
                return true;
            }
        }
        if(!(myBull is null) && IsInRange(myBull))
        {
            closest = myBull;
            minDis = Distance(myBull);
        }


        foreach (var unit in Major.Units)
        {
            if (unit.team == this.team || unit.IsKilled) continue;
            
            if (!IsInRange(unit)) continue;

            var dis = Distance(unit);

            if (dis > minDis) continue;

            closest = unit;
            minDis = Distance(unit);
        }
       
        
        to_attack = closest;

        return to_attack != null;
    }

    public override void BaseAttack()
    {

        //AttackPower +/- 5.0f
        Target.AddDamge(this, AttackPower+(UnityEngine.Random.value*2-1.0f)*5.0f);
    }

}
