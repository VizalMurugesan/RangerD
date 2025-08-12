using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public List<GameObject> Arrows;

    public Transform SpawnPoint;
    

    public void SpawnArrow(Quaternion mainhandRotation)
    {
        
        foreach (GameObject arrow in Arrows)
        {
            if (!arrow.activeInHierarchy)
            {
                Debug.Log("spawn done");
                arrow.GetComponent<Arrow>().EnableArrow(SpawnPoint, mainhandRotation);
                break;
            }
        }
    }
}
