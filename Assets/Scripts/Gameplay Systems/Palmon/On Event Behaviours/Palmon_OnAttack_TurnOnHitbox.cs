using UnityEngine;

public class Palmon_OnAttack_TurnOnHitbox : Palmon_OnEvent_Behaviour
{
    [SerializeField] private GameObject hitbox;
    private void Start()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }
    
    public override void OnEventBehave()
    {
        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }
    }
}