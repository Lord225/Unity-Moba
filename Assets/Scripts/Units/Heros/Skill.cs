using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public abstract float ManaCost { get; }
    public abstract float CoolDown { get; }

    public float CoolDownTimer = 0;

    private Hero _myGuy;

    protected Skill(Hero myGuy)
    {
        _myGuy = myGuy;
    }

    public bool IsOnCooldown()
    {
        return CoolDownTimer < CoolDown;
    }

    private void Update()
    {
        if (CoolDownTimer < CoolDown)
        {
            CoolDownTimer += Time.deltaTime;
        }
    }
    public enum SkillType
    {
        Usable,
        Targetable,
        Positionable,
    }

    public static SkillType GetSkillType(Skill skill)
    {
        switch (skill)
        {
            case PositionableSkill positionableSkill:
                return SkillType.Positionable;
            case TargetableSkill targetableSkill:
                return SkillType.Targetable;
            case UseOnlySkill useOnlySkill:
                return SkillType.Usable;
            default:
                throw new ArgumentOutOfRangeException(nameof(skill));
        }
    }
}

public abstract class UseOnlySkill : Skill
{
    public abstract void UseSkill();

    protected UseOnlySkill(Hero myGuy) : base(myGuy) {}
}

public abstract class TargetableSkill : Skill
{
    public abstract void UseSkill(EntityBase target);

    protected TargetableSkill(Hero myGuy) : base(myGuy) {}
}

public abstract class PositionableSkill : Skill
{
    public abstract void UseSkill(Vector3 target);

    protected PositionableSkill(Hero myGuy) : base(myGuy) {}
}