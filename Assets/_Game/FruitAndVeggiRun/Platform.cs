using UnityEngine;

/// <summary>
/// A floating grass ledge. One-way: the player lands on the top face but
/// passes straight up through it from underneath.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour
{
    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    /// <summary>World Y of the walkable surface.</summary>
    public float TopY
    {
        get
        {
            if (box == null) box = GetComponent<BoxCollider2D>();
            return transform.position.y + box.offset.y + box.size.y * 0.5f;
        }
    }

    public float LeftX
    {
        get
        {
            if (box == null) box = GetComponent<BoxCollider2D>();
            return transform.position.x + box.offset.x - box.size.x * 0.5f;
        }
    }

    public float RightX
    {
        get
        {
            if (box == null) box = GetComponent<BoxCollider2D>();
            return transform.position.x + box.offset.x + box.size.x * 0.5f;
        }
    }
}
