using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public List<GameObject> Arrows;
    public enum ArrowType { normal, ability1, ability2};

    public Transform SpawnPoint;
    

    public void SpawnArrow(Quaternion mainhandRotation, ArrowType arrowType)
    {
        
        foreach (GameObject arrow in Arrows)
        {
            if (!arrow.activeInHierarchy)
            {
                arrow.GetComponent<Arrow>().EnableArrow(SpawnPoint, mainhandRotation, arrowType);
                break;
            }
        }
    }
}
