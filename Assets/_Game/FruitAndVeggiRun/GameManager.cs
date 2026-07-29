using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private ShoppingListManager shoppingListManager;
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private UIManager uiManager;
    
    private bool isGameRunning = true;
    
    private void Awake()
    {
        // Ensure all managers are assigned
        if (inputManager == null)
            inputManager = FindObjectOfType<InputManager>();
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        if (laneManager == null)
            laneManager = FindObjectOfType<LaneManager>();
        if (shoppingListManager == null)
            shoppingListManager = FindObjectOfType<ShoppingListManager>();
        if (itemSpawner == null)
            itemSpawner = FindObjectOfType<ItemSpawner>();
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
    }
    
    private void Start()
    {
        Debug.Log("Game Started!");
        
        // Subscribe to game events
        if (shoppingListManager != null)
        {
            shoppingListManager.OnShoppingListComplete += OnGameComplete;
        }
    }
    
    private void OnGameComplete()
    {
        isGameRunning = false;
        Debug.Log("Game Complete!");
        // TODO: Show win screen or next level
    }
    
    public bool IsGameRunning()
    {
        return isGameRunning;
    }
}
