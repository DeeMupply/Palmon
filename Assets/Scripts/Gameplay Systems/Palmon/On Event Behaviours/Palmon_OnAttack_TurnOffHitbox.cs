using UnityEngine;

public class Palmon_OnAttack_TurnOffHitbox : Palmon_OnEvent_Behaviour
{
    [SerializeField] private GameObject hitbox;
    
    public override void OnEventBehave()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }
}