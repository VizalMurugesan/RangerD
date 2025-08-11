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
    
    
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void EnableArrow(Transform spawnPoint)
    {
        transform.position = spawnPoint.position;
        DisplayArrow();
        Vector3 TargetPos = Game.Instance.GetCursorPosition();
        
        Vector2 direction = (TargetPos - transform.position);
        Debug.Log("before:"+direction);
        if (direction.magnitude>1f)
            direction = direction.normalized;
        else { direction = Game.Instance.UpScaleNormalize(direction); }
        Debug.Log("after"+direction);
        body.linearVelocity = direction * Velocity;
        
    }

    public void DisplayArrow()
    {
        
        gameObject.SetActive(true);
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        transform.rotation = Game.Instance.player.MainHand.transform.rotation * Quaternion.Euler(0f,0f, 90f);
        sprite.sortingOrder = Game.Instance.player.MainHand.GetComponent<SpriteRenderer>().sortingOrder;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CollisionCheck(collision.gameObject))
        {
            Debug.Log(collision.gameObject.name + "enabled the blast");
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
