using UnityEngine;

public class Palmon_OnAttackAnimation_ResetState : Palmon_OnEvent_Behaviour
{
    public override void OnEventBehave()
    {
        palmon.UpdateHasJustAttackedAfterAttacking();
    }
}