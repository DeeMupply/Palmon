public class AllPalsAnimation : PalmonAnimation
{
    public override void OnIdle()
    {
        animator.Play(palmonASM[PalmonAnimationKeys.Idle]);
    }

    public override void OnMoving()
    {
        animator.Play(palmonASM[PalmonAnimationKeys.Run]);
    }

    public override void OnAttacking()
    {
        animator.Play(palmonASM[PalmonAnimationKeys.Attack]);
    }

    public override void OnRotateWithoutMoving()
    {
        animator.Play(palmonASM[PalmonAnimationKeys.Idle]);
    }

    public override void OnHit()
    {
        //
    }

    public override void OnDying()
    {
        //
    }
}