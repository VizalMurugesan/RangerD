using System.Collections;
using UnityEngine;

public class Naga : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
    }

    public override IEnumerator Attack()
    {
        Debug.Log("haha");
        state = EnemyStateEnum.Attacking;
        if (anim!= null) { anim.SetTrigger("SwingAtk"); }
        yield return new WaitForSeconds(AttackCooldown);
        state = EnemyStateEnum.None;
    }
}
