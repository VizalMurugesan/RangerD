using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Arrow : MonoBehaviour
{
    public float damage = 50f;
    public float Velocity;
    //public float DistanceFactor;
    SpriteRenderer sprite;
    Rigidbody2D body;
    ArrowSpawner.ArrowType ArrowType = ArrowSpawner.ArrowType.normal;
    TrailRenderer trail;
    [SerializeField]float Range;
    Coroutine main;
    Vector2 SpawnPoint;



    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
    }

    public void EnableArrow(Transform spawnPoint, Quaternion mainhandRotation,  ArrowSpawner.ArrowType arrowType, Vector3 TargetPos)
    {
        
        transform.position = spawnPoint.position;
        SpawnPoint = spawnPoint.position;
        DisplayArrow(mainhandRotation);
        
        
        Vector2 direction = (TargetPos - transform.position);
        
        if (direction.magnitude>1f)
            direction = direction.normalized;
        else { direction = Game.Instance.UpScaleNormalize(direction); }
        
        body.linearVelocity = direction * Velocity;
        if (main != null)
        {
            StopCoroutine(main); 
        }
        main = StartCoroutine(Main());
        ArrowType = arrowType;
        
    }

    public void DisplayArrow(Quaternion mainhandRotation)
    {
        
        gameObject.SetActive(true);
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        transform.rotation =  mainhandRotation * Quaternion.Euler(0f,0f, 90f);
        sprite.sortingLayerName = Game.Instance.player.rendr.sortingLayerName;
        sprite.sortingOrder = Game.Instance.player.MainHand.GetComponent<SpriteRenderer>().sortingOrder;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CollisionCheck(collision.gameObject))
        {
            
            if (ArrowType.Equals(ArrowSpawner.ArrowType.ability2))
            {
                Game.Instance.EffectManager.EnablePoisonEffect((Vector2)transform.position);
            }
            
            if(collision.gameObject.CompareTag("Character"))
            {
                if(collision.gameObject.GetComponent<EnemyHealth>() != null)
                {
                    EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                    enemyHealth.TakeDamage(damage);
                }
            }
            
            DisableArrow();
        }

    }

    bool CollisionCheck(GameObject objectHit)
    {
        if(objectHit.name == "Player") { return false; }
        if(objectHit.CompareTag("Player")) { return false; }
        if (objectHit.CompareTag("Vegetation")) { return false; }
        if (objectHit.CompareTag("PlayerPivot")) { return false; }
        return true;
    }

    void DisableArrow()
    {
        trail.Clear();
        gameObject.SetActive(false);
    }

    public IEnumerator Main()
    {
        while (true)
        {
            if(Vector2.Distance(transform.position, SpawnPoint) >= Range)
            {
                DisableArrow();
                yield break;
                
            }
            Debug.Log("helly");
            yield return null;
        }
    }
}
