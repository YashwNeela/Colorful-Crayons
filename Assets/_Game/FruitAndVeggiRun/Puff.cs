using UnityEngine;

/// <summary>Quick expanding blob used for collects and crashes.</summary>
public class Puff : MonoBehaviour
{
    [SerializeField] private float life = 0.45f;
    [SerializeField] private float startScale = 0.25f;
    [SerializeField] private float endScale = 1.9f;

    private SpriteRenderer sr;
    private float t;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play(Color c, float sizeMultiplier)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = c;
        startScale *= sizeMultiplier;
        endScale *= sizeMultiplier;
    }

    private void Update()
    {
        t += Time.deltaTime / life;
        float e = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f); // ease out
        float s = Mathf.Lerp(startScale, endScale, e);
        transform.localScale = new Vector3(s, s, 1f);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, e);
            sr.color = c;
        }

        if (t >= 1f) Destroy(gameObject);
    }
}
