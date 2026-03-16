using UnityEngine;

public class OxAnimation : PalmonAnimation
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    public override void OnIdle()
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        PlayIdleSound();
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Idle]);
    }

    public void PlayIdleSound()
    {
        if (!audioSource.isPlaying)
        audioSource.Play();
    }

    public override void OnMoving()
    {
        // Stop trước khi play âm khác
        audioSource.Stop();

        animator.speed = 1f;
        animator.Play(palmonASM[PalmonAnimationKeys.Run]);
    }

    public override void OnRunning()
    {
        // Stop trước khi play âm khác
        audioSource.Stop();

        // Chạy
        animator.speed = 1.0f;
        animator.Play("Armature_Run");
    }

    public override void OnAttacking()
    {
        // Stop trước khi play âm khác
        audioSource.Stop();
        // Nếu có thì tiếng gầm gừ, báo hiệu tấn công
        // Tấn công
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Attack]);
    }

    public override void OnRotateWithoutMoving()
    {
        // Không cần âm thanh
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Run]);
    }

    public override void OnEating()
    {
        animator.speed = 1.0f;
        animator.Play(palmonASM[PalmonAnimationKeys.Eat]);
    }
}