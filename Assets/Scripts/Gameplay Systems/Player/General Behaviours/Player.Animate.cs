using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    // Animation-related properties and methods can be added here in the future
    [Header("Player Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationStateMapperSO playerASM;
    [SerializeField] private List<Player_OnEvent_Behaviour> playerEventBehaviours;
    
    private bool isDying = false;
    private bool isHit = false;
    // private bool isUsingScanTool = false;
    private bool isUsingTool = false;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isMoving = false;

    private void UpdateAnimation()
    {
        if (isDying)
        {
            animator.Play(playerASM[PlayerAnimationKeys.Die]);
            return;
        }

        if (isHit)
        {
            animator.speed = 1.0f;
            animator.Play(playerASM[PlayerAnimationKeys.Hit]);
            return;
        }

        if (isUsingTool)
        {
            animator.Play(playerASM[PlayerAnimationKeys.Interact]);
            return;
        }

        if (isJumping)
        {
            animator.Play(playerASM[PlayerAnimationKeys.Jump]);
            return;
        }

        if (isSprinting)
        {
            animator.Play(playerASM[PlayerAnimationKeys.Run]);
            return;
        }

        if (isMoving)
        {
            animator.Play(playerASM[PlayerAnimationKeys.Run]);
            return;
        }

        animator.Play(playerASM[PlayerAnimationKeys.Idle]);
    }

    public List<Player_OnEvent_Behaviour> GetPlayerEventBehaviours()
    {
        Debug.Log($"Getting {playerEventBehaviours.Count} player event behaviours");
        return playerEventBehaviours;
    }

    public void SetIsDyingToFalse()
    {
        isDying = false;
    }

    private void ResetAllAnimationFlags()
    {
        isDying = false;
        isHit = false;
        isUsingTool = false;
        isJumping = false;
        isSprinting = false;
        isMoving = false;
    }
}