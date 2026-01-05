using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Bait : MonoBehaviour
{
    [SerializeField] private float lifetime = 60f; // Bait disappears after 60 seconds
    [SerializeField] private int palmonCapacity = 2; // How many Palmon can eat this bait at once
    [SerializeField] private TextMeshProUGUI lifetimeText;
    [SerializeField] private Color lowLifetimeColor = Color.red;
    [SerializeField] private float lowLifetimeThreshold = 10f; // seconds

    private float spawnTime;
    private HashSet<Palmon> currentlyEatingPalmon = new HashSet<Palmon>();

    private void Start()
    {
        spawnTime = Time.time;

        // Register with BaitManager
        BaitManager.Instance?.RegisterBait(this);
    }

    private void Update()
    {
        // Check if bait expired
        UpdateLifetimeText();
        if (Time.time - spawnTime >= lifetime)
        {
            DestroySelf();
        }
    }

    private void UpdateLifetimeText()
    {
        if (lifetimeText != null)
        {
            float timeLeft = lifetime - (Time.time - spawnTime);
            lifetimeText.text = Mathf.CeilToInt(timeLeft).ToString();

            // Change color if low on lifetime
            if (timeLeft <= lowLifetimeThreshold)
            {
                lifetimeText.color = lowLifetimeColor;
            }
        }
    }

    public bool CanEat()
    {
        // Remove any null references (destroyed Palmon)
        currentlyEatingPalmon.RemoveWhere(p => p == null);

        return currentlyEatingPalmon.Count < palmonCapacity;
    }

    public bool TryStartEating(Palmon palmon)
    {
        if (CanEat() && !currentlyEatingPalmon.Contains(palmon))
        {
            currentlyEatingPalmon.Add(palmon);
            Debug.Log($"{palmon.name} started eating bait. Current eaters: {currentlyEatingPalmon.Count}/{palmonCapacity}");
            return true;
        }

        return false;
    }

    public void StopEating(Palmon palmon)
    {
        if (currentlyEatingPalmon.Remove(palmon))
        {
            Debug.Log($"{palmon.name} stopped eating bait. Current eaters: {currentlyEatingPalmon.Count}/{palmonCapacity}");
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsAvailable()
    {
        // Remove any null references
        currentlyEatingPalmon.RemoveWhere(p => p == null);

        return currentlyEatingPalmon.Count < palmonCapacity;
    }

    public int GetCurrentEaters()
    {
        currentlyEatingPalmon.RemoveWhere(p => p == null);
        return currentlyEatingPalmon.Count;
    }

    private void DestroySelf()
    {
        // Notify all eating Palmon that bait is gone
        foreach (Palmon palmon in currentlyEatingPalmon)
        {
            if (palmon != null)
            {
                palmon.OnBaitDestroyed();
            }
        }

        BaitManager.Instance?.UnregisterBait(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        BaitManager.Instance?.UnregisterBait(this);
    }
}