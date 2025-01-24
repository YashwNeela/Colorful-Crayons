using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    void Start()
    {
        oldPosition = transform.position.x;
    }

 [SerializeField] private float lerpSpeed = 5f; // Speed of interpolation

private void Update()
{
    if (Mathf.Abs(transform.position.x - oldPosition) > 0.01f) // Small threshold to avoid minor jitter
    {
        // Calculate the delta
        float targetDelta = oldPosition - transform.position.x;

        // Interpolate the delta smoothly
        float smoothedDelta = Mathf.Lerp(0, targetDelta, Time.deltaTime * lerpSpeed);

        // Invoke the callback with the smoothed delta
        onCameraTranslate?.Invoke(smoothedDelta);

        // Update oldPosition using Lerp for smooth transition
        oldPosition = Mathf.Lerp(oldPosition, transform.position.x, Time.deltaTime * lerpSpeed);
    }
}
}