using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectile : PositionableSkill
{
    public override float ManaCost => 20;
    public override float CoolDown => 5.0f;

    public SkillProjectile(Hero myGuy) : base(myGuy) {}

    public override void UseSkill(Vector3 target)
    {
        Debug.Log($"USED SKILL {this.ToString()} with pos {target}");


    }

}
