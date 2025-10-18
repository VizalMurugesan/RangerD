using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().ApplyPoisonedEffect();
            
        }
        else
        {
            Debug.Log("enemy script is null");
        }
    }
}
