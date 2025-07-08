using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float MovementSpeed;
    Vector2 movementInput = Vector2.zero;
    Rigidbody2D rb;

    public List<SpriteRenderer> RendrList;
    public SpriteRenderer rendr;

    public Game.Layers PlayerLayer = Game.Layers.Layer1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if(GetComponent<SpriteRenderer>() != null )
            rendr = GetComponent<SpriteRenderer>();

        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                RendrList.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
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

    public static string LayerToLayerName(Game.Layers layer)
    {
        return layer switch
        {
            Game.Layers.Layer1 => "Player",
            Game.Layers.Layer2 => "Layer2Player",
            _ => "Unknown"

        };
    }

    public void ChangePlayerLayer(Game.Layers layer)
    {
        string LayerName = LayerToLayerName(layer);

        if (rendr!= null)
        {
            rendr.sortingLayerName = LayerName;
        }
        else
        {
            foreach (var rendrer in RendrList)
            {
                rendrer.sortingLayerName = LayerName;
            }
        }

        
    }
}
