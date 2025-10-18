
using System.Collections.Generic;
using UnityEngine;

public class Horde
{
    public List<Enemy> enemies {  get; private set; }
    List<Enemy> NonLeaderEnemies;
    Player target;


    float HordePressureValue;

    float CohesionStrength; //this is a variable that measures how much should the enemy move towards leader.
    float SeparationStrength; //this is a variable that measures how much should the enemy stay away from leader.
    float SeparationRadius; //this is a variable that measures how wide should the enemy stay away from leader. fOR BIGGER ENEMIES.
    float CohesiveRadius = 3f;
    //public float InitialHordePressure { get; private set; }
    public Enemy Leader {  get; private set; }
    Enemy.EnemyStateEnum leaderState;

    public Horde(List<Enemy> enemies, float CohesionStrength, float SeparationStrength, float SeparationRadius)
    {
        this.enemies = enemies;
        NonLeaderEnemies = new List<Enemy>(enemies);
        this.CohesionStrength = CohesionStrength;
        this.SeparationStrength = SeparationStrength;
        this.SeparationRadius = SeparationRadius;
        if (enemies != null) { Leader = enemies[0]; NonLeaderEnemies.RemoveAt(0); }
        ChangeLeaderColorToRed();
        UpdateHordePressureValue();
    }

    public Enemy GetLeader()
    {
        return Leader;
    }
    public void SetLeader(Enemy enemy)
    {
        Leader = enemy;
        enemy.body.linearVelocity = Vector3.zero;
        NonLeaderEnemies.Remove(enemy);
    }

    public float GetHordePressureValue()
    {
        return HordePressureValue;
    }

    public void SetHordePressureValue(float Val)
    {
        HordePressureValue = Val;
    }

    public void RemoveInActiveEnemies()
    {
        enemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
        if (Leader != null &&!Leader.gameObject.activeInHierarchy && enemies.Count>0)
        {
            SetLeader(enemies[0]);
            //Debug.Log(enemies[0].body.linearVelocity);
            ChangeLeaderColorToRed();
            UpdateHordePressureValue();
        }
    }

    public void SetHordeActive(Vector2 SpawnPoint)
    {
        if(Leader == null) {  return; }
        Leader.transform.position = SpawnPoint;
        Leader.gameObject.SetActive(true);
        Game.Instance.characterLayerManager.CharactersInRange.Add(Leader);

        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
            if (enemy.health != null) { enemy.health.setHpToMax(); }
            

            Game.Instance.characterLayerManager.CharactersInRange.Add(enemy);

            if (enemy.gameObject == Leader.gameObject) { continue; }

            enemy.gameObject.SetActive(true);

            Vector2 offset = Random.insideUnitCircle * 2f;

            enemy.transform.position = SpawnPoint + offset;

            
        }
    }

    public void SetLeaderAgrro()
    {
        Leader.SetAggroTrue();
        Leader.DecideState();
        leaderState = Leader.state;
        
    }

    public Enemy.EnemyStateEnum GetLeaderState()
    {
        return leaderState;
    }

    public Dictionary<GameObject, Vector3> GetFlockToPosOfEntireHorde()
    {
        Dictionary<GameObject, Vector3> FlockToPos = new Dictionary<GameObject, Vector3>();

        foreach(Enemy enemy in NonLeaderEnemies)
        {
            FlockToPos.Add(enemy.gameObject, ComputeFlockingDirection(enemy));
            //Debug.Log("Returning Flocking direction");
        }

        return FlockToPos;
    }
    
    Vector3 ComputeFlockingDirection(Enemy enemy)
    {
        Vector3 CohesionDirection = (Leader.transform.position - enemy.transform.position).normalized;
        Vector3 SeperationDirection = Vector3.zero;
        //Debug.Log("Computing Flocking direction");
        foreach(Enemy other in enemies)
        {
            if (enemy.gameObject.Equals(other.gameObject)) { continue; }

            float distance = Vector2.Distance(enemy.transform.position, other.transform.position);

            if ( distance < SeparationRadius)
            {
                SeperationDirection += (enemy.transform.position - other.transform.position).normalized/distance;
            }
        }
        float CohesiveCloseness = Vector2.Distance(Leader.transform.position, enemy.transform.position);

        float CohesionStrengthMultiplier = Mathf.Min(1f, CohesiveCloseness/CohesiveRadius);

        return SeperationDirection * SeparationStrength + (CohesionDirection * CohesionStrength * CohesionStrengthMultiplier);
    }

    void ChangeLeaderColorToRed()
    {
        List<SpriteRenderer> renderers = Leader.GetComponent<Character>().renderers;
        foreach (var rend in renderers)
        {
            rend.color = new Vector4(1f, 0f, 0f, 0.7f);
        }
    }

    void UpdateHordePressureValue()
    {
        float val = 0f;
        foreach(Enemy enemy in enemies)
        {
            val += enemy.PressureValue;
        }
        HordePressureValue = val;
    }

    public void MergeHorde(Horde other)
    {
        if (other == null || other == this) return;
        enemies.AddRange(other.enemies);
        NonLeaderEnemies.AddRange(other.enemies);

    }
}
