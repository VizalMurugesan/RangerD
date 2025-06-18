using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float MovementSpeed;
    Vector2 movementInput = Vector2.zero;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    

    private void Update()
    {
        float VelocityX = Input.GetAxis("Horizontal");
        float VelocityY = Input.GetAxis("Vertical");

        movementInput = new Vector2(VelocityX, VelocityY).normalized;

    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementInput* MovementSpeed* Time.fixedDeltaTime);
    }
}
