using UnityEngine;

public class FlameOx_OnAttack_Run : Palmon_OnEvent_Behaviour
{
    [SerializeField] private Animator animator;
    
    public override void OnEventBehave()
    {
        animator.Play("Armature_Run");
    }
}