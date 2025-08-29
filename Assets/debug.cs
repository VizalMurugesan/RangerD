using UnityEngine;

public class debug : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Physics2D.OverlapBox(transform.position, new Vector2(1f, 1f), 0f) != null)
        {
            Debug.Log("overlapping");
        }
    }

    
}
