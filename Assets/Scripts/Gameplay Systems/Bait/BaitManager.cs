using System.Collections.Generic;
using UnityEngine;

public class BaitManager : MonoBehaviour
{
    public static BaitManager Instance { get; private set; }
    
    private List<Bait> activeBaits = new List<Bait>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RegisterBait(Bait bait)
    {
        if (!activeBaits.Contains(bait))
        {
            activeBaits.Add(bait);
            Debug.Log($"Bait registered. Total active baits: {activeBaits.Count}");
        }
    }
    
    public void UnregisterBait(Bait bait)
    {
        activeBaits.Remove(bait);
        Debug.Log($"Bait unregistered. Total active baits: {activeBaits.Count}");
    }
    
    public Bait GetNearestBait(Vector3 position, float maxDistance)
    {
        Bait nearestBait = null;
        float nearestDistance = maxDistance;
        
        // Clean up null baits
        activeBaits.RemoveAll(b => b == null);
        
        foreach (Bait bait in activeBaits)
        {
            if (!bait.IsAvailable()) continue;
            
            float distance = Vector3.Distance(position, bait.GetPosition());
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestBait = bait;
            }
        }
        
        return nearestBait;
    }
    
    public List<Bait> GetAllActiveBaits()
    {
        activeBaits.RemoveAll(b => b == null);
        return new List<Bait>(activeBaits);
    }
    
    public int GetActiveBaitCount()
    {
        activeBaits.RemoveAll(b => b == null);
        return activeBaits.Count;
    }
    
    public Transform GetTransform()
    {
        return transform;
    }
}