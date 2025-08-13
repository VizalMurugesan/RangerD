using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Arrow : MonoBehaviour
{
    public float Velocity;
    //public float DistanceFactor;
    SpriteRenderer sprite;
    Rigidbody2D body;
    ArrowSpawner.ArrowType ArrowType = ArrowSpawner.ArrowType.normal;



    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void EnableArrow(Transform spawnPoint, Quaternion mainhandRotation,  ArrowSpawner.ArrowType arrowType)
    {
        Debug.Log(arrowType);
        transform.position = spawnPoint.position;
        DisplayArrow(mainhandRotation);
        Vector3 TargetPos = Game.Instance.GetCursorPosition();
        
        Vector2 direction = (TargetPos - transform.position);
        
        if (direction.magnitude>1f)
            direction = direction.normalized;
        else { direction = Game.Instance.UpScaleNormalize(direction); }
        
        body.linearVelocity = direction * Velocity;

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
            Debug.Log(collision.gameObject.name+" triggered the blast");
            if (ArrowType.Equals(ArrowSpawner.ArrowType.ability2))
            {
                Game.Instance.EffectManager.EnablePoisonEffect((Vector2)transform.position);
            }
            
            gameObject.SetActive(false);
        }

    }

    bool CollisionCheck(GameObject objectHit)
    {
        if(objectHit.name == "Player") { return false; }
        if (objectHit.CompareTag("Vegetation")) { return false; }
        return true;
    }
}
