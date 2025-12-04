using UnityEngine;

public partial class Player
{
    // Animation-related properties and methods can be added here in the future
    [Header("Player Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationStateMapperSO playerASM;
    
    private bool isDying = false;
    private bool isHit = false;
    private bool isUsingScanTool = false;
    private bool isUsingTool = false;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isMoving = false;

    private void UpdateAnimation()
    {
        if (isDying)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.Die]);
            return;
        }

        if (isHit)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.Hit]);
            return;
        }

        if (isUsingScanTool)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.UseScanTool]);
            return;
        }

        if (isUsingTool)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.UseTool]);
            return;
        }

        if (isJumping)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.Jump]);
            return;
        }

        if (isSprinting)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.Sprint]);
            return;
        }

        if (isMoving)
        {
            // animator.Play(playerASM[PlayerAnimationKeys.Run]);
            return;
        }

        // animator.Play(playerASM[PlayerAnimationKeys.Idle]);
    }
}