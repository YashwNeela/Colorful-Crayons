using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Streams the world in fixed-width segments ahead of the camera and recycles
/// them once they fall behind. Ground is either safe grass or deadly water.
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private float segmentWidth = 9f;
    [SerializeField] private float groundTopY = -3.6f;
    [SerializeField] private int segmentsAhead = 4;
    [SerializeField] private int firstHazardSegment = 3;
    [Tooltip("How high a pickup sits above the surface it rests on.")]
    [SerializeField] private float restHeight = 0.52f;

    [SerializeField] private float marketScale = 3f;

    [Header("Sprites")]
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite platformSprite;
    [SerializeField] private Sprite flowerSprite;
    [SerializeField] private Sprite cloudSprite;
    [SerializeField] private Sprite reedSprite;
    [SerializeField] private Sprite glowSprite;
    [SerializeField] private Sprite groundSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite skySprite;
    [SerializeField] private Sprite marketSprite;

    [Header("Background")]
    [Tooltip("0 = pinned to the camera, 1 = fixed in the world.")]
    [SerializeField] private float marketParallax = 0.55f;
    [SerializeField] private float marketBaseY = -1.75f;
    [SerializeField] private Sprite[] produceSprites; // matches GameDefs.Names order

    [Header("Refs")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameFlow flow;

    private readonly Dictionary<int, GameObject> segments = new Dictionary<int, GameObject>();
    private readonly HashSet<int> waterSegments = new HashSet<int>();
    private int nextSegment;

    private Transform skyLayer;
    private Transform marketLayer;

    // colours sampled from the bottom edge of the new ground / water art so the
    // fill below each band joins it seamlessly
    private static readonly Color DirtFill = new Color(0.541f, 0.224f, 0.055f);
    private static readonly Color WaterFill = new Color(0.659f, 0.902f, 0.957f);

    public float GroundTopY { get { return groundTopY; } }

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        if (flow == null) flow = FindObjectOfType<GameFlow>();
    }

    private void Start()
    {
        BuildBackground();

        for (int i = -1; i < segmentsAhead; i++) BuildSegment(i);
        nextSegment = segmentsAhead;
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        UpdateBackground();

        int camSeg = Mathf.FloorToInt(cameraTransform.position.x / segmentWidth);

        while (nextSegment <= camSeg + segmentsAhead)
        {
            BuildSegment(nextSegment);
            nextSegment++;
        }

        // recycle anything well behind the camera
        List<int> dead = null;
        foreach (KeyValuePair<int, GameObject> kv in segments)
        {
            if (kv.Key < camSeg - 2)
            {
                if (dead == null) dead = new List<int>();
                dead.Add(kv.Key);
            }
        }
        if (dead != null)
        {
            for (int i = 0; i < dead.Count; i++)
            {
                Destroy(segments[dead[i]]);
                segments.Remove(dead[i]);
                waterSegments.Remove(dead[i]);
            }
        }
    }

    /// <summary>True when there is no safe ground under this world X.</summary>
    public bool IsWaterAt(float worldX)
    {
        int seg = Mathf.FloorToInt(worldX / segmentWidth);
        return waterSegments.Contains(seg);
    }

    /// <summary>Nearest X to the left that has safe ground, for respawns.</summary>
    public float FindSafeX(float fromX)
    {
        int seg = Mathf.FloorToInt(fromX / segmentWidth);
        for (int i = seg; i >= seg - 6; i--)
        {
            if (!waterSegments.Contains(i)) return (i + 0.5f) * segmentWidth;
        }
        return (seg + 1.5f) * segmentWidth;
    }

    // ------------------------------------------------------------------

    private void BuildSegment(int index)
    {
        if (segments.ContainsKey(index)) return;

        GameObject root = new GameObject("Segment_" + index);
        root.transform.SetParent(transform);
        float x0 = index * segmentWidth;
        root.transform.position = new Vector3(x0, 0f, 0f);
        segments[index] = root;

        // deterministic per-segment randomness so a segment always looks the same
        Random.State prev = Random.state;
        Random.InitState(index * 7919 + 13);

        bool water = index >= firstHazardSegment && RawWater(index) && !RawWater(index - 1);
        if (water) waterSegments.Add(index);

        BuildGround(root, x0, water);
        BuildClouds(root, x0);

        List<Surface> surfaces = new List<Surface>();
        if (!water || Random.value < 0.85f) BuildPlatforms(root, x0, surfaces);
        if (!water) surfaces.Add(new Surface(x0 + 0.8f, x0 + segmentWidth - 0.8f, groundTopY));
        BuildProduce(root, surfaces);

        Random.state = prev;
    }

    /// <summary>Deterministic, so a gap never sits next to another gap.</summary>
    private static bool RawWater(int index)
    {
        unchecked
        {
            uint h = (uint)index * 2654435761u;
            h ^= h >> 13;
            return (h % 1000u) < 300u;
        }
    }

    /// <summary>A flat top face produce can be placed on.</summary>
    private class Surface
    {
        public float left, right, top;
        public Surface(float l, float r, float t) { left = l; right = r; top = t; }
        public float Width { get { return right - left; } }
    }

    private void BuildGround(GameObject root, float x0, bool water)
    {
        float h = 5f;
        float topY = water ? groundTopY - 0.25f : groundTopY;
        float cx = x0 + segmentWidth * 0.5f;
        float w = segmentWidth;          // exact whole tiles for the art bands
        float wf = segmentWidth + 0.02f; // slight overlap for the solid fills

        if (!water)
        {
            // grass/dirt band from the art, with flat dirt continuing below it
            float band = groundSprite != null ? groundSprite.bounds.size.y : 1.73f;
            MakeTiled("Ground", root.transform, groundSprite,
                new Vector3(cx, topY - band * 0.5f, 0f),
                new Vector2(w, band), Color.white, -5);

            MakeSprite("GroundFill", root.transform, squareSprite,
                new Vector3(cx, topY - band - (h - band) * 0.5f, 0f),
                new Vector3(wf, h - band, 1f), DirtFill, -6);
        }
        else
        {
            // water surface from the art, with matching fill beneath
            float band = waterSprite != null ? waterSprite.bounds.size.y : 2.44f;
            MakeTiled("Water", root.transform, waterSprite,
                new Vector3(cx, topY - band * 0.5f, 0f),
                new Vector2(w, band), Color.white, -5);

            MakeSprite("WaterFill", root.transform, squareSprite,
                new Vector3(cx, topY - band - (h - band) * 0.5f, 0f),
                new Vector3(wf, h - band, 1f), WaterFill, -6);

            // reeds poking out of the water
            int reeds = Random.Range(3, 6);
            for (int i = 0; i < reeds; i++)
            {
                GameObject r = MakeSprite("Reed", root.transform, reedSprite,
                    new Vector3(x0 + Random.Range(0.5f, segmentWidth - 0.5f), topY + 0.42f, 0f),
                    Vector3.one * Random.Range(0.85f, 1.15f), Color.white, -2);
                r.transform.position += new Vector3(0f, Random.Range(-0.1f, 0.25f), 0f);
            }

            // hazard volume covering the water -- unchanged
            GameObject hz = new GameObject("WaterHazard");
            hz.transform.SetParent(root.transform);
            hz.transform.position = new Vector3(x0 + segmentWidth * 0.5f, topY - 1f, 0f);
            BoxCollider2D bc = hz.AddComponent<BoxCollider2D>();
            bc.isTrigger = true;
            bc.size = new Vector2(segmentWidth, 2f);
            hz.AddComponent<Hazard>();
        }
    }

    /// <summary>Sky pinned to the camera, market stalls scrolling behind at a slower rate.</summary>
    private void BuildBackground()
    {
        if (cameraTransform == null) return;

        if (skySprite != null)
        {
            GameObject sky = MakeSprite("Sky", transform, skySprite, Vector3.zero, Vector3.one, Color.white, -30);
            skyLayer = sky.transform;
        }

        if (marketSprite != null)
        {
            GameObject mk = new GameObject("MarketBackdrop");
            mk.transform.SetParent(transform);
            SpriteRenderer sr = mk.AddComponent<SpriteRenderer>();
            sr.sprite = marketSprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(marketSprite.bounds.size.x * 4f, marketSprite.bounds.size.y);
            sr.sortingOrder = -20;
            marketLayer = mk.transform;
            mk.transform.localScale = new Vector3(marketScale, marketScale, 1f);
        }

        UpdateBackground();
    }

    private void UpdateBackground()
    {
        Camera cam = Camera.main;
        Vector3 camPos = cameraTransform.position;

        if (skyLayer != null && cam != null && skySprite != null)
        {
            float camH = cam.orthographicSize * 2f;
            float camW = camH * cam.aspect;
            float s = Mathf.Max(camW / skySprite.bounds.size.x, camH / skySprite.bounds.size.y);
            skyLayer.localScale = new Vector3(s, s, 1f);
            skyLayer.position = new Vector3(camPos.x, camPos.y, 0f);
        }

        if (marketLayer != null && marketSprite != null)
        {
            // slower-moving layer, nudged by whole tiles so the repeat never shows a seam
            float tile = marketSprite.bounds.size.x * marketScale;  // Account for scale
            float lx = camPos.x * (1f - marketParallax);
            lx += Mathf.Round((camPos.x - lx) / tile) * tile;
            marketLayer.position = new Vector3(lx, marketBaseY + camPos.y * 0.12f, 0f);
        }
    }

    private void BuildPlatforms(GameObject root, float x0, List<Surface> surfaces)
    {
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            float w = Random.Range(2.2f, 4.4f);
            float px = x0 + Random.Range(0.6f, segmentWidth - w - 0.3f) + w * 0.5f;
            float py = Random.Range(-1.9f, 4.4f);

            GameObject p = new GameObject("Platform");
            p.transform.SetParent(root.transform);
            p.transform.position = new Vector3(px, py, 0f);

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = platformSprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(w, 0.48f);
            sr.sortingOrder = -1;

            // solid top face so the rocket can land on it
            BoxCollider2D box = p.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(w, 0.48f);
            box.offset = Vector2.zero;
            p.AddComponent<Platform>();

            surfaces.Add(new Surface(px - w * 0.5f + 0.35f, px + w * 0.5f - 0.35f, py + 0.24f));

            int flowers = Random.Range(1, 4);
            for (int f = 0; f < flowers; f++)
            {
                MakeSprite("Flower", p.transform,
                    flowerSprite,
                    new Vector3(px + Random.Range(-w * 0.4f, w * 0.4f), py + 0.24f, 0f),
                    Vector3.one * Random.Range(0.6f, 0.9f), Color.white, 0);
            }
        }
    }

    private void BuildClouds(GameObject root, float x0)
    {
        int n = Random.Range(0, 3);
        for (int i = 0; i < n; i++)
        {
            MakeSprite("Cloud", root.transform, cloudSprite,
                new Vector3(x0 + Random.Range(0f, segmentWidth), Random.Range(2.2f, 6.4f), 0f),
                Vector3.one * Random.Range(0.55f, 1.1f),
                new Color(1f, 1f, 1f, 0.95f), -8);
        }
    }

    private void BuildProduce(GameObject root, List<Surface> surfaces)
    {
        if (flow == null || surfaces.Count == 0) return;

        // one pickup per surface at most, so nothing ever hangs in empty air
        int count = Mathf.Min(Random.Range(1, 3), surfaces.Count);

        // shuffle so we don't always favour the first ledge
        List<Surface> pool = new List<Surface>(surfaces);
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Surface tmp = pool[i]; pool[i] = pool[j]; pool[j] = tmp;
        }

        for (int i = 0; i < count; i++)
        {
            Surface s = pool[i];
            if (s.Width < 0.5f) continue;

            // 70% of pickups are the item the player is currently hunting
            bool isTarget = Random.value < 0.7f;
            string name = isTarget ? flow.CurrentTarget : GameDefs.Names[Random.Range(0, GameDefs.Names.Length)];
            if (name == flow.CurrentTarget) isTarget = true;

            float px = Random.Range(s.left, s.right);
            float py = s.top + restHeight;   // sitting on the surface

            GameObject c = new GameObject("Pickup_" + name);
            c.transform.SetParent(root.transform);
            c.transform.position = new Vector3(px, py, 0f);

            GameObject glow = MakeSprite("Glow", c.transform, glowSprite,
                new Vector3(px, py, 0f), Vector3.one * 1.5f, new Color(1f, 1f, 1f, 0.85f), 1);

            GameObject icon = MakeSprite("Icon", c.transform, SpriteFor(name),
                new Vector3(px, py, 0f), Vector3.one * 0.85f, Color.white, 3);

            CircleCollider2D col = c.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.55f;

            Collectible cc = c.AddComponent<Collectible>();
            SerializeCollectible(cc, name, icon.GetComponent<SpriteRenderer>(), glow.GetComponent<SpriteRenderer>(), isTarget);
        }
    }

    private void SerializeCollectible(Collectible c, string name, SpriteRenderer icon, SpriteRenderer glow, bool isTarget)
    {
        // Collectible caches its own refs through Setup
        var iconField = typeof(Collectible).GetField("icon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var glowField = typeof(Collectible).GetField("glow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (iconField != null) iconField.SetValue(c, icon);
        if (glowField != null) glowField.SetValue(c, glow);
        c.Setup(name, SpriteFor(name), isTarget);
    }

    private Sprite SpriteFor(string name)
    {
        for (int i = 0; i < GameDefs.Names.Length && i < produceSprites.Length; i++)
            if (GameDefs.Names[i] == name) return produceSprites[i];
        return produceSprites.Length > 0 ? produceSprites[0] : null;
    }

    private GameObject MakeTiled(string name, Transform parent, Sprite sprite, Vector3 pos, Vector2 size, Color color, int order)
    {
        // fall back to a tinted square if the artwork slot is empty
        if (sprite == null)
            return MakeSprite(name, parent, squareSprite, pos, new Vector3(size.x, size.y, 1f), color, order);

        GameObject g = new GameObject(name);
        g.transform.SetParent(parent);
        g.transform.position = pos;
        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        sr.color = color;
        sr.sortingOrder = order;
        return g;
    }

    private GameObject MakeSprite(string name, Transform parent, Sprite sprite, Vector3 pos, Vector3 scale, Color color, int order)
    {
        GameObject g = new GameObject(name);
        g.transform.SetParent(parent);
        g.transform.position = pos;
        g.transform.localScale = scale;
        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = order;
        return g;
    }
}
