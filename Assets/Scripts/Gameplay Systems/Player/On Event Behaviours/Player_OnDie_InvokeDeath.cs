using UnityEngine;
public class Player_OnDie_InvokeDeath : Player_OnEvent_Behaviour
{
    [SerializeField] private Animator animator;
    public override void OnEventBehave()
    {
        player.SetIsDyingToFalse();
        animator.speed = 0f;
        player.InvokeDeathEvent();
    }
}