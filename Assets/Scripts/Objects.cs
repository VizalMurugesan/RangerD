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

    [NonSerialized] public BoxCollider2D FrontCollider;
    [NonSerialized] public BoxCollider2D BackCollider;

    public GameObject ObjectWithFrontCollider;
    public GameObject ObjectWithBackCollider;
    public GameObject pivot;


    public float YOffset = 0f;
    public float size = 1f;

    
    [SerializeField] public Game.SortingLayers BackLayer;
    [SerializeField] public Game.SortingLayers FrontLayer;


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
                if(transform.GetChild(i).gameObject.name.Equals("pivot", StringComparison.OrdinalIgnoreCase))
                {
                    pivot = transform.GetChild(i).gameObject;
                }

            }
        }

        if(Game.Instance.player != null)
        {
            Player = Game.Instance.player.gameObject;
        }
        if(ObjectWithFrontCollider != null)
        {
            FrontCollider = ObjectWithFrontCollider.GetComponent<BoxCollider2D>();
            FrontCollider.enabled = false;
        }

        if (ObjectWithBackCollider != null)
        {
            BackCollider = ObjectWithBackCollider.GetComponent<BoxCollider2D>();
            BackCollider.enabled = false;
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
            BringFront();

           

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

        if(FrontCollider != null)
            FrontCollider.enabled = true;
        if(BackCollider != null)
            BackCollider.enabled = false;
        
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

        if (FrontCollider != null)
            FrontCollider.enabled = false;
        if (BackCollider != null)
            BackCollider.enabled = true;

    }

    public IEnumerator LayerCheck(GameObject objecthit)
    {
        Debug.Log("layercheck started");
        while(Vector2.Distance(objecthit.transform.position, transform.position)< size)
        {
            if (pivot.transform.position.y >= objecthit.transform.position.y - YOffset && Mathf.Abs(transform.position.x - objecthit.transform.position.x)<1f)
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
