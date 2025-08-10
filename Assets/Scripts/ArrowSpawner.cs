using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public List<GameObject> Arrows;

    public Transform SpawnPoint;
    

    public void SpawnArrow()
    {
        
        foreach (GameObject arrow in Arrows)
        {
            if (!arrow.activeInHierarchy)
            {
                
                arrow.GetComponent<Arrow>().EnableArrow(SpawnPoint);
                break;
            }
        }
    }
}
