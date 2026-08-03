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
    [SerializeField] private TargetWordUI wordUI;
    [SerializeField] private TextMeshProUGUI basketText;
    [SerializeField] private TextMeshProUGUI bannerText;
    [SerializeField] private Sprite puffSprite;
    [SerializeField] private ItemCompletePopup itemCompletePopup;
    [SerializeField] private Image[] heartIcons;
    [SerializeField] private Color fullHeartColor = Color.white;
    [SerializeField] private Color emptyHeartColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Lives")]
    [SerializeField] private int maxLives = 5;

    [Header("Tuning")]
    [SerializeField] private float respawnDelay = 0.9f;
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
        if (wordUI != null) wordUI.SetWord(t.itemName, t.count);
        if (player != null) player.SetTrailColor(GameDefs.ColorOf(t.itemName));
    }

    public void OnCollected(string itemName, Vector3 at)
    {
        if (finished) return;

        bool correct = (itemName == CurrentTarget);

        if (!correct)
        {
            RocketRunGameManager.RaiseIncorrectPickup();
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

        if (collectedForTarget >= targets[targetIndex].count)
        {
            if (wordUI != null) wordUI.Celebrate();
            string completedItem = targets[targetIndex].itemName;
            if (bannerText != null) bannerText.text = "Nice! " + completedItem + " done!";

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
        targetIndex++;

        if (targetIndex >= targets.Count) Finish();
        else ApplyCurrentTarget();
    }

    public void OnPlayerCrashed(Vector3 at)
    {
        SpawnPuff(at, Color.white, 1.8f);
        RocketRunGameManager.RaisePlayerCrashed();
        LoseLife();
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (player == null || level == null) yield break;

        if (lives <= 0)
        {
            RestartRun();
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
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] == null) continue;
            heartIcons[i].color = i < lives ? fullHeartColor : emptyHeartColor;
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

        if (player != null) player.Respawn(new Vector3(0f, 2.5f, 0f));
    }

    private void Finish()
    {
        finished = true;
        if (bannerText != null) bannerText.text = "Anjali has everything for dinner!";
        GameManager.Instance.GameWin();
    }

    private void UpdateBasket()
    {
        if (basketText == null) return;
        int total = 0;
        for (int i = 0; i < targets.Count; i++) total += targets[i].count;
        basketText.text = "Basket " + basket + "/" + total;
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
}
