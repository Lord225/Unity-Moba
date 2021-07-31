using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class LinkedListExt {
    public static void RemoveAll<T>(this LinkedList<T> ts, Predicate<T> match)
    {
        var node = ts.First;

        while (node != null)
        {
            if (match(node.Value))
            {
                Debug.Log("Removing Effect:" + node.Value);
                ts.Remove(node);
            }

            node = node.Next;
        }
    }
}

public class EffectMenager
{
    public LinkedList<Effect> effects = new LinkedList<Effect>();

    public void ApplyEffects(EntityBase target)
    {
        foreach (var effect in effects)
        {
            effect.ApplyEffect(target);
        }
    }

    public void TimeUpdate()
    {
        var curTime = Time.realtimeSinceStartup;

        effects.RemoveAll(effect => effect.EndTime < curTime && effect.EndTime > 0);
    }

    public void AddEffect(Effect newEffect)
    {
        Debug.Log("Adding Effect:" + newEffect);
        effects.AddLast(newEffect);
    }
}
