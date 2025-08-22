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
        


        //defaultSprite = Rendr.sprite;
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        

        

        if (layerCheckCoroutine == null && collision.CompareTag("Character"))
        {
            layerCheckCoroutine = StartCoroutine(LayerCheck(collision.gameObject));
        }
       
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
        
        GameObject objecthitPivot = FindObjectHitPivot(objecthit);

        while(Vector2.Distance(objecthit.transform.position, transform.position)< size)
        {
            if (pivot.transform.position.y >= objecthitPivot.transform.position.y - YOffset )
            {
                BringBack();
            }
            else
            {
                BringFront();
            }
            yield return null;
            


        }
        
        layerCheckCoroutine = null;
        BringFront();
        yield break;
        
    }

    bool IsCoroutinenull(Coroutine coroutine)
    {
        return (coroutine == null);
    }

    GameObject FindObjectHitPivot(GameObject objecthit)
    {
        int ChildCount = objecthit.transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            if (objecthit.transform.GetChild(i).gameObject.name.Equals("pivot"))
                return objecthit.transform.GetChild(i).gameObject;

        }

        return objecthit;
    }

}
