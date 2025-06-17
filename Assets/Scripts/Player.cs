using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float MovementSpeed;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        float VelocityX = Input.GetAxis("Horizontal") ;
        float VelocityY = Input.GetAxis("Vertical") ;

        Vector2 movemnntInput = new Vector2 (VelocityX, VelocityY).normalized;

        rb.MovePosition ((Vector2)transform.position + movemnntInput * MovementSpeed * Time.deltaTime);

        
    }
}
