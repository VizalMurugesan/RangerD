using System;
using System.Collections;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [NonSerialized] public GameObject Player;
    [NonSerialized] public SpriteRenderer Rendr;
    public float YOffset = 0f;
    [NonSerialized] public string BackLayer;
    [NonSerialized] public string FrontLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        Rendr = GetComponent<SpriteRenderer>();
        if(Game.Instance.player != null)
        {
            Player = Game.Instance.player.gameObject;
        }
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(LayerCheck(collision.gameObject));
    }

    public virtual void BringFront()
    {
        Rendr.sortingLayerName = FrontLayer;
    }

    public virtual void BringBack()
    {
        Rendr.sortingLayerName = BackLayer;
    }

    public IEnumerator LayerCheck(GameObject objecthit)
    {
        while(Vector2.Distance(objecthit.transform.position, transform.position)<1.5f)
        {
            if (transform.position.y >= objecthit.transform.position.y - YOffset)
            {
                BringBack();
            }
            else
            {
                BringFront();
            }
            yield return null;
        }
        yield break;
    }
}
