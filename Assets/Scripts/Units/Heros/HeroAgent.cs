using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class HeroAgent : Agent
{
    HeuristicInput PlayerInput;
    Hero me;

    public override void Initialize()
    {
        me = GetComponent<Hero>();
        PlayerInput = GetComponent<HeuristicInput>();
    }

    public override void OnEpisodeBegin()
    {

    }

    class Actions
    {
        public bool IsQUsed;
        public bool IsWUsed;
        public bool IsRUsed;
        public bool IsAttackUsed;

        public bool IsWalkingInvoked;

        public Vector3 WalkDir;
        public Vector3 AttakDir;
        public EntityBase Target;
    }

    void ApplyActions(Actions actions)
    {
        if(actions.IsWalkingInvoked)
            me.StandardMove(actions.WalkDir);
        if (actions.IsQUsed)
            me.UseSkill(Hero.SkillKey.Q, actions.AttakDir, actions.Target);
        if (actions.IsWUsed)
            me.UseSkill(Hero.SkillKey.W, actions.AttakDir, actions.Target);
        if (actions.IsRUsed)
            me.UseSkill(Hero.SkillKey.R, actions.AttakDir, actions.Target);
        if (actions.IsAttackUsed)
            me.BaseAttack(actions.Target);
    }


    // Konieczny Input:
    // Poruszanie siê (18)
    // U¿yæ skilla Q  (19)
    // U¿yæ skilla W  (20)
    // U¿yæ skilla R  (21)
    // U¿yj AutoAtaku (22)
    // 
    // Target do autoataku/skilla (Bohater zawsze + 10 slotów dla jednostek (je¿eli wybrany niew³aœciwy, ingoruje))
    // kiernuk dla skilla
    // 
    public override void OnActionReceived(float[] vectorAction)
    {
        var actions = new Actions();

        var serializedEnd = 2 * PlayerInput.OneHotSize + 2;

        var walkDirSerialized = vectorAction.SubArray(0, serializedEnd);

        var walkDirDeserialized = PlayerInput.ActionDeserialize(walkDirSerialized);
        actions.WalkDir = PlayerInput.ConvertFromActionSpace(walkDirDeserialized) + this.transform.position; //Respects from players origin

        actions.IsWalkingInvoked = vectorAction[serializedEnd] > 0.5f;

        serializedEnd += 1;
        actions.IsQUsed = vectorAction[serializedEnd] > 0.5f;
        serializedEnd += 1;
        actions.IsWUsed = vectorAction[serializedEnd] > 0.5f;
        serializedEnd += 1;
        actions.IsRUsed = vectorAction[serializedEnd] > 0.5f;
        serializedEnd += 1;
        actions.IsAttackUsed = vectorAction[serializedEnd] > 0.5f;

        //var attakDirSerialized = vectorAction.SubArray(serializedEnd, serializedEnd + 2 * PlayerInput.OneHotSize + 2);
        //serializedEnd += 2 * PlayerInput.OneHotSize + 2;


        //var attakDirDeserialized = PlayerInput.ActionDeserialize(attakDirSerialized);
        //actions.AttakDir = PlayerInput.ConvertFromActionSpace(attakDirDeserialized); //Respects from players origin
        ApplyActions(actions);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var relativeTo = transform.position;
        // Wszystko.
        // 1. czas od rozpoczêcia gry
        // 2. czas do spawnu fali
        // Dla jednostek (Miniony - pool 10 najbli¿szych jednostek, wrogich)
        // 3. Pozycja (x, y)
        // 4. Facing (cos, sin)
        // 5. isAttacing
        // 6. Ostatni czas od ataku
        // 7. Maksymalne HP
        // 8. Aktualnie HP
        // 9. AttackDamage
        // 10. AttakSpeed
        // 11. Armor
        // 12. Movment Speed
        // 13. Unit Type
        //
        // + 10 najbli¿szych swoich
        //
        // Dla Herosów (Enemy potem Ten)
        // To samo co dla Jednostek
        // isAlive
        // NumOfDeaths
        // Respawn Time
        // level
        // mana, max+current+ mana/hp change
        // Is Visible
        // usingAbility

        // Czujniki dooko³a, ró¿ne warstwy
        // 
        // 

        // Collect Units
        var units = me.Major.CollectClosests(relativeTo, 5.0f, limit: PlayerInput.EntityCells);
        var array = HeuristicInput.EntityData.GetFilldedArray(size: PlayerInput.EntityCells);
        for (int i = 0; i < units.Count; i++)
        {
            array[i].load_entity(units[i], relativeTo);
        }

    }

    // Konieczny Input:
    // Poruszanie siê (18)
    // U¿yæ skilla Q  (19)
    // U¿yæ skilla W  (20)
    // U¿yæ skilla R  (21)
    // U¿yj AutoAtaku (22)
    // 
    // Target do autoataku/skilla (Bohater zawsze + 10 slotów dla jednostek (je¿eli wybrany niew³aœciwy, ingoruje))
    // kiernuk dla skilla
    // 
    private void WriteOffset(ref float[] output, ref float[] input, int offset)
    {
        for (int i = 0; i < input.Length; i++)
        {
            output[i + offset] = input[i];
        }
    }    
    public override void Heuristic(float[] actionsOut)
    {
        var serializedEnd = 0;

        var mousePos   = PlayerInput.MousePos();
        var relativeTo = transform.position;

        // Collect Player mouse pos
        var moveDecoded = PlayerInput.ConvertToActionSpace(mousePos, relativeTo);
        var moveCoded   = PlayerInput.ActionSerialize(moveDecoded);
        WriteOffset(ref actionsOut, ref moveCoded, 0);
        serializedEnd += moveCoded.Length;

        // Collect mouse click
        actionsOut[serializedEnd] = Input.GetMouseButton(1) ? 1.0f : 0.0f; //TODO: Fix behav to more like GetMouseButtonDown with flag or sth.

        var clicedEntity = PlayerInput.ClickedEntity();

        if(!(clicedEntity is null))
        {
            if(clicedEntity.TryGetComponent<EntityBase>(out var entity))
            {
                //For sure entity was cliced.

                print($"CLICED ENTITY {entity}");
            }
        }

    }

}
