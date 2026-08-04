using UnityEngine;

/// <summary>A single piece of produce floating in the level.</summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SpriteRenderer glow;

    private bool taken;
    private float bobPhase;
    private float baseY;
    private bool isTarget;

    public string ItemName { get { return itemName; } }

    private void Start()
    {
        baseY = transform.position.y;
        bobPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    public void Setup(string name, Sprite sprite, bool target)
    {
        itemName = name;
        isTarget = target;
        if (icon != null) icon.sprite = sprite;
        if (glow != null)
        {
            glow.gameObject.SetActive(target);
            glow.color = new Color(1f, 1f, 1f, target ? 0.85f : 0f);
        }
    }

public void SetVisible(bool visible)
    {
        if (icon != null) icon.enabled = visible;
        if (glow != null) glow.enabled = visible && isTarget;
    }


    private void Update()
    {
        // gentle bob + the target item pulses so it reads as "collect me"
        float t = Time.time * 2f + bobPhase;
        Vector3 p = transform.position;
        p.y = baseY + Mathf.Sin(t) * 0.05f;
        transform.position = p;

        if (isTarget && glow != null)
        {
            float s = 1f + Mathf.Sin(t * 1.6f) * 0.12f;
            glow.transform.localScale = new Vector3(s, s, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (taken) return;
        RocketPlayer p = other.GetComponentInParent<RocketPlayer>();
        if (p == null || !p.Alive) return;

        taken = true;
        GameFlow flow = FindObjectOfType<GameFlow>();
        if (flow != null) flow.OnCollected(itemName, transform.position);
        Destroy(gameObject);
    }
}
