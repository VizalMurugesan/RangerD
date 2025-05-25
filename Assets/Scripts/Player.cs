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
        float VelocityX = Input.GetAxis("Horizontal") * MovementSpeed;
        float VelocityY = Input.GetAxis("Vertical") * MovementSpeed;

        rb.linearVelocity = new Vector2 (VelocityX, VelocityY);

        
    }
}
