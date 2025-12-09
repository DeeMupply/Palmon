using UnityEngine;

public abstract class Palmon_OnEvent_Behaviour : MonoBehaviour
{
    [SerializeField] protected Palmon palmon;
    public bool hasTriggered = false;
    public PalmonState AssociatedState;
    public A2BProfileSO A2BProfile;
    public abstract void OnEventBehave();
}