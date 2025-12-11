using UnityEngine;

public abstract class Player_OnEvent_Behaviour : MonoBehaviour
{
    [SerializeField] protected Player player;
    public bool hasTriggered = false;
    public A2BProfileSO A2BProfile;
    public abstract void OnEventBehave();
}