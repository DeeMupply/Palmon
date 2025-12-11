using UnityEngine;

public class Dock : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Dock Trigger Entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            Player.Instance.SetIsAtDock(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.SetIsAtDock(false);
        }
    }
}