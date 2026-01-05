public class AllPalsAnimation : PalmonAnimation
{
    public override void OnIdle()
    {
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Idle]);
    }

    public override void OnMoving()
    {
        animator.speed = 1f;
        animator.Play(palmonASM[PalmonAnimationKeys.Run]);
    }

    public override void OnRunning()
    {
        animator.speed = 1.0f;
        animator.Play("Armature_Run");
    }

    public override void OnAttacking()
    {
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Attack]);
    }

    public override void OnRotateWithoutMoving()
    {
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Run]);
    }

    public override void OnEating()
    {
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Eat]);
    }
}