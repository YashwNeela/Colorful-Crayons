using UnityEngine;

[ExecuteAlways] // Allows the script to run in Edit Mode
public class SpriteFillEditor : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    [Header("Settings")]
    public float maxWidth = 5f;

    [Range(0, 1)]
    public float fillAmount = 1.0f;

    // Runs whenever you change values in the Inspector
    private void OnValidate()
    {
        UpdateSpriteSize();
    }

    private void Update()
    {
        // Only necessary if you want it to update constantly (e.g., during animations)
        // while in Edit Mode. OnValidate is usually enough for manual slider changes.
        if (!Application.isPlaying)
        {
            UpdateSpriteSize();
        }
    }

    private void UpdateSpriteSize()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.drawMode == SpriteDrawMode.Tiled)
        {
            // Update the width based on the fill percentage
            spriteRenderer.size = new Vector2(maxWidth * fillAmount, spriteRenderer.size.y);
        }
    }
}