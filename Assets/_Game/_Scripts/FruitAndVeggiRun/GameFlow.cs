using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMKOC;
using UnityEngine.UI;
using TMKOC.Sorting;

namespace TMKOC.FruitAndVeggiRun
{

    [System.Serializable]
    public class TargetEntry
    {
        public string itemName;
        public int count = 3;
    }

    /// <summary>
    /// Owns the shopping mission: which item is being hunted, how many are left,
    /// crashes and respawns, and the win state.
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
        [Header("Mission")]
        [SerializeField]
        private List<TargetEntry> targets = new List<TargetEntry>();

        [Header("Refs")]
        [SerializeField] private RocketPlayer player;
        [SerializeField] private LevelBuilder level;
        [SerializeField] private CameraRig cameraRig;
        [SerializeField] private TargetWordUI wordUI;
        [SerializeField] private TextMeshProUGUI basketText;
        [SerializeField] private TextMeshProUGUI bannerText;
        [SerializeField] private Sprite puffSprite;
        [SerializeField] private ItemCompletePopup itemCompletePopup;
        [SerializeField] private Image[] heartIcons;
        [SerializeField] private TextMeshProUGUI livesText;
        [Tooltip("The 'fridge is full again' card shown once the whole shopping list is done.")]
        [SerializeField] private StoryCutsceneUI closingStory;
        [Tooltip("TRY AGAIN! panel, shown when the last life is gone.")]
        [SerializeField] private EndScreenUI loseScreen;
        [Tooltip("YOU WIN! panel, shown once the closing cut-scene has played out.")]
            [SerializeField] private EndScreenUI winScreen;
        [Tooltip("Fullscreen confetti burst, fired the moment the whole shopping list is finished.")]
        [SerializeField] private ConfettiUI winConfetti;
        [SerializeField] private Color fullHeartColor = Color.white;
        [SerializeField] private Color emptyHeartColor = new Color(1f, 1f, 1f, 0.25f);

        [Header("Lives")]
        [SerializeField] private int maxLives = 5;

        [Header("Tuning")]
        [SerializeField] private float respawnDelay = 0.9f;
        [Tooltip("Victory lap: seconds of free flying after the last fruit before the closing story plays.")]
        [SerializeField] private float closingStoryDelay = 3.5f;
        [SerializeField] private float floorMargin = 0.35f;

        private int targetIndex;
        private int collectedForTarget;
        private int basket;
        private bool finished;
        private int lives;
        // Later bands put two fruits in play at the same time, so the hunt is a small
        // list rather than a single index. `active` holds indices into `targets`;
        // `activeProgress` mirrors it with how many of each have been collected.
        // `targetIndex` is kept pointing at the first of them purely so CurrentTarget
        // still means something to older callers.
        private readonly List<int> active = new List<int>();
        private readonly List<int> activeProgress = new List<int>();
        private int nextTarget;

        // How many shopping-list items are fully in the basket. This -- not distance
        // flown -- is what moves the game from one difficulty band to the next.
        private int itemsCompleted;
        private int stageIndex;

        // Held up while the opening cut-scene owns the screen, so the first fruit lands
        // on the HUD without a voice line talking over the story.
        private bool suppressTargetAnnounce;

        public string CurrentTarget
        {
            get
            {
                if (targetIndex < 0 || targetIndex >= targets.Count) return GameDefs.Names[0];
                return targets[targetIndex].itemName;
            }
        }

        private void Awake()
        {
            if (targets.Count == 0)
            {
                targets.Add(NewTarget("apple", 3));
                targets.Add(NewTarget("banana", 2));
                targets.Add(NewTarget("mango", 2));
                targets.Add(NewTarget("orange", 2));
                targets.Add(NewTarget("watermelon", 3));
            }

            if (player == null) player = FindObjectOfType<RocketPlayer>();
            if (level == null) level = FindObjectOfType<LevelBuilder>();
            if (cameraRig == null) cameraRig = FindObjectOfType<CameraRig>();
            if (wordUI == null) wordUI = FindObjectOfType<TargetWordUI>();
                    if (itemCompletePopup == null) itemCompletePopup = FindObjectOfType<ItemCompletePopup>();
            if (winConfetti == null) winConfetti = FindObjectOfType<ConfettiUI>();
        }

        private static TargetEntry NewTarget(string n, int c)
        {
            TargetEntry t = new TargetEntry();
            t.itemName = n;
            t.count = c;
            return t;
        }

        private void Start()
        {
            GameManager.Instance.GameStart(0);

            // make sure this game's voice-over bundle is in memory even when the scene
            // is opened directly rather than through the Playschool menu
            if (Voice != null && RuntimeAudioLoader.Instance != null)
            {
                RuntimeAudioLoader.Instance.EnsureCategoryLoaded(Voice.CategoryName);
            }

            // NB: no target call-out here. Start() runs underneath the opening cut-scene,
            // so anything spoken now would be talked over -- hence the suppress flag
            // below. The first 'go' line is AnnounceRunStart(), which the tutorial fires
            // when it hands control back.

            lives = maxLives;
            UpdateLives();

            active.Clear();
            activeProgress.Clear();
            nextTarget = 0;
            itemsCompleted = 0;
            stageIndex = 0;
            finished = false;
            if (level != null) level.BeginStage(0);

            suppressTargetAnnounce = true;
            SyncActiveTargets();
            suppressTargetAnnounce = false;

            UpdateBasket();
            if (bannerText != null) bannerText.text = "Hold anywhere to fly \u2014 grab every " + CurrentTarget + "!";
        }

        private void Update()
        {
            if (player == null || level == null) return;

            // which fruits are being hunted follows the shopping list, and the bird
            // interlude can pause the hunt outright -- so keep the list in step here
            SyncActiveTargets();

            if (!player.Alive) return;

            // fell below the world over water -> crash
            float floorY = level.GroundTopY + floorMargin;
            if (player.transform.position.y <= floorY)
            {
                if (level.IsWaterAt(player.transform.position.x))
                {
                    player.Crash();
                }
                else
                {
                    // skim along the grass instead of sinking into it
                    Vector3 p = player.transform.position;
                    p.y = floorY;
                    player.transform.position = p;
                }
            }
        }

        // Pushes the whole active list into the HUD and re-tags any pickups already
        // lying in the world, so a fruit that has only just joined the hunt starts
        // glowing rather than staying a decoy.
        private void ApplyCurrentTarget()
        {
            targetIndex = active.Count > 0 ? active[0] : targets.Count;
            collectedForTarget = active.Count > 0 ? activeProgress[0] : 0;

            if (wordUI != null)
            {
                List<string> names = new List<string>();
                List<int> counts = new List<int>();
                List<Sprite> icons = new List<Sprite>();

                for (int i = 0; i < active.Count; i++)
                {
                    TargetEntry t = targets[active[i]];
                    names.Add(t.itemName);
                    counts.Add(t.count);
                    icons.Add(level != null ? level.GetProduceSprite(t.itemName) : null);
                }

                wordUI.SetTargets(names, counts, icons);

                // SetTargets zeroes every fill, so put progress back afterwards -- a
                // second fruit joining must not wipe the first one's ring
                for (int i = 0; i < active.Count; i++) wordUI.SetSlotProgress(i, activeProgress[i]);
            }

            if (player != null && active.Count > 0)
            {
                player.SetTrailColor(GameDefs.ColorOf(targets[active[0]].itemName));
            }

            Collectible[] all = FindObjectsOfType<Collectible>();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null) all[i].SetIsTarget(IsActiveTarget(all[i].ItemName));
            }
        }

    private void SetCollectiblesVisible(bool visible)
        {
            Collectible[] collectibles = FindObjectsOfType<Collectible>();
            for (int i = 0; i < collectibles.Length; i++)
            {
                collectibles[i].SetVisible(visible);
            }
        }


        public void OnCollected(string itemName, Vector3 at)
        {
            if (finished) return;

            if (PracticeMode)
            {
                // all of the feel, none of the bookkeeping. No voice line either: the
                // tutorial is talking, and praise on top of instructions is a mess.
                if (IsActiveTarget(itemName))
                {
                    SpawnPuff(at, GameDefs.ColorOf(itemName), 1f);
                    RocketRunGameManager.RaiseCorrectPickup();
                }
                else
                {
                    RocketRunGameManager.RaiseIncorrectPickup();
                    PlayCrashFeedback(at, false);
                }
                return;
            }

            int slot = ActiveSlotOf(itemName);

            if (slot < 0)
            {
                RocketRunGameManager.RaiseIncorrectPickup();
                // a wrong pickup is a free mistake: SFX and puff only -- no voice line,
                // no life lost, and the rocket keeps flying
                PlayCrashFeedback(at, false);
                return;
            }

            SpawnPuff(at, GameDefs.ColorOf(itemName), 1f);
            RocketRunGameManager.RaiseCorrectPickup();

            activeProgress[slot]++;
            basket++;
            UpdateBasket();

            if (slot == 0) collectedForTarget = activeProgress[0];
            if (wordUI != null) wordUI.SetSlotProgress(slot, activeProgress[slot]);

            int entry = active[slot];

            // praise every pickup except the one that finishes the item -- that gets its
            // own 'all the apples are in the basket!' line a few lines down
            if (activeProgress[slot] < targets[entry].count)
            {
                if (Voice != null) RocketRunVoice.Play(Voice.GetRandomCorrectPickup());
                return;
            }

            if (wordUI != null) wordUI.CelebrateSlot(slot);

            string completedItem = targets[entry].itemName;
            if (bannerText != null) bannerText.text = "Nice! " + completedItem + " done!";

            // the item-complete line replaces the per-pickup praise, so the two
            // never talk over each other on the final fruit of an item
            if (Voice != null) RocketRunVoice.Play(Voice.GetDoneAudio(completedItem));

            // retire just this fruit -- anything hunted alongside it carries on
            itemsCompleted++;
            active.RemoveAt(slot);
            activeProgress.RemoveAt(slot);

            SetCollectiblesVisible(false);
            if (wordUI != null) wordUI.SetIconContainerVisible(false);

            if (itemCompletePopup != null)
            {
                Sprite icon = level != null ? level.GetProduceSprite(completedItem) : null;
                itemCompletePopup.Show(completedItem, icon, AdvanceTarget);
            }
            else
            {
                AdvanceTarget();
            }
        }
    private void AdvanceTarget()
        {
            SetCollectiblesVisible(true);
            if (wordUI != null) wordUI.SetIconContainerVisible(true);

            // the item that just landed in the basket may have opened the next band.
            // If that band starts with a bird interlude, SyncActiveTargets deliberately
            // hands back nothing until the player has flown through it -- which is also
            // what keeps the world fruit-free for the length of the dodging stretch.
            UpdateStage();
            SyncActiveTargets();

            if (active.Count == 0 && nextTarget >= targets.Count)
            {
                Finish();
                return;
            }

            // whatever joined the hunt was called out by SyncActiveTargets as it was
            // added -- safe to speak there because the item-complete popup has already
            // finished its own line and dismissed itself
            ApplyCurrentTarget();
        }

        // ---- active-target bookkeeping ---------------------------------------

        /// <summary>True when this fruit is one of the ones currently being hunted.</summary>
        public bool IsActiveTarget(string itemName)
        {
            return ActiveSlotOf(itemName) >= 0;
        }

        /// <summary>
        /// One of the fruits currently being hunted, picked at random. LevelBuilder
        /// calls this when deciding what to drop, so the later bands scatter both
        /// targets rather than only ever the first one.
        /// </summary>
        public string RandomActiveTarget()
        {
            if (active.Count == 0) return CurrentTarget;
            return targets[active[Random.Range(0, active.Count)]].itemName;
        }

        private int ActiveSlotOf(string itemName)
        {
            for (int i = 0; i < active.Count; i++)
            {
                if (targets[active[i]].itemName == itemName) return i;
            }
            return -1;
        }

        // How many fruits the open band wants on the HUD at once.
        private int DesiredActiveCount()
        {
            StageConfig s = level != null ? level.StageAt(stageIndex) : null;
            return s != null ? Mathf.Max(1, s.activeTargets) : 1;
        }

        /// <summary>True while at least one fruit is being hunted. False during the bird interlude.</summary>
        public bool HasActiveTarget { get { return active.Count > 0; } }

        /// <summary>
        /// While true a pickup looks and sounds exactly as it does in the real run but
        /// adds nothing to the basket. The opening tutorial holds this on so the sky is
        /// never empty while the player is still learning to fly, without letting a
        /// lucky first flight finish the whole item before the lesson is over.
        /// </summary>
        public bool PracticeMode { get; set; }

        // Which band a given number of finished items falls in. Each band swallows its
        // own itemCount worth of the list; the last one takes whatever is left over.
        private int StageIndexFor(int itemsDone)
        {
            if (level == null) return 0;

            int remaining = itemsDone;
            int count = level.StageCount;

            for (int i = 0; i < count; i++)
            {
                StageConfig s = level.StageAt(i);
                int take = s != null ? Mathf.Max(1, s.itemCount) : 1;
                if (remaining < take) return i;
                remaining -= take;
            }
            return Mathf.Max(0, count - 1);
        }

        // First list item belonging to a band.
        private int StageItemStart(int index)
        {
            if (level == null) return 0;

            int n = 0;
            for (int i = 0; i < index && i < level.StageCount; i++)
            {
                StageConfig s = level.StageAt(i);
                n += s != null ? Mathf.Max(1, s.itemCount) : 1;
            }
            return n;
        }

        // One past the last list item belonging to a band. The final band absorbs any
        // remainder, so a mis-typed itemCount can never strand a fruit unreachable.
        private int StageItemEnd(int index)
        {
            if (level == null || index >= level.StageCount - 1) return targets.Count;

            StageConfig s = level.StageAt(index);
            int take = s != null ? Mathf.Max(1, s.itemCount) : 1;
            return Mathf.Min(targets.Count, StageItemStart(index) + take);
        }

        // Opens the band the current progress calls for, and tells the world about it.
        private void UpdateStage()
        {
            int idx = StageIndexFor(itemsCompleted);
            if (idx == stageIndex && level != null && level.CurrentStageIndex == idx) return;

            stageIndex = idx;
            if (level == null) return;

            level.BeginStage(stageIndex);

            if (level.BirdIntroActive && bannerText != null)
            {
                bannerText.text = "Look out \u2014 birds! Fly around them!";
            }
        }

        // Tops the hunt up to whatever the open band wants, never reaching past the end
        // of that band's slice of the list. Only ever adds: a fruit the player has
        // already part-collected is never taken away.
        private void SyncActiveTargets()
        {
            if (finished) return;

            // birds first. Nothing is hunted while the player learns to dodge, and that
            // empty hunt is also what stops LevelBuilder dropping fruit through it.
            if (level != null && level.BirdIntroActive) return;

            int end = Mathf.Min(StageItemEnd(stageIndex), targets.Count);
            int want = DesiredActiveCount();
            int firstAdded = -1;

            while (active.Count < want && nextTarget < end)
            {
                if (firstAdded < 0) firstAdded = nextTarget;
                active.Add(nextTarget);
                activeProgress.Add(0);
                nextTarget++;
            }

            if (firstAdded < 0) return;

            ApplyCurrentTarget();
            if (suppressTargetAnnounce) return;

            // whatever just joined the hunt gets called out. Coming off the bird stretch
            // the HUD was empty, so an icon appearing unannounced would read as a bug.
            string announced = targets[firstAdded].itemName;
            if (bannerText != null) bannerText.text = "Now grab every " + announced + "!";
            if (Voice != null) RocketRunVoice.Play(Voice.GetFindAudio(announced));
        }

    /// <summary>
        /// The "ready, set, fly" line. Called by RocketRunTutorial the moment it stops
        /// interrupting and real play begins, which is the first point in the whole
        /// opening sequence where nothing else is speaking.
        /// </summary>
        public void AnnounceRunStart()
        {
            if (Voice != null) RocketRunVoice.Play(Voice.GameStart);
        }


        /// <summary>
        /// Puff and crash SFX. Shared by hazard crashes and wrong pickups; only a real
        /// crash gets the spoken line -- a wrong pickup stays SFX-only.
        /// </summary>
        private void PlayCrashFeedback(Vector3 at, bool withVoice)
        {
            SpawnPuff(at, Color.white, 1.8f);
            RocketRunGameManager.RaisePlayerCrashed();
            if (withVoice && Voice != null) RocketRunVoice.Play(Voice.GetRandomCrash());
        }

        public void OnPlayerCrashed(Vector3 at)
        {
            PlayCrashFeedback(at, true);
            LoseLife();
            StartCoroutine(RespawnRoutine());
        }

    private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            if (player == null || level == null) yield break;

            if (lives <= 0)
            {
                // hand over to the retry screen; it freezes the game and calls
                // RestartRun only if the player chooses to play again
                if (loseScreen != null) loseScreen.Show(RestartRun);
                else RestartRun();
                yield break;
            }

            float safeX = level.FindSafeX(player.transform.position.x - 2f);
            player.Respawn(new Vector3(safeX, 2.5f, 0f));
        }

        private void LoseLife()
        {
            lives = Mathf.Max(0, lives - 1);
            UpdateLives();
        }

    private void UpdateLives()
        {
            if (livesText != null) livesText.text = lives.ToString();

            if (heartIcons == null) return;
            for (int i = 0; i < heartIcons.Length; i++)
            {
                if (heartIcons[i] == null) continue;
                // single-heart HUD: the heart stays lit and the number carries the count
                heartIcons[i].color = (heartIcons.Length == 1)
                    ? (lives > 0 ? fullHeartColor : emptyHeartColor)
                    : (i < lives ? fullHeartColor : emptyHeartColor);
            }
        }

        /// <summary>Out of lives -- reset the whole run: lives, mission progress, basket, player position.</summary>
        private void RestartRun()
        {
            lives = maxLives;
            UpdateLives();

            active.Clear();
            activeProgress.Clear();
            nextTarget = 0;
            targetIndex = 0;
            collectedForTarget = 0;
            itemsCompleted = 0;
            stageIndex = 0;
            basket = 0;
            finished = false;
            UpdateBasket();

            // Order matters. Put the rocket back first, then drag the camera to it in
            // the same breath -- CameraRig only catches up in LateUpdate, so without
            // the snap the level streamer still thinks we are out at the crash site
            // and recycles the freshly built world as "behind the camera". Only then
            // is it safe to rebuild, because ResetLevel keys off the camera position.
            if (player != null) player.Respawn(new Vector3(0f, 2.5f, 0f));
            if (cameraRig != null) cameraRig.SnapToTarget();
            if (level != null) level.ResetLevel();

            // back to band one -- apples only, dry ground, no birds. Has to come after
            // ResetLevel, which is what re-seeds the segment counter BeginStage reads.
            if (level != null) level.BeginStage(0);

            suppressTargetAnnounce = true;
            SyncActiveTargets();
            suppressTargetAnnounce = false;
            ApplyCurrentTarget();

            if (bannerText != null) bannerText.text = "Hold anywhere to fly — grab every " + CurrentTarget + "!";
        }

        private void Finish()
        {
            finished = true;
            if (bannerText != null) bannerText.text = "Anjali has everything for dinner!";

            // whole shopping list is done -- fullscreen confetti over the HUD while the
            // player takes the victory lap, before the closing story card takes over
            if (winConfetti != null) winConfetti.PlayParticle();

            if (Voice != null) RocketRunVoice.Play(Voice.GameComplete);
            StartCoroutine(FinishRoutine());
        }

        /// <summary>
        /// Lets the player fly on for a short victory lap, then plays the closing
        /// story card before handing over to the usual win flow.
        /// </summary>
    private IEnumerator FinishRoutine()
        {
            yield return new WaitForSeconds(closingStoryDelay);

            if (closingStory != null)
            {
                bool done = false;
                closingStory.Play(delegate { done = true; });
                while (!done) yield return null;
            }

            GameManager.Instance.GameWin();

            // same panel as the lose screen, different badge -- play again resets the run
            // in place exactly as it does after running out of lives
            if (winScreen != null) winScreen.Show(RestartRun);
        }

        private void UpdateBasket()
        {
            if (basketText == null) return;
            int total = 0;
            for (int i = 0; i < targets.Count; i++) total += targets[i].count;
            basketText.text = /*"Basket "*/ + basket + "/" + total;
        }

        private void SpawnPuff(Vector3 at, Color c, float size)
        {
            if (puffSprite == null) return;

            GameObject g = new GameObject("Puff");
            g.transform.position = at;
            SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
            sr.sprite = puffSprite;
            sr.sortingOrder = 20;
            Puff p = g.AddComponent<Puff>();
            p.Play(c, size);
        }


    /// <summary>RocketRun's voice-over key table, or null when no mapper is in the scene.</summary>
        private RocketRunAudioMapper Voice
        {
            get { return RocketRunVoice.Mapper; }
        }
    }
}
