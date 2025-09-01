using UnityEngine;

public class HealthPotion : Item
{
    public float Amount;
    public override void OnUse()
    {
        base.OnUse();
        Game.Instance.player.playerHealth.AddHealth(Amount);
    }
}
