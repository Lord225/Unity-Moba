using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public float EndTime;
    public float Duration;
    private readonly float _startTime;

    protected Effect(float duration)
    {
        Duration = duration;
        _startTime = Time.realtimeSinceStartup;
        EndTime = _startTime + Duration;
    }
    protected Effect(float duration, float endTime)
    {
        _startTime = Time.realtimeSinceStartup;
        EndTime = endTime;
    }

    public float get_finish_pct()
    {
        return (_startTime - EndTime) / Duration;
    }

    public abstract void ApplyEffect(EntityBase target);
    public abstract bool IsVisible { get; }
}
