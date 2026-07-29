using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ShoppingListManager shoppingListManager;
    [SerializeField] private TextMeshProUGUI shoppingListText;
    [SerializeField] private TextMeshProUGUI basketCountText;
    [SerializeField] private RectTransform progressBar;

    private void Awake()
    {
        if (shoppingListManager == null) shoppingListManager = FindObjectOfType<ShoppingListManager>();
    }

    private void OnEnable()
    {
        if (shoppingListManager != null)
        {
            shoppingListManager.OnItemCollected += HandleItemCollected;
            shoppingListManager.OnBasketUpdated += UpdateBasketUI;
            shoppingListManager.OnShoppingListComplete += OnCompleteGame;
        }
    }

    private void OnDisable()
    {
        if (shoppingListManager != null)
        {
            shoppingListManager.OnItemCollected -= HandleItemCollected;
            shoppingListManager.OnBasketUpdated -= UpdateBasketUI;
            shoppingListManager.OnShoppingListComplete -= OnCompleteGame;
        }
    }

    private void Start()
    {
        UpdateShoppingListDisplay();
        UpdateBasketUI();
        UpdateProgressBar();
    }

    private void UpdateShoppingListDisplay()
    {
        if (shoppingListManager == null || shoppingListText == null) return;

        string s = "SHOPPING LIST:   ";
        foreach (ShoppingItem item in shoppingListManager.GetShoppingList())
        {
            bool done = item.collectedCount >= item.targetCount;
            string col = done ? "#2E7D32" : "#1A1A1A";
            s += "<color=" + col + "><b>" + item.itemName + " " + item.collectedCount + "/" + item.targetCount + "</b></color>   ";
        }
        shoppingListText.text = s;
    }

    private void HandleItemCollected(string itemName, int collected, int target)
    {
        UpdateShoppingListDisplay();
        UpdateProgressBar();
    }

    private void UpdateBasketUI()
    {
        if (shoppingListManager == null || basketCountText == null) return;
        basketCountText.text = "Basket " + shoppingListManager.GetBasketCount() + "/" + shoppingListManager.GetBasketCapacity();
    }

    private void UpdateProgressBar()
    {
        if (shoppingListManager == null || progressBar == null) return;

        float totalTarget = 0f;
        float totalCollected = 0f;
        foreach (ShoppingItem item in shoppingListManager.GetShoppingList())
        {
            totalTarget += item.targetCount;
            totalCollected += item.collectedCount;
        }

        float progress = totalTarget > 0f ? totalCollected / totalTarget : 0f;
        progressBar.localScale = new Vector3(progress, 1f, 1f);
    }

    private void OnCompleteGame()
    {
        if (shoppingListText != null)
            shoppingListText.text = "<color=#2E7D32><b>SHOPPING COMPLETE! Dinner time!</b></color>";
    }
}
