using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private ShoppingListManager shoppingListManager;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Sprite obstacleSprite;
    [SerializeField] private float spawnRate = 1.2f;
    [SerializeField] private float spawnPositionX = 12f;
    [SerializeField] private float obstacleChance = 0.3f;

    private float nextSpawnTime = 0f;

    private static readonly string[] ItemNames = { "Tomato", "Carrot", "Potato", "Apple", "Banana" };

    private static Color ColorFor(string n)
    {
        if (n == "Tomato") return new Color(0.90f, 0.20f, 0.16f);
        if (n == "Carrot") return new Color(0.95f, 0.55f, 0.13f);
        if (n == "Potato") return new Color(0.76f, 0.61f, 0.38f);
        if (n == "Apple") return new Color(0.85f, 0.15f, 0.35f);
        if (n == "Banana") return new Color(0.98f, 0.85f, 0.20f);
        return Color.white;
    }

    private void Awake()
    {
        if (laneManager == null) laneManager = FindObjectOfType<LaneManager>();
        if (shoppingListManager == null) shoppingListManager = FindObjectOfType<ShoppingListManager>();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnItem();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private string PickItemName()
    {
        if (shoppingListManager != null)
        {
            List<string> needed = new List<string>();
            foreach (ShoppingItem s in shoppingListManager.GetShoppingList())
                if (s.collectedCount < s.targetCount) needed.Add(s.itemName);
            if (needed.Count > 0 && Random.value < 0.8f)
                return needed[Random.Range(0, needed.Count)];
        }
        return ItemNames[Random.Range(0, ItemNames.Length)];
    }

    private void SpawnItem()
    {
        if (itemPrefab == null || laneManager == null) return;

        int lane = laneManager.GetRandomLane();
        Vector3 spawnPos = new Vector3(spawnPositionX, laneManager.GetLanePosition(lane).y, 0f);

        GameObject go = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        Item item = go.GetComponent<Item>();
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

        bool isObstacle = Random.value < obstacleChance;

        if (isObstacle)
        {
            go.name = "Obstacle";
            item.SetItemName("Obstacle");
            item.SetIsObstacle(true);
            if (sr != null)
            {
                if (obstacleSprite != null) sr.sprite = obstacleSprite;
                sr.color = new Color(0.42f, 0.27f, 0.15f);
            }
            go.transform.localScale = new Vector3(1.0f, 1.0f, 1f);
        }
        else
        {
            string n = PickItemName();
            go.name = n;
            item.SetItemName(n);
            item.SetIsObstacle(false);
            if (sr != null)
            {
                if (itemSprite != null) sr.sprite = itemSprite;
                sr.color = ColorFor(n);
            }
            go.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        }
    }
}
