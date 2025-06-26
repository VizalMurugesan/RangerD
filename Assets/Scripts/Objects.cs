using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [NonSerialized] public GameObject Player;
    [NonSerialized] public SpriteRenderer Rendr;
    public List<SpriteRenderer> RendrList;
    public float YOffset = 0f;
    public float size = 1f;
    [NonSerialized] public string BackLayer;
    [NonSerialized] public string FrontLayer;
    public Coroutine layerCheckCoroutine;
    //public Sprite instance;
    //[NonSerialized] Sprite defaultSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if(gameObject.GetComponent<SpriteRenderer>() != null)
        {
            Rendr = GetComponent<SpriteRenderer>();
        }

        else
        {
            int childCount = gameObject.transform.childCount;
            for(int i = 0; i < childCount; i++)
            {
                RendrList.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
            }
        }

        if(Game.Instance.player != null)
        {
            Player = Game.Instance.player.gameObject;
        }
        //defaultSprite = Rendr.sprite;
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("triggered" + IsCoroutinenull(layerCheckCoroutine));
        if (layerCheckCoroutine == null && collision.CompareTag("Character"))
        {
            layerCheckCoroutine = StartCoroutine(LayerCheck(collision.gameObject));
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (layerCheckCoroutine != null)
        {
            //Rendr.sprite = defaultSprite;
            //Debug.Log("layercheck ended");
            StopCoroutine(layerCheckCoroutine);
            //Debug.Log(LayerCheck(collision.gameObject));
        }

        layerCheckCoroutine = null;
    }

    public virtual void BringFront()
    {
        if(Rendr!= null)
        {
            Rendr.sortingLayerName = FrontLayer;
        }

        else
        {
            foreach( SpriteRenderer rend in RendrList)
            {
                rend.sortingLayerName= FrontLayer;
            }
        }
        
    }

    public virtual void BringBack()
    {
        if (Rendr != null)
        {
            Rendr.sortingLayerName = BackLayer;
        }
        else
        {
            foreach (SpriteRenderer rend in RendrList)
            {
                rend.sortingLayerName = BackLayer;
            }
        }

    }

    public IEnumerator LayerCheck(GameObject objecthit)
    {
        Debug.Log("layercheck started");
        while(Vector2.Distance(objecthit.transform.position, transform.position)< size)
        {
            if (transform.position.y >= objecthit.transform.position.y - YOffset && Mathf.Abs(transform.position.x - objecthit.transform.position.x)<1f)
            {
                BringBack();
            }
            else
            {
                BringFront();
            }
            yield return null;
            Debug.Log("layerchecking");
        }
        //Rendr.sprite = defaultSprite;
        //Debug.Log("layercheck ended");
        layerCheckCoroutine = null;
        yield break;
        
    }

    bool IsCoroutinenull(Coroutine coroutine)
    {
        return (coroutine == null);
    }

}
