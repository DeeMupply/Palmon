using UnityEngine;
using UnityEngine.AI;

public class FlameOx_OnAttack_ChargeController : Palmon_OnEvent_Behaviour
{
    [SerializeField] private float overshootDistance = 3f; // How far past target to run
    [SerializeField] private float reachedThreshold = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private Palmon_OnEvent_Behaviour turnOffHitboxBehaviour;
    
    private Vector3 chargeTargetPosition;
    private bool isCharging = false;
    
    public override void OnEventBehave()
    {
        animator.Play("Armature_Run");
        StartCharging();
    }
    
    private void StartCharging()
    {
        isCharging = true;
        
        // Calculate overshoot position
        Vector3 palmonPos = palmon.transform.position;
        Vector2 targetPos2D = palmon.GetAttackPosition();
        Vector3 targetPos = new Vector3(targetPos2D.x, palmonPos.y, targetPos2D.y);
        
        // Direction from palmon to target
        Vector3 chargeDirection = (targetPos - palmonPos).normalized;
        
        // Target position with overshoot
        chargeTargetPosition = targetPos + chargeDirection * overshootDistance;
        
        // Set navmesh to charge to overshoot position
        NavMeshAgent agent = palmon.GetNavMeshAgent();
        palmon.PalmonSpecialRunAttack();
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = palmon.GetRunSpeed(); // Use run speed for charge
            agent.SetDestination(chargeTargetPosition);
        }
        
        Debug.Log($"Buffalo starting charge to {chargeTargetPosition}");
    }
    
    private void Update()
    {
        if (!isCharging) return;
        if (palmon.GetCurrentState() != PalmonState.Attacking)
        {
            isCharging = false;
            return;
        }
        
        Vector3 palmonPos = palmon.transform.position;
        float distanceToChargeTarget = Vector3.Distance(palmonPos, chargeTargetPosition);
        
        // Stop if reached charge target or time exceeded
        if (distanceToChargeTarget <= reachedThreshold)
        {
            StopCharging();
        }
    }
    
    private void StopCharging()
    {
        isCharging = false;
        palmon.UpdateHasJustAttackedAfterAttacking();
        turnOffHitboxBehaviour.OnEventBehave();
        
        // Stop movement
        NavMeshAgent agent = palmon.GetNavMeshAgent();
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
        
        Debug.Log("Buffalo charge completed");
    }
}