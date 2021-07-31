using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsMenager : MonoBehaviour
{
    public LinkedList<EntityBase> Units = new LinkedList<EntityBase>();
    public Hero HeroA;
    public Hero HeroB;

    public Hero GetEnemy(Team urTeam)
    {
        switch (urTeam)
        {
            case Team.TeamA:
                return HeroB;
            case Team.TeamB:
                return HeroA;
            default:
                throw new ArgumentOutOfRangeException(nameof(urTeam), urTeam, null);
        }
    }
    public Hero GetYourBoi(Team urTeam)
    {
        switch (urTeam)
        {
            case Team.TeamA:
                return HeroA;
            case Team.TeamB:
                return HeroB;
            default:
                throw new ArgumentOutOfRangeException(nameof(urTeam), urTeam, null);
        }
    }

    public GameObject HeroPref;

    public GameObject TeamAHeroPref;
    public GameObject TeamBHeroPref;

    public GameObject MinionPref;

    public Transform MinionSpawnA;
    public Transform MinionSpawnB;

    public Transform HeroSpawnA;
    public Transform HeroSpawnB;

    public float MinionWaveSpawnInterval;
    public float SpawnInrervalWithinTheWave;
    public int WaveSize;

    private float _waveSpawnTimer;

    EntityBase SpawnUnit(GameObject Base, Team team)
    {
        var newMinion = Instantiate(Base, transform);
        var minion = newMinion.GetComponent<EntityBase>();

        minion.team = team;
        minion.Major = this;
        minion.ResetMaxStatsCalc();
        minion.OnBirth();

        return minion;
    }

    void SpawnHeroA()
    {
        var hero = SpawnUnit(HeroPref, Team.TeamA);

        Hero heroComponent = hero.gameObject.GetComponent<Hero>();

        HeroA = heroComponent;
    }

    void SpawnHeroB()
    {
        var hero = SpawnUnit(HeroPref, Team.TeamB);

        Hero heroComponent = hero.gameObject.GetComponent<Hero>();

        HeroB = heroComponent;
    }

    void SpawnMinion(Team team)
    {
        var minion = (Minion)SpawnUnit(MinionPref, team);

        switch (team)
        {
            case Team.TeamA:
                minion.MinionPrimaryDestination = MinionSpawnB;
                minion.transform.position = MinionSpawnA.position;
                break;
            case Team.TeamB:
                minion.MinionPrimaryDestination = MinionSpawnA;
                minion.transform.position = MinionSpawnB.position;
                break;
        }

        Units.AddLast(minion);
        
    }

    public EntityBase GetClosest(Vector3 pos)
    {
        return Units.Select(entity => (Vector3.SqrMagnitude(entity.transform.position - pos), entity)).Min().Item2;
    }

    public List<EntityBase> CollectClosests(Vector3 pos, float min_dis, int limit)
    {
        var list = new List<EntityBase>();

        var dissqrt = min_dis * min_dis;

        foreach(var entity in Units)
        {
            if(Vector3.SqrMagnitude(entity.transform.position - pos) < dissqrt)
            {
                list.Add(entity);
            }
        }

        //Sort by distance to pos
        list.Sort(delegate (EntityBase a, EntityBase b)
        {
            if (a is null || b is null) return 0;

            if(Vector3.SqrMagnitude(a.transform.position - pos) > Vector3.SqrMagnitude(b.transform.position - pos))
            {
                return 1;
            }
            else
            {
                return -1;
            }

        });
        
        return list.Take(limit).ToList();
    }
    private void Start()
    {
        _waveSpawnTimer = MinionWaveSpawnInterval-1.0f;
    }

    // Update is called once per frame
    private new void Update()
    {
        _waveSpawnTimer += Time.deltaTime;

        if (_waveSpawnTimer > MinionWaveSpawnInterval)
        {
            _waveSpawnTimer = 0;
            StartCoroutine(WaveSpawner(Team.TeamA));
            StartCoroutine(WaveSpawner(Team.TeamB));
        }

        if (HeroA == null)
        {
            SpawnHeroA();
        }
    }

    public void RemoveUnit(EntityBase entity)
    {
        Units.Remove(entity);
        Destroy(entity.gameObject);
    }

    IEnumerator WaveSpawner(Team team)
    {
        for (var i = 0; i < WaveSize; i++)
        {
            SpawnMinion(team);
            yield return new WaitForSeconds(SpawnInrervalWithinTheWave);
        }
    }
}
