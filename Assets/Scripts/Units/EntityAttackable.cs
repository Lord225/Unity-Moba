using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public abstract class EntityAttackable : EntityNavigable
{
    public abstract float AttackSpeed { get; }
    public abstract float AttackRange { get; }
    public bool IsBaseAttackOnCooldown { get; private set; }

    public EntityBase Target;

    protected new void Update()
    {
        base.Update();

        BaseAttackChecker();
    }

    void BaseAttackChecker()
    {
        if (Target != null && !Target.IsKilled)
        {
            if (Vector3.Distance(Target.transform.position, transform.position) < AttackRange && !IsBaseAttackOnCooldown)
            {
                //Debug.Log($"Unit {this.gameObject} attacks: {Target.gameObject}");
                StartCoroutine(BaseAttackCorutine());
            }
        }
        else
        {
            Target = null;
        }
    }

    public float Distance(EntityBase other) => Distance(other.transform);
    public float Distance(Transform other) => Vector3.Distance(other.position, this.transform.position);


    public bool IsInRange(EntityBase other) => Distance(other) < AttackRange;
    public bool IsInRange(Transform other) => Distance(other) < AttackRange;

    IEnumerator BaseAttackCorutine()
    {
        var cooldown = 1 / AttackSpeed;

        IsBaseAttackOnCooldown = true;

        yield return new WaitForSeconds(cooldown);

        IsBaseAttackOnCooldown = false;
    }

    [UsedImplicitly]
    public void BaseAttackAnimEvent()
    {
        if (Target != null)
        {
            BaseAttack();
        }
    }

    public virtual void BaseAttack()
    {
        Target.AddDamge(this, AttackPower);
    }

}
