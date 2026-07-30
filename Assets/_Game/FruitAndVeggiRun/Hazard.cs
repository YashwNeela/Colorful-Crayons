using UnityEngine;

/// <summary>Water, reeds, anything that ends the flight.</summary>
public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        RocketPlayer p = other.GetComponentInParent<RocketPlayer>();
        if (p != null && p.Alive) p.Crash();
    }
}
