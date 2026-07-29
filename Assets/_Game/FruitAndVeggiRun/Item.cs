using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private bool isObstacle = false;
    [SerializeField] private float moveSpeed = -5f; // Move towards left
    
    private ShoppingListManager shoppingListManager;
    private bool isCollected = false;
    
    public event Action<string> OnItemCollected;
    
    private void OnEnable()
    {
        if (shoppingListManager == null)
        {
            shoppingListManager = FindObjectOfType<ShoppingListManager>();
        }
    }
    
    private void Update()
    {
        MoveItem();
        
        // Destroy item if it goes off screen
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
    
    private void MoveItem()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected)
            return;
        
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            if (isObstacle)
            {
                HandleObstacleCollision();
            }
            else
            {
                HandleItemCollection();
            }
        }
    }
    
    private void HandleItemCollection()
    {
        isCollected = true;
        
        if (shoppingListManager != null)
        {
            shoppingListManager.CollectItem(itemName);
        }
        
        OnItemCollected?.Invoke(itemName);
        
        // Visual feedback
        Debug.Log("Collected item: " + itemName);
        
        // Destroy after a short delay for animation
        Destroy(gameObject, 0.1f);
    }
    
    private void HandleObstacleCollision()
    {
        Debug.Log("Hit obstacle!");
        // TODO: Add game over or damage logic
        Destroy(gameObject);
    }
    
    public string GetItemName()
    {
        return itemName;
    }
    
    public bool IsObstacle()
    {
        return isObstacle;
    }
    
    public void SetItemName(string name)
    {
        itemName = name;
    }
    
    public void SetIsObstacle(bool obstacle)
    {
        isObstacle = obstacle;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
