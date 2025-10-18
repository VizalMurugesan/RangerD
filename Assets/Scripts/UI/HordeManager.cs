using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HordeManager : MonoBehaviour
{
    public List<Enemy> enemies;
    public List<Horde> Hordes;

    public float CohesiveStrength;
    public float SeperationStrength;
    public float SeperationRadius;


    float totalTime = 0f;

    float cooldownTimer = 0f;

    public float Interval;

    void Start()
    {
        Hordes = new List<Horde>();
    }

    public void FixedUpdate()
    {
        foreach (Horde horde in Hordes)
        {
            horde.RemoveInActiveEnemies();
        }

        if (cooldownTimer> Interval)
        {
            if(Hordes.Count==0) { SpawnHorde(); }
            else
            {
                foreach(Horde horde in Hordes)
                {
                    horde.GetLeader().ChasePlayer();
                }
            }
            cooldownTimer = 0f;
        }

        foreach (Horde horde in Hordes)
        {
            Dictionary<GameObject, Vector3> flockDir = horde.GetFlockToPosOfEntireHorde();
            Debug.Log("Setting Flocking direction for " + Hordes.IndexOf(horde));
            foreach (var pair in flockDir)
            {
                pair.Key.GetComponent<Rigidbody2D>().linearVelocity = pair.Value;
            }

        }

        totalTime += Time.fixedDeltaTime;
        cooldownTimer += Time.fixedDeltaTime;
    }

    public void SpawnHorde()
    {
        Horde horde = new Horde(enemies, CohesiveStrength, SeperationStrength, SeperationRadius);
        Hordes.Add(horde);
        horde.SetHordeActive((Vector2)Game.Instance.player.transform.position + new Vector2(-6f, -6f));
    }

    
}
