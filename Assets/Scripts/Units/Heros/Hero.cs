using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : EntityAttackable
{
    [Space]
    public Skill Q;

    public Skill W;

    public Skill R;


    public void StandardMove(Vector3 positionOnMap)
    {
        Destination = positionOnMap;
    }

    public void BaseAttack(EntityBase ToAttack)
    {
        Target = ToAttack;
    }

    public enum SkillKey
    {
        Q,
        W,
        R,
    }

    public void UseSkill(Skill skillToUse, Vector3 position, EntityBase target)
    {
        if (skillToUse == null)
            throw new NullReferenceException("SKILL CANNOT BE NULL");
        switch (skillToUse)
        {
            case PositionableSkill positionableSkill when !(position.Equals(Vector3.zero)):
                positionableSkill.UseSkill(position);
                break;
            case TargetableSkill targetableSkill when !(target is null):
                targetableSkill.UseSkill(target);
                break;
            case UseOnlySkill useOnlySkill:
                useOnlySkill.UseSkill();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Q), "Skill type/argument is invalid.");
        }
    }

    public void UseSkill(SkillKey key, Vector3 position, EntityBase target)
    {
        switch (key)
        {
            case SkillKey.Q:
                UseSkill(Q, position, target);
                break;
            case SkillKey.W:
                UseSkill(W, position, target);
                break;
            case SkillKey.R:
                UseSkill(R, position, target);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, null);
        }

    }

    public override void OnDeath()
    {

    }
    
    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (Target.IsKilled)
        {
            Target = null;
        }
    }
}
