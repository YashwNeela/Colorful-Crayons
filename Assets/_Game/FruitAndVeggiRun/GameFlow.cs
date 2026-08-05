using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMKOC;
using UnityEngine.UI;

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
        // so anything spoken now would be talked over. The first "go" line is
        // AnnounceRunStart(), which the tutorial fires when it hands control back.

        lives = maxLives;
        UpdateLives();
        ApplyCurrentTarget();
        UpdateBasket();
        if (bannerText != null) bannerText.text = "Hold anywhere to fly \u2014 grab every " + CurrentTarget + "!";
    }

    private void Update()
    {
        if (player == null || level == null || !player.Alive) return;

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

    private void ApplyCurrentTarget()
    {
        if (targetIndex >= targets.Count) return;

        collectedForTarget = 0;
        TargetEntry t = targets[targetIndex];
        Sprite icon = level != null ? level.GetProduceSprite(t.itemName) : null;
        if (wordUI != null) wordUI.SetWord(t.itemName, t.count, icon);
        if (player != null) player.SetTrailColor(GameDefs.ColorOf(t.itemName));
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

        bool correct = (itemName == CurrentTarget);

        if (!correct)
        {
            RocketRunGameManager.RaiseIncorrectPickup();
            if (Voice != null) RocketRunVoice.Play(Voice.GetRandomWrongPickup());
            // same restarting treatment as a water crash: puff, lose a life, respawn
            if (player != null) player.Crash();
            return;
        }

        SpawnPuff(at, GameDefs.ColorOf(itemName), 1f);
        RocketRunGameManager.RaiseCorrectPickup();

        collectedForTarget++;
        basket++;
        UpdateBasket();

        if (wordUI != null) wordUI.SetProgress(collectedForTarget);

        // praise every pickup except the one that finishes the item -- that gets its
        // own "all the apples are in the basket!" line a few lines down
        bool completesItem = collectedForTarget >= targets[targetIndex].count;
        if (!completesItem && Voice != null) RocketRunVoice.Play(Voice.GetRandomCorrectPickup());

        if (collectedForTarget >= targets[targetIndex].count)
        {
            if (wordUI != null) wordUI.Celebrate();
            string completedItem = targets[targetIndex].itemName;
            if (bannerText != null) bannerText.text = "Nice! " + completedItem + " done!";

            // the item-complete line replaces the per-pickup praise, so the two
            // never talk over each other on the final fruit of an item
            if (Voice != null) RocketRunVoice.Play(Voice.GetDoneAudio(completedItem));

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
    }
private void AdvanceTarget()
    {
        SetCollectiblesVisible(true);
        if (wordUI != null) wordUI.SetIconContainerVisible(true);
        targetIndex++;

        if (targetIndex >= targets.Count)
        {
            Finish();
            return;
        }

        ApplyCurrentTarget();

        // "Now find the bananas!" -- safe to speak here because the item-complete
        // popup has already finished its own line and dismissed itself
        if (Voice != null) RocketRunVoice.Play(Voice.GetFindAudio(CurrentTarget));
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


    public void OnPlayerCrashed(Vector3 at)
    {
        SpawnPuff(at, Color.white, 1.8f);
        RocketRunGameManager.RaisePlayerCrashed();
        if (Voice != null) RocketRunVoice.Play(Voice.GetRandomCrash());
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

        targetIndex = 0;
        basket = 0;
        finished = false;
        UpdateBasket();
        ApplyCurrentTarget();

        if (bannerText != null) bannerText.text = "Hold anywhere to fly \u2014 grab every " + CurrentTarget + "!";

        // Order matters. Put the rocket back first, then drag the camera to it in
        // the same breath -- CameraRig only catches up in LateUpdate, so without
        // the snap the level streamer still thinks we are out at the crash site
        // and recycles the freshly built world as "behind the camera". Only then
        // is it safe to rebuild, because ResetLevel keys off the camera position.
        if (player != null) player.Respawn(new Vector3(0f, 2.5f, 0f));
        if (cameraRig != null) cameraRig.SnapToTarget();
        if (level != null) level.ResetLevel();
    }

private void Finish()
    {
        finished = true;
        if (bannerText != null) bannerText.text = "Anjali has everything for dinner!";
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
