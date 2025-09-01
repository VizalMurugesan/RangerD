using UnityEngine;

public class EXPpotion : Item
{
    public float Amount;
    public override void OnUse()
    {
        base.OnUse();
        Game.Instance.levelManager.AddEXP(Amount);
    }
}
