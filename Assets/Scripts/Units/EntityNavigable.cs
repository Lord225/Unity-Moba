using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EntityNavigable : EntityBase
{
    protected abstract double BaseMovmentSpeed { get; }
    public double Speed { get; set; }

    public Vector3 Destination
    {
        get => _navigation.destination;
        set
        {
            if (IsKilled) return;
            _navigation.destination = value;
        }
    }

    private NavMeshAgent _navigation;

    public void SetStoppingDistance(float distance) => _navigation.stoppingDistance = distance;

    protected void Awake()
    {
        _navigation = GetComponent<NavMeshAgent>();
    }

    protected new void Update()
    {
        Speed = BaseMovmentSpeed;

        base.Update();

        _navigation.speed = (float)Speed;
    }
}
