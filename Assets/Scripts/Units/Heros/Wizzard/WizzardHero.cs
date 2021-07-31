using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizzardHero : Hero
{
    protected override float BaseMaxHealth => 500;
    protected override float BaseMaxMana => 100;
    protected override float BaseArmor => 30;
    protected override float BaseAttackPower => 30;

    protected override double BaseMovmentSpeed => 2.0f;

    public override float AttackSpeed => 0.7f;
    public override float AttackRange => 3.0f;


    private new void Awake()
    {
        base.Awake();
        OnBirth();
    }

    private new void Update()
    {
        base.Update();

    }
}
