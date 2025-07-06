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

    public BoxCollider2D ActualCollider;
    public GameObject ObjectWithCollider;


    public float YOffset = 0f;
    public float size = 1f;

    public enum Layers { BaseGrass, BaseGround, path, VegetationBeforePlayer, StructuresBeforePlayer, VegetationOrstructures, Player, VegetationAfterPlayer, StructuresAfterPlayer, VegetationOrstructuresAfterPlayer }
    [SerializeField] public Layers BackLayer;
    [SerializeField] public Layers FrontLayer;


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
                if(transform.GetChild(i).GetComponent<SpriteRenderer>()!=null)
                    RendrList.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
            }
        }

        if(Game.Instance.player != null)
        {
            Player = Game.Instance.player.gameObject;
        }
        if(ObjectWithCollider != null)
        {
            ActualCollider = ObjectWithCollider.GetComponent<BoxCollider2D>();
            ActualCollider.enabled = false;
        }


        //defaultSprite = Rendr.sprite;
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("triggered" + IsCoroutinenull(layerCheckCoroutine));

        if(ActualCollider != null)
            ActualCollider.enabled = true;

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
            BringFront();

            if (ActualCollider != null)
                ActualCollider.enabled = false;

            //Debug.Log(LayerCheck(collision.gameObject));
        }

        layerCheckCoroutine = null;
    }

    public void BringFront()
    {
        if(Rendr!= null)
        {
            Rendr.sortingLayerName = FrontLayer.ToString();
        }

        else
        {
            foreach( SpriteRenderer rend in RendrList)
            {
                rend.sortingLayerName= FrontLayer.ToString();
            }
        }
        
    }

    public void BringBack()
    {
        if (Rendr != null)
        {
            Rendr.sortingLayerName = BackLayer.ToString();
        }
        else
        {
            foreach (SpriteRenderer rend in RendrList)
            {
                rend.sortingLayerName = BackLayer.ToString();
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
        BringFront();
        yield break;
        
    }

    bool IsCoroutinenull(Coroutine coroutine)
    {
        return (coroutine == null);
    }

}
