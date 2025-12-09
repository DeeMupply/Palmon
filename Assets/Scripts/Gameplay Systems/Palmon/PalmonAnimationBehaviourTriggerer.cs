using System.Collections.Generic;
using UnityEngine;

public class PalmonAnimationBehaviourTriggerer : StateMachineBehaviour
{
    private List<Palmon_OnEvent_Behaviour> receivers;
    private int lastLoop = -1;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (receivers == null)
        {
            var palmon = animator.GetComponent<Palmon>();
            if (palmon != null)
            {
                receivers = palmon.GetPalmonEventBehaviours(palmon.GetCurrentState());
            }
        }
        lastLoop = -1; // reset for new entry
        Debug.Log($"Entered state {animator.GetCurrentAnimatorStateInfo(0).shortNameHash}, found {receivers?.Count ?? 0} receivers");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int currentLoop = Mathf.FloorToInt(stateInfo.normalizedTime);

        // Reset events if we looped
        if (currentLoop != lastLoop)
        {
            foreach (var behaviour in receivers)
                behaviour.hasTriggered = false;

            lastLoop = currentLoop;
        }

        // Check each event
        float t = stateInfo.normalizedTime % 1f;
        foreach (var behaviour in receivers)
        {
            if (!behaviour.hasTriggered && t >= behaviour.A2BProfile.NormalizedTime)
            {
                behaviour.OnEventBehave();
                Debug.Log($"Triggered behaviour {behaviour.GetType().Name} at normalized time {t} (profile time {behaviour.A2BProfile.NormalizedTime})");
                behaviour.hasTriggered = true;
            }
        }
    }
}