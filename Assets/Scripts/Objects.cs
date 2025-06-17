using System;
using System.Collections;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [NonSerialized] public GameObject Player;
    [NonSerialized] public SpriteRenderer Rendr;
    public float YOffset = 0f;
    public float size = 1f;
    [NonSerialized] public string BackLayer;
    [NonSerialized] public string FrontLayer;
    public Coroutine layerCheckCoroutine;
    //public Sprite instance;
    [NonSerialized] Sprite defaultSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        Rendr = GetComponent<SpriteRenderer>();
        if(Game.Instance.player != null)
        {
            Player = Game.Instance.player.gameObject;
        }
        defaultSprite = Rendr.sprite;
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (layerCheckCoroutine == null && collision.CompareTag("Character"))
        layerCheckCoroutine = StartCoroutine(LayerCheck(collision.gameObject));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (layerCheckCoroutine != null)
        {
            Rendr.sprite = defaultSprite;
            StopCoroutine(layerCheckCoroutine);
            //Debug.Log(LayerCheck(collision.gameObject));
        }
            
        layerCheckCoroutine = null;
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
        //Rendr.sprite = instance;
        while(Vector2.Distance(objecthit.transform.position, transform.position)< size)
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
        //Rendr.sprite = defaultSprite;
        yield break;
    }
}
