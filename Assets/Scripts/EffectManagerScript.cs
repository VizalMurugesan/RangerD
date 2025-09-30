
using System.Collections.Generic;
using UnityEngine;

public class EffectManagerScript : MonoBehaviour
{
    public List<GameObject> PoisonEffect;
    Queue<GameObject> PoisonEffectQueue;

    public void Start()
    {
        PoisonEffectQueue = new Queue<GameObject>();
    }
    public void EnablePoisonEffect(Vector2 SpawnPos)
    {
        foreach (var p in PoisonEffect)
        {
            if (!p.activeInHierarchy)
            {
                p.SetActive(true);
                p.transform.position = SpawnPos;
                p.GetComponent<Animator>().SetTrigger("explode");
                PoisonEffectQueue.Enqueue(p);
                Game.Instance.timeManager.DoAnActionAfterTime(DisablePoisonArrow,
                    Game.Instance.timeManager.TotalTime + 7f);
                break;
            }
        }
    }

    public void DisablePoisonArrow() 
    {
        GameObject effect = PoisonEffectQueue.Dequeue();
        effect.SetActive(false);
    }

    public void SetLayer( GameObject effect)
    {
        if (Game.Instance.player.PlayerLayer.Equals(Game.Layers.Layer1))
        {
            effect.GetComponent<SpriteRenderer>().sortingLayerName = Game.SortingLayers.path.ToString();
        }
        else if (Game.Instance.player.PlayerLayer.Equals(Game.Layers.Layer1))
        {
            effect.GetComponent<SpriteRenderer>().sortingLayerName = Game.SortingLayers.Layer2ground.ToString();
        }

        
    }
    
}
