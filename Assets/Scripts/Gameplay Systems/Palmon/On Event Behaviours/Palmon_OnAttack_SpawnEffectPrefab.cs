using UnityEngine;

public class Palmon_OnAttack_SpawnEffectPrefab : Palmon_OnEvent_Behaviour
{
    [SerializeField] private GameObject effectPrefab;
    private void Start()
    {

    }
    
    public override void OnEventBehave()
    {
        if (effectPrefab != null)
        {
            GameObject effectInstance = Instantiate(effectPrefab, transform);
        }
    }
}