using System.Collections;
using UnityEngine;

public class Naga : Enemy
{
    public float AnticipationDuration;

    [Header("Camera Shake Variables")]
    public float ShakeDuration;
    public float Mag;
    public GameObject MainHand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
    }

    public override IEnumerator Attack()
    {
        
        TurnToPlayer();
        state = EnemyStateEnum.Attacking;
        if (anim!= null) { anim.SetTrigger("SwingAtk"); }
        yield return new WaitForSeconds(AttackCooldown);
        
        state = EnemyStateEnum.None;
    }

    public void attack()
    {
        Debug.Log("anticipation phase finished");
        if (Vector2.Distance(transform.position, Game.Instance.player.GetPlayerPosition()) < 3f)
        {
            Game.Instance.player.playerHealth.TakeDamage(damage);
            StartCoroutine(Game.Instance.ChangePlayerColorToAttacked());
            StartCoroutine(Game.Instance.mainCamera.Shake(ShakeDuration, Mag));
        }

    }

    public override void SetSortingOrder(int order, bool IsCharacterBelow)
    {
        spriteRenderer.sortingOrder = order;
        if(IsCharacterBelow)
        {
            MainHand.GetComponent<SpriteRenderer>().sortingOrder = order - 1;
        }
        else
        {
            MainHand.GetComponent<SpriteRenderer>().sortingOrder = order + 1;
        }
        

    }


}
