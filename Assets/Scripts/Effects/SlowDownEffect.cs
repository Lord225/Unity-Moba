using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownEffect : Effect
{
    public SlowDownEffect(float amount, bool isDecay, float duration) : base(duration)
    {
        Amount = amount;
        IsDecay = isDecay;
    }

    public float Amount;
    public bool IsDecay;

    public override void ApplyEffect(EntityBase target)
    {
        if (!(target is EntityNavigable nav)) return;

        var isDecay = IsDecay ? 1.0f : 0.0f;
        nav.Speed -= nav.Speed * Amount * (1 - get_finish_pct()*isDecay);
    }

    public override bool IsVisible => true;
}
