    using UnityEngine;
using System.Collections.Generic;
using TMKOC;

namespace TMKOC.FruitAndVeggiRun
{

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
        [SerializeField] private float restHeight = 0.52f;        [Tooltip("Smallest gap between two floating platforms, in world units. 0 = auto, which uses the camera's visible width so only one is ever on screen at a time.")]
        [SerializeField] private float platformSpacing = 0f;


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

        [Header("Background - second band")]
        [Tooltip("Second backdrop strip. Give it a SMALLER parallax than the first so it scrolls faster and reads as closer to the player.")]
        [SerializeField] private Sprite marketSprite2;
        [Tooltip("0 = pinned to the camera, 1 = fixed in the world.")]
        [SerializeField] private float marketParallax2 = 0.35f;
        [SerializeField] private float marketBaseY2 = -2.4f;
        [SerializeField] private float marketScale2 = 3f;
        [Tooltip("Draws in front of the first band (-20). Keep it below the ground's -5.")]
        [SerializeField] private int marketOrder2 = -18;

        [SerializeField] private Sprite[] produceSprites; // matches GameDefs.Names order

        [Header("Refs")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameFlow flow;
        [Header("Difficulty")]
        [Tooltip("Bands of the difficulty ramp, applied by how far the player has flown. Keep them sorted by startSegment -- the last band whose startSegment has been passed is the one in force.")]
        [SerializeField] private List<StageConfig> stages = new List<StageConfig>();

        [Header("Birds")]
        [Tooltip("Animated bird prefab (e.g. Mynah_Rig). Takes priority over the sprite below; its own Animator drives the wing flap.")]
        [SerializeField] private GameObject birdPrefab;
        [Tooltip("How wide the bird should read on screen, in world units. The prefab is measured and scaled to match, so re-exported art keeps working.")]
        [SerializeField] private float birdWidth = 1.6f;
        [Tooltip("Tick if the artwork faces right -- birds travel left, towards the player.")]
        [SerializeField] private bool birdFacesRight = true;

        [Tooltip("Fallback only, used when no prefab is assigned above.")]
        [SerializeField] private Sprite birdSprite;
        [Tooltip("How fast a bird drifts back towards the player, in units per second.")]
        [SerializeField] private float birdSpeed = 2.4f;
        // (birdScale is gone -- birdWidth above is measured against the actual art)        [Tooltip("Tints the bird art. Dark reads as a silhouette, which is what the placeholder shape relies on -- set it to white once real artwork is in the slot above.")]
        [SerializeField] private Color birdTint = new Color(0.16f, 0.17f, 0.24f);        [Tooltip("Only needed so the bird interlude can be measured in seconds of flight. Auto-found if left empty.")]
        [SerializeField] private RocketPlayer player;

        // Which band is in force. GameFlow drives this as the shopping list is worked
        // through -- the world does not decide for itself.
        private int stageIndex;

        // World-space window in which birds fly and no fruit is dropped at all. Set
        // when a band with a birdIntroSeconds opens; empty the rest of the time.
        private float birdIntroStartX = float.PositiveInfinity;
        private float birdIntroEndX = float.NegativeInfinity;

        // Birds can be held back for a while after their band opens, so the player gets
        // a stretch of the band's other new thing (water, a second fruit) on its own
        // first. birdsLive stays true once they have arrived.
        private bool birdsLive;
        private bool birdsPending;
        private float birdsStartX;

        // Birds are not allowed out until the player has been warned about them. The
        // tutorial calls ConfirmBirdBriefing() when it has actually shown the bubble;
        // briefingWait is a fallback so a scene with no tutorial is not stuck forever.
        private bool birdsBriefed;
        private float briefingWait;

        [Tooltip("Seconds to wait for the tutorial to acknowledge the bird warning before letting birds out regardless. Only matters in a scene with no tutorial.")]
        [SerializeField] private float birdBriefingTimeout = 5f;

        /// <summary>
        /// True while birds are ready to arrive but the player has not been warned yet.
        /// The tutorial watches this, shows its bubble, then calls ConfirmBirdBriefing().
        /// </summary>
        public bool BirdsAwaitingBriefing { get; private set; }




        private readonly Dictionary<int, GameObject> segments = new Dictionary<int, GameObject>();
        private readonly HashSet<int> waterSegments = new HashSet<int>();
        private int nextSegment;

        private Transform skyLayer;
        private Transform marketLayer;
        private Transform marketLayer2;

        // colours sampled from the bottom edge of the new ground / water art so the
        // fill below each band joins it seamlessly
        private static readonly Color DirtFill = new Color(0.541f, 0.224f, 0.055f);
        private static readonly Color WaterFill = new Color(0.659f, 0.902f, 0.957f);

        // platform placement: how tall one is, and how much clear air to keep around it
        // so two in the same segment never stack up into one unreadable blob
        private const float PlatformHeight = 0.8f;
        // horizontal/vertical clearance is no longer needed: SegmentsPerPlatform keeps
        // platforms a whole screen apart, so two can never crowd each other
        //
        //



        /// <summary>While true, BuildProduce skips spawning pickups (used by the tutorial).</summary>
        public bool SuppressProduce { get; set; }

        /// <summary>Destroys every pickup currently in the level.</summary>
        public void ClearProduce()
        {
            Collectible[] all = FindObjectsOfType<Collectible>();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null) Destroy(all[i].gameObject);
            }
        }

    /// <summary>
        /// Tears the level down and rebuilds it from the beginning.
        ///
        /// Needed after a restart: segments are streamed forwards only and anything
        /// more than two behind the camera is destroyed, so by the time the player is
        /// sent back to x = 0 the world there is long gone. Without this the rocket
        /// respawns into empty sky -- no ground, no platforms, no produce.
        /// </summary>
    public void ResetLevel()
        {
            foreach (KeyValuePair<int, GameObject> kv in segments)
            {
                if (kv.Value == null) continue;
                // deactivate as well as destroy: Destroy is deferred to end of frame,
                // and we are about to rebuild segments at overlapping indices
                kv.Value.SetActive(false);
                Destroy(kv.Value);
            }
            segments.Clear();
            waterSegments.Clear();

            // tutorial drops and anything else parented outside a segment
            ClearProduce();

            // rebuild around wherever the camera actually is, and leave nextSegment
            // consistent with it -- otherwise the very next Update() recycles
            // everything we just built as "behind the camera"
            int camSeg = cameraTransform != null
                ? Mathf.FloorToInt(cameraTransform.position.x / segmentWidth)
                : 0;

            for (int i = camSeg - 1; i <= camSeg + segmentsAhead; i++) BuildSegment(i);
            nextSegment = camSeg + segmentsAhead + 1;
        }

    public float GroundTopY { get { return groundTopY; } }

        private void Awake()
        {
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
            if (flow == null) flow = FindObjectOfType<GameFlow>();
            if (player == null) player = FindObjectOfType<RocketPlayer>();
            EnsureStages();
        }

        /// <summary>
        /// Fills in a sensible ramp when nothing has been authored in the inspector, so
        /// the game is playable straight out of the box. Anything set in the scene wins.
        /// </summary>
        private void EnsureStages()
        {
            if (stages != null && stages.Count > 0) return;

            stages = new List<StageConfig>();
            stages.Add(NewStage("1 - Apples only", 1, 1, false, 0.00f, 1, false, 0.00f, 0f, 0f));
            stages.Add(NewStage("2 - Water",       1, 1, false, 0.28f, 1, false, 0.00f, 0f, 0f));
            stages.Add(NewStage("3 - Two fruits, then birds", 2, 2, true, 0.30f, 2, true, 0.40f, 8f, 14f));
            stages.Add(NewStage("4 - Everything",  3, 2, true,  0.38f, 2, true,  0.55f, 0f, 0f));
        }

        private static StageConfig NewStage(string label, int itemCount, int activeTargets, bool decoys,
            float water, int maxPickupsOnScreen, bool birds, float birdChance,
            float birdIntroSeconds, float birdsAfterSeconds)
        {
            StageConfig s = new StageConfig();
            s.label = label;
            s.itemCount = itemCount;
            s.activeTargets = activeTargets;
            s.decoys = decoys;
            s.waterChance = water;
            s.platformChance = 1f;
            s.maxPickupsOnScreen = maxPickupsOnScreen;
            s.birds = birds;
            s.birdChance = birdChance;
            s.birdIntroSeconds = birdIntroSeconds;
            s.birdsAfterSeconds = birdsAfterSeconds;
            return s;
        }

        /// <summary>
        /// The band currently in force. Nothing here looks at distance -- GameFlow
        /// moves the world on when the shopping list says so.
        /// </summary>
        public StageConfig CurrentStage
        {
            get
            {
                EnsureStages();
                if (stages == null || stages.Count == 0) return null;
                return stages[Mathf.Clamp(stageIndex, 0, stages.Count - 1)];
            }
        }

        public int CurrentStageIndex { get { return stageIndex; } }

        public int StageCount
        {
            get { EnsureStages(); return stages != null ? stages.Count : 0; }
        }

        public StageConfig StageAt(int index)
        {
            EnsureStages();
            if (stages == null || stages.Count == 0) return null;
            return stages[Mathf.Clamp(index, 0, stages.Count - 1)];
        }

        /// <summary>
        /// Opens a difficulty band. Called by GameFlow the moment enough of the
        /// shopping list is done, so the world ahead of the player starts building
        /// itself to the new rules.
        ///
        /// If the band opens with a bird interlude, the window is measured from the
        /// first segment that has NOT been built yet -- everything already streamed in
        /// still holds the old rules, so anchoring to the player would put the birds
        /// in terrain that was laid down before they existed.
        /// </summary>
        public void BeginStage(int index)
        {
            EnsureStages();
            stageIndex = Mathf.Clamp(index, 0, Mathf.Max(0, stages.Count - 1));

            // the world is streamed several segments ahead, so without this the new
            // rules would not show up until the player had flown past everything laid
            // down under the old ones -- a long, flat nothing between bands
            RebuildAhead();

            birdIntroStartX = float.PositiveInfinity;
            birdIntroEndX = float.NegativeInfinity;

            StageConfig s = CurrentStage;

            if (s == null || !s.birds)
            {
                birdsLive = false;
                birdsPending = false;
                BirdsAwaitingBriefing = false;
                return;
            }

            if (s.birdsAfterSeconds > 0f && !birdsLive)
            {
                // hold them back: this band has just introduced something else, and
                // stacking birds on top of it in the same breath teaches neither
                birdsPending = true;
                birdsStartX = PlayerX() + PlayerSpeed() * s.birdsAfterSeconds;
                return;
            }

            StartBirds();
        }

        /// <summary>
        /// Birds are due. The first time round they are held at the gate until the
        /// player has been told what they are -- nothing should be able to cost a life
        /// before it has been introduced.
        /// </summary>
        private void StartBirds()
        {
            birdsPending = false;

            if (!birdsBriefed)
            {
                BirdsAwaitingBriefing = true;
                briefingWait = 0f;
                return;
            }

            ReleaseBirds();
        }

        /// <summary>Called by the tutorial once the "birds cost a life" bubble has been seen.</summary>
        public void ConfirmBirdBriefing()
        {
            if (!BirdsAwaitingBriefing && birdsBriefed) return;
            ReleaseBirds();
        }

        /// <summary>
        /// Actually lets birds into the world, and -- the first time -- opens the
        /// fruit-free stretch that gives the player room to practise dodging.
        /// </summary>
        private void ReleaseBirds()
        {
            BirdsAwaitingBriefing = false;
            birdsBriefed = true;
            birdsPending = false;
            birdsLive = true;

            StageConfig s = CurrentStage;
            if (s == null || s.birdIntroSeconds <= 0f) return;

            // the segments still on screen were laid down before birds existed and are
            // carrying fruit -- sweep them, or the "nothing but birds" stretch opens
            // with a handful of stale pickups still lying about
            ClearProduce();
            RebuildAhead();

            // measured from the first segment that has not been built yet: terrain
            // already on screen cannot grow birds retroactively
            birdIntroStartX = nextSegment * segmentWidth;
            birdIntroEndX = birdIntroStartX + PlayerSpeed() * s.birdIntroSeconds;
        }

        private float PlayerX()
        {
            if (player != null) return player.transform.position.x;
            if (cameraTransform != null) return cameraTransform.position.x;
            return 0f;
        }

        private float PlayerSpeed()
        {
            return player != null ? player.ForwardSpeed : 7.5f;
        }

        /// <summary>What the camera can actually see, in world units.</summary>
        private float VisibleWidth()
        {
            Camera cam = Camera.main;
            return cam != null ? cam.orthographicSize * 2f * cam.aspect : 24f;
        }

        /// <summary>
        /// Throws away the part of the world the player has not reached yet, so a
        /// freshly opened band takes hold within a segment or two instead of five.
        /// Everything up to one segment in front of the camera is left alone -- that is
        /// on screen, and rebuilding it would pop.
        /// </summary>
        private void RebuildAhead()
        {
            if (cameraTransform == null || segments.Count == 0) return;

            int keepThrough = Mathf.FloorToInt(cameraTransform.position.x / segmentWidth) + 1;

            List<int> dead = null;
            foreach (KeyValuePair<int, GameObject> kv in segments)
            {
                if (kv.Key <= keepThrough) continue;
                if (dead == null) dead = new List<int>();
                dead.Add(kv.Key);
            }

            if (dead != null)
            {
                for (int i = 0; i < dead.Count; i++)
                {
                    GameObject g = segments[dead[i]];
                    if (g != null)
                    {
                        // deactivate as well as destroy: Destroy is deferred to end of
                        // frame and these indices are about to be rebuilt
                        g.SetActive(false);
                        Destroy(g);
                    }
                    segments.Remove(dead[i]);
                    waterSegments.Remove(dead[i]);
                }
            }

            nextSegment = keepThrough + 1;
        }

        /// <summary>
        /// True while the player still has the fruit-free dodging stretch ahead of or
        /// around them. GameFlow holds the next fruits back until this clears.
        /// </summary>
        public bool BirdIntroActive
        {
            get
            {
                if (float.IsNegativeInfinity(birdIntroEndX)) return false;
                // Fail towards "over". With no reference to track, an interlude that
                // never clears would leave the player with nothing to collect for the
                // rest of the run -- far worse than skipping the dodging stretch.
                if (player == null && cameraTransform == null) return false;

                float x = player != null ? player.transform.position.x
                        : (cameraTransform != null ? cameraTransform.position.x : 0f);
                return x < birdIntroEndX;
            }
        }

        private bool IsBirdIntroSegment(float x0)
        {
            return x0 + segmentWidth > birdIntroStartX && x0 < birdIntroEndX;
        }

        /// <summary>1-based band number, for the HUD or for debugging.</summary>
        public int StageNumber { get { return stageIndex + 1; } }




        /// <summary>
        /// A bird crossing the flight path. It drifts back towards the player as it
        /// bobs, so it sweeps through the airspace rather than sitting there waiting to
        /// be flown around. Touching one costs a life, exactly like the water.
        ///
        /// The mover, the collider and the despawn logic all live on a plain wrapper at
        /// scale 1, with the artwork parented underneath and scaled to fit. Keeping the
        /// two apart means the collider stays honest in world units and nothing the
        /// prefab's Animator does to its own transform can interfere.
        /// </summary>
        private void BuildBird(GameObject root, float x0)
        {
            GameObject b = new GameObject("Bird");
            b.transform.SetParent(root.transform);

            float bx = x0 + Random.Range(1.2f, segmentWidth - 1.2f);
            float by = Random.Range(groundTopY + 2.2f, 5.0f);
            b.transform.position = new Vector3(bx, by, 0f);

            bool animated = birdPrefab != null;
            if (animated) BuildBirdArt(b.transform);
            else BuildPlaceholderBirdArt(b.transform);

            CircleCollider2D col = b.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = Mathf.Max(0.25f, birdWidth * 0.32f);

            Bird bird = b.AddComponent<Bird>();
            bird.Configure(birdSpeed * Random.Range(0.85f, 1.25f),
                Random.Range(0.45f, 1.05f), cameraTransform, !animated);
        }

        // Drops the prefab in, sizes it to birdWidth and centres its art on the wrapper
        // so the collider actually sits on the bird rather than on its pivot.
        private void BuildBirdArt(Transform parent)
        {
            GameObject art = Instantiate(birdPrefab, parent);
            art.name = "Art";
            art.transform.localRotation = Quaternion.identity;
            art.transform.localPosition = Vector3.zero;
            art.transform.localScale = Vector3.one;

            // Measure at scale 1 so the fit factor is exact -- measuring after an
            // arbitrary placeholder scale and then overwriting localScale with the
            // fit factor (instead of compounding it) silently threw the placeholder
            // away and left birds ~25% oversized.
            Bounds bounds;
            if (TryMeasure(art, out bounds) && bounds.size.x > 0.0001f)
            {
                float s = birdWidth / bounds.size.x;
                art.transform.localScale = new Vector3(birdFacesRight ? -s : s, s, 1f);

                // re-measure: the pivot is rarely the middle of the artwork
                if (TryMeasure(art, out bounds))
                {
                    art.transform.position += parent.position - bounds.center;
                }
            }
            else if (birdFacesRight)
            {
                art.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        private void BuildPlaceholderBirdArt(Transform parent)
        {
            GameObject art = new GameObject("Art");
            art.transform.SetParent(parent);
            art.transform.position = parent.position;

            bool hasSprite = birdSprite != null;
            SpriteRenderer sr = art.AddComponent<SpriteRenderer>();
            sr.sprite = hasSprite ? birdSprite : squareSprite;
            sr.color = birdTint;
            sr.sortingOrder = 6;

            art.transform.localScale = hasSprite
                ? Vector3.one * birdWidth
                : new Vector3(birdWidth * 0.95f, birdWidth * 0.5f, 1f);
        }

        // Combined world bounds of every renderer under a transform.
        private static bool TryMeasure(GameObject go, out Bounds bounds)
        {
            bounds = new Bounds();

            Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
            bool any = false;

            for (int i = 0; i < rs.Length; i++)
            {
                if (rs[i] == null) continue;
                if (!any) { bounds = rs[i].bounds; any = true; }
                else bounds.Encapsulate(rs[i].bounds);
            }
            return any;
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

            // birds held back at the start of their band arrive here
            if (birdsPending && PlayerX() >= birdsStartX) StartBirds();

            // fallback for a scene with no tutorial to give the briefing. Uses scaled
            // time, so it cannot tick away while the warning bubble has the game frozen.
            if (BirdsAwaitingBriefing)
            {
                briefingWait += Time.deltaTime;
                if (briefingWait >= birdBriefingTimeout) ReleaseBirds();
            }

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
                    GameObject g = segments[dead[i]];
                    if (g != null)
                    {
                        // Destroy is deferred to end of frame; deactivate now so a
                        // recycled segment can never be seen on top of a rebuilt one
                        g.SetActive(false);
                        Destroy(g);
                    }
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

            // everything about how hard this stretch is comes from here
            StageConfig stage = CurrentStage;

            // the stretch where birds are being taught: no fruit to chase and no water
            // to fall in, so the only thing to think about is the thing in the air
            bool intro = IsBirdIntroSegment(x0);

            // Adjacency is judged on what the neighbour ACTUALLY became, not on a
            // fresh roll. Re-rolling asks the band that is open NOW about a segment
            // that may have been laid down under a different one -- and if the odds
            // moved in between, two gaps can end up side by side, which reads as one
            // enormous unbroken stretch of water.
            bool prevWasWater = waterSegments.Contains(index - 1);

            bool water = !intro
                      && index >= firstHazardSegment
                      && RawWater(index)
                      && !prevWasWater;
            if (water) waterSegments.Add(index);

            BuildGround(root, x0, water);
        //    BuildClouds(root, x0);

            List<Surface> surfaces = new List<Surface>();
            BuildPlatforms(root, x0, surfaces, stage);
            if (!water) surfaces.Add(new Surface(x0 + 0.8f, x0 + segmentWidth - 0.8f, groundTopY));

            if (!intro) BuildProduce(root, x0, surfaces, stage);

            bool wantBird = intro
                || (stage != null && stage.birds && birdsLive && Random.value < stage.birdChance);
            if (wantBird) BuildBird(root, x0);

            Random.state = prev;
        }

        /// <summary>
        /// Deterministic per segment, so a gap never sits next to another gap. The odds
        /// come from whichever band is open, which is how the opening stretch stays
        /// completely dry without needing a special case for it.
        /// </summary>
        private bool RawWater(int index)
        {
            StageConfig s = CurrentStage;
            if (s == null || s.waterChance <= 0f) return false;

            uint odds = (uint)Mathf.RoundToInt(Mathf.Clamp01(s.waterChance) * 1000f);

            unchecked
            {
                uint h = (uint)index * 2654435761u;
                h ^= h >> 13;
                return (h % 1000u) < odds;
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
                    new Vector2(w, band), Color.white, -3);

                MakeSprite("WaterFill", root.transform, squareSprite,
                    new Vector3(cx, topY - band - (h - band) * 0.5f, 0f),
                    new Vector3(wf, h - band, 1f), WaterFill, -4);

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

        /// <summary>
        /// Sky pinned to the camera, with two market bands scrolling behind it at their
        /// own rates. The second band should be given a smaller parallax and a higher
        /// sorting order than the first, so it moves faster and draws in front -- that
        /// difference in speed is the whole illusion of depth.
        /// </summary>
        private void BuildBackground()
        {
            if (cameraTransform == null) return;

            if (skySprite != null)
            {
                GameObject sky = MakeSprite("Sky", transform, skySprite, Vector3.zero, Vector3.one, Color.white, -30);
                skyLayer = sky.transform;
            }

            if (marketSprite != null)
                marketLayer = MakeParallaxStrip("MarketBackdrop", marketSprite, marketScale, -20);

            if (marketSprite2 != null)
                marketLayer2 = MakeParallaxStrip("MarketBackdrop2", marketSprite2, marketScale2, marketOrder2);

            UpdateBackground();
        }

        /// <summary>
        /// A tiled, horizontally repeating backdrop strip. Four tiles wide is plenty:
        /// the strip is re-centred on the camera every frame in whole-tile steps, so it
        /// only ever has to cover one screen plus a margin.
        /// </summary>
        private Transform MakeParallaxStrip(string name, Sprite sprite, float scale, int order)
        {
            GameObject g = new GameObject(name);
            g.transform.SetParent(transform);
            g.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            
            sr.size = new Vector2(sprite.bounds.size.x * 4f, 12);
            
            sr.sortingOrder = order;

            return g.transform;
        }

        /// <summary>
        /// Slides one strip to its parallax position, then nudges it by whole tiles so
        /// the repeat lands back under the camera and the seam never comes into view.
        /// </summary>
        private void ScrollParallaxStrip(Transform layer, Sprite sprite, float parallax,
            float baseY, float scale, float camX)
        {
            if (layer == null || sprite == null) return;

            float tile = sprite.bounds.size.x * scale;
            if (tile <= 0.0001f) return;

            float lx = camX * (1f - parallax);
            lx += Mathf.Round((camX - lx) / tile) * tile;

            // fixed height: the stalls are meant to sit on the horizon, and letting
            // them track the camera's Y made the whole market bob as the rocket flew
            layer.position = new Vector3(lx, baseY, 0f);
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

            ScrollParallaxStrip(marketLayer,  marketSprite,  marketParallax,  marketBaseY,  marketScale,  camPos.x);
            ScrollParallaxStrip(marketLayer2, marketSprite2, marketParallax2, marketBaseY2, marketScale2, camPos.x);
        }

        public void OnBackButtonClicked()
            {
                RocketRunGameManager.Instance.GoBackToPlayschool();
            }

        // How many segments apart platforms have to be. Derived from what the camera
        // can actually see, so the next one only comes into view as the last leaves.
        private int SegmentsPerPlatform()
        {
            float spacing = platformSpacing > 0f ? platformSpacing : VisibleWidth();
            return Mathf.Max(1, Mathf.CeilToInt(spacing / segmentWidth));
        }

        // Same idea for fruit: a whole screen between pickups when the band asks for
        // one at a time, half a screen when it allows two, and so on.
        private int SegmentsPerPickup(StageConfig stage)
        {
            int onScreen = stage != null ? Mathf.Max(1, stage.maxPickupsOnScreen) : 1;
            return Mathf.Max(1, Mathf.CeilToInt(VisibleWidth() / onScreen / segmentWidth));
        }

    private void BuildPlatforms(GameObject root, float x0, List<Surface> surfaces, StageConfig stage)
        {
            // One platform on screen at a time. Eligibility is worked out from the
            // segment index rather than remembered between calls, so it survives the
            // level being rebuilt mid-run (band changes, restarts) without drifting.
            int every = SegmentsPerPlatform();
            int index = Mathf.RoundToInt(x0 / segmentWidth);
            if (every > 1 && ((index % every) + every) % every != 0) return;

            float chance = stage != null ? Mathf.Clamp01(stage.platformChance) : 1f;
            if (chance <= 0f) return;
            if (chance < 1f && Random.value > chance) return;

            float w = Random.Range(2.2f, 4.4f);
            float px = x0 + Random.Range(0.6f, segmentWidth - w - 0.3f) + w * 0.5f;
            float py = Random.Range(-1.9f, 4.4f);

            GameObject p = new GameObject("Platform");
            p.transform.SetParent(root.transform);
            p.transform.position = new Vector3(px, py, 0f);

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = platformSprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(w, PlatformHeight);
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

        private void BuildProduce(GameObject root, float x0, List<Surface> surfaces, StageConfig stage)
        {
            if (flow == null || surfaces.Count == 0) return;
            if (SuppressProduce) return;

            // nothing to drop while the hunt is paused (e.g. mid bird-interlude)
            if (!flow.HasActiveTarget) return;

            // Fruit is spaced the same way platforms are: eligibility comes from the
            // segment index, so it holds through the rebuilds a band change triggers,
            // and the spacing is read off the camera so it holds on any aspect ratio.
            int every = SegmentsPerPickup(stage);
            int index = Mathf.RoundToInt(x0 / segmentWidth);
            if (every > 1 && ((index % every) + every) % every != 0) return;

            // one per eligible segment, on whichever surface this segment happens to
            // offer -- the ground, or the floating platform if there is one
            Surface s = surfaces[Random.Range(0, surfaces.Count)];
            if (s.Width < 0.5f) return;

            // early bands drop nothing but the fruit on the list -- there is enough to
            // learn without a wrong answer on screen. Decoys switch on per band.
            bool allowDecoys = stage != null && stage.decoys;

            bool isTarget = !allowDecoys || Random.value < 0.7f;
            string name = isTarget
                ? flow.RandomActiveTarget()
                : GameDefs.Names[Random.Range(0, GameDefs.Names.Length)];

            if (string.IsNullOrEmpty(name)) return;

            // a decoy roll can still land on something being hunted -- let it glow
            if (flow.IsActiveTarget(name)) isTarget = true;

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

        private void SerializeCollectible(Collectible c, string name, SpriteRenderer icon, SpriteRenderer glow, bool isTarget)
        {
            // Collectible caches its own refs through Setup
            var iconField = typeof(Collectible).GetField("icon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var glowField = typeof(Collectible).GetField("glow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (iconField != null) iconField.SetValue(c, icon);
            if (glowField != null) glowField.SetValue(c, glow);
            c.Setup(name, SpriteFor(name), isTarget);
        }

        /// <summary>Public lookup so other scripts (e.g. the item-complete popup) can grab the same produce art.</summary>
        public Sprite GetProduceSprite(string name)
        {
            return SpriteFor(name);
        }

    public Collectible SpawnTutorialCollectible(Vector3 position, string itemName, bool isTarget = true)
        {
            GameObject c = new GameObject("TutorialPickup_" + itemName);
            c.transform.SetParent(transform);
            c.transform.position = position;

            GameObject glow = MakeSprite("Glow", c.transform, glowSprite,
                position, Vector3.one * 1.5f, new Color(1f, 1f, 1f, 0.85f), 1);

            GameObject icon = MakeSprite("Icon", c.transform, SpriteFor(itemName),
                position, Vector3.one * 0.85f, Color.white, 3);

            CircleCollider2D col = c.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.55f;

            Collectible cc = c.AddComponent<Collectible>();
            SerializeCollectible(cc, itemName, icon.GetComponent<SpriteRenderer>(), glow.GetComponent<SpriteRenderer>(), isTarget);
            return cc;
        }


        /// <summary>Exposes the water sprite so other scripts (e.g. the tutorial) can reference the same art.</summary>
        public Sprite WaterSprite { get { return waterSprite; } }

        /// <summary>
        /// The bird's artwork as a flat sprite, so the tutorial can put it in an
        /// instruction bubble. Picks the largest piece of the rig -- the body reads as
        /// a bird on its own, a wing or a beak does not.
        /// </summary>
        public Sprite BirdIcon
        {
            get
            {
                if (birdPrefab != null)
                {
                    SpriteRenderer[] rs = birdPrefab.GetComponentsInChildren<SpriteRenderer>(true);
                    SpriteRenderer best = null;

                    for (int i = 0; i < rs.Length; i++)
                    {
                        if (rs[i] == null || rs[i].sprite == null) continue;
                        if (best == null || rs[i].bounds.size.sqrMagnitude > best.bounds.size.sqrMagnitude) best = rs[i];
                    }

                    if (best != null) return best.sprite;
                }
                return birdSprite;
            }
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
}