using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Velocity;
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
        
        Vector3 direction = TargetPos - transform.position;
        body.linearVelocity = direction.normalized * Velocity;
        
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
        if(collision.gameObject.name != "Player")
        {
            Debug.Log(collision.gameObject.name + "enabled the blast");
            gameObject.SetActive(false);
        }
        

    }

}
