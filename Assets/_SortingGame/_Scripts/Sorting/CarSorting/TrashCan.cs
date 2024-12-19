using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace TMKOC.Sorting.CarSorting{
public class TrashCan : MonoBehaviour
{
     /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<Collectible>(out Collectible collectible))
        {
            collectible.gameObject.SetActive(false);
            
        }
    }
}
}
