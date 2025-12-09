using UnityEngine;

public abstract class PalmonAnimation : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AnimationStateMapperSO palmonASM;
    public void OnPalmonStateChange(PalmonState newState, Palmon palmon)
    {
        switch (newState)
        {
            case PalmonState.Idle:
                OnIdle();
                break;
            case PalmonState.Moving:
                OnMoving();
                break;
            case PalmonState.Attacking:
                OnAttacking();
                break;
            case PalmonState.RotateWithoutMoving:
                OnRotateWithoutMoving();
                break;
            case PalmonState.Hit:
                OnHit();
                break;
            case PalmonState.Dying:
                OnDying();
                break;
        }
    }
    public abstract void OnIdle();
    public abstract void OnMoving();
    public abstract void OnAttacking();
    public abstract void OnRotateWithoutMoving();
    public abstract void OnHit();
    public abstract void OnDying();

    public Animator GetAnimator()
    {
        return animator;
    }

}