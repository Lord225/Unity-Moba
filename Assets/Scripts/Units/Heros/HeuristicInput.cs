using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return new ArraySegment<T>(array, offset, length).ToArray();
    }
}

public class HeuristicInput : MonoBehaviour
{
    public Camera camera;
    public int CellsAmount = 4;
    public float Size = 10.0f;

    public float click_treshold = 0.5f;

    public int EntityCells = 4;

    public int OneHotSize => CellsAmount * 2 + 1;

    private void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public Vector3 MousePos()
    {
        const int layerMask = 1 << 6;
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out var hit, layerMask) ? hit.point : Vector3.zero;
    }

    public Transform ClickedEntity()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.CompareTag("Minion") || hit.transform.CompareTag("Player"))
                return hit.transform;
        }
        return null;
    }


    /// <summary>
    /// Zamienia Koordynaty pos, z centrum w origin na postaæ 'przyjazn¹' AI
    /// Pierwsze dwie wartoœci int, int to one-hotowe wektory zaznaczaj¹ce kratkê w której znajdujê siê pos
    /// nastêpnie wartoœci float, float to 'korekta', wektor o jaki nale¿y siê przesun¹æ z kwadratu do dok³adnej pozycji.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public (int, int, float, float) ConvertToActionSpace(Vector3 origin, Vector3 pos)
    {
        var diff = origin-pos;

        diff /= Size;

        diff *= CellsAmount;

        var value = (Mathf.Clamp(diff.x,-CellsAmount, CellsAmount), Mathf.Clamp(diff.z, -CellsAmount, CellsAmount));

        var (x, y) = (Mathf.RoundToInt(value.Item1), Mathf.RoundToInt(value.Item2));

        var (ofx, ofy) = (x - diff.x, y - diff.z);
        return (x + CellsAmount, y + CellsAmount, ofx, ofy);
    }

    public (int, int, float, float) ActionDeserialize(float[] vectorAction)
    {
        Debug.Assert(vectorAction.Length == OneHotSize * 2 + 2, 
            $"Action Space for serialazing using ActionSerialize have {OneHotSize * 2 + 2} floats but passed one have lenght of {vectorAction.Length}");

        var onehot = vectorAction.SubArray(0, OneHotSize);

        //                   Index of maximum
        var idx = onehot
            .Select((value, index) => new { Value = value, Index = index })
            .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
            .Index;

        var onehot2 = vectorAction.SubArray(OneHotSize, OneHotSize);

        var idy = onehot2
            .Select((value, index) => new { Value = value, Index = index })
            .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
            .Index;

        var baseindex = 2 * OneHotSize;
        return (idx, idy, vectorAction[baseindex], vectorAction[baseindex + 1]);
    }


    public float[] ActionSerialize((int, int, float, float) tuple)
    {
        return ActionSerialize(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
    }
    public float[] ActionSerialize(int x, int y, float ofx, float ofy)
    {
        var output = new float[OneHotSize * 2 + 2];

        Debug.Assert(x >= 0 && x < OneHotSize, $"x cord wasn't in bounds: {x}");
        Debug.Assert(y >= 0 && y < OneHotSize, $"y cord wasn't in bouds: {x}");

        output[x] = 1.0f;
        output[OneHotSize + y] = 1.0f;

        output[2 * OneHotSize] = ofx;
        output[2 * OneHotSize + 1] = ofy;

        return output;
    }

    public Vector3 ConvertFromActionSpace((int, int, float, float) tuple)
    {
        return ConvertFromActionSpace(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
    }
    public Vector3 ConvertFromActionSpace(int x, int y, float ofx, float ofy)
    {
        var (posxx, posyy) = ((x - CellsAmount) - ofx, (y - CellsAmount) - ofy);

        posxx /= CellsAmount;
        posyy /= CellsAmount;

        posxx *= Size;
        posyy *= Size;

        return new Vector3(posxx, 0, posyy);
    }
    public class EntityData
    {
        float exists = 0.0f;

        float mana = 0.0f;
        float maxmana = 0.0f;
        float health = 0.0f;
        float maxhealth = 0.0f;
        float attack = 0.0f;
        float armor = 0.0f;
        Vector3 relative_pos = Vector3.zero;
        float rotSin = 0.0f;
        float rotCos = 0.0f;

        float speed = 0.0f;

        float As = 0.0f;
        float Ar = 0.0f;
        float Ap = 0.0f;

        public void load_entity(EntityBase target, Vector3 pos)
        {
            exists = 1.0f;

            mana = target.Mana;
            maxmana = target.MaxMana;
            health = target.Health;
            maxhealth = target.MaxHealth;

            attack = target.AttackPower;
            armor = target.Armor;

            relative_pos = pos-target.transform.position;

            var angle = target.transform.forward;
            rotSin = Mathf.Sin(angle.x);
            rotCos = Mathf.Cos(angle.y);

            if (target is EntityNavigable nav)
            {
                speed = (float)nav.Speed;
            }
            if(target is EntityAttackable atc)
            {
                Ar = atc.AttackRange;
                Ap = atc.AttackPower;
                As = atc.AttackSpeed;
            }
        }

        public float[] ToArray()
        {
            var vec = new List<float>();

            vec.Add(exists);

            vec.Add(mana);
            vec.Add(maxmana);
            vec.Add(health);
            vec.Add(maxhealth);
            vec.Add(attack);
            vec.Add(armor);

            vec.Add(relative_pos.x);
            vec.Add(relative_pos.y);
            vec.Add(relative_pos.z);

            vec.Add(rotSin);
            vec.Add(rotCos);
            vec.Add(speed);

            vec.Add(As);
            vec.Add(Ar);
            vec.Add(Ap);

            return vec.ToArray();
        }

        public static float[] SerializeArray(EntityData[] arrayOfEntityData)
        {
            var vec = new List<float>();

            foreach(var data in arrayOfEntityData)
            {
                vec.AddRange(data.ToArray());
            }

            return vec.ToArray();
        }

        public static EntityData[] GetFilldedArray(int size)
        {
            var vec = new List<EntityData>(size);
            for (int i = 0; i < size; i++)
            {
                vec.Add(new EntityData());
            }
            return vec.ToArray();
        }
    }

    public EntityBase ToAttack(EntityBase[] entityList, float[] selection)
    {
        throw new NotImplementedException();
    }

}
