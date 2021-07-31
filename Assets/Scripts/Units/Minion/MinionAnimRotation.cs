using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAnimRotation : MonoBehaviour
{
    private Minion _minionBase;
    private Animator _anim;

    private bool WasBaseAttackOnCooldownLastFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        _minionBase = GetComponent<Minion>();

        _anim = GetComponentInChildren<Animator>();
    }

    void TriggerAutoattackAnim()
    {
        if (_minionBase.Target == null) return;
        if (_minionBase.IsBaseAttackOnCooldown == WasBaseAttackOnCooldownLastFrame) return;

        _anim.SetTrigger("BaseAttack");
        WasBaseAttackOnCooldownLastFrame = !_minionBase.IsBaseAttackOnCooldown;
    }

    void LookAtAnim()
    {
        if (_minionBase.Target != null)
        {
            var dir = _minionBase.Target.transform.position-transform.position;

            dir.y = 0;//This allows the object to only rotate on its y axis
            var rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 10.0f * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _anim.speed = _minionBase.AttackSpeed * 2;

        TriggerAutoattackAnim();

        LookAtAnim();
    }
}
