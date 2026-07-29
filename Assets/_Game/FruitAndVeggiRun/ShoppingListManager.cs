using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class ShoppingItem
{
    public string itemName;
    public int targetCount;
    public int collectedCount;
}

public class ShoppingListManager : MonoBehaviour
{
    [SerializeField] private List<ShoppingItem> shoppingList = new List<ShoppingItem>();
    [SerializeField] private int basketCapacity = 20;
    
    private int currentBasketCount = 0;
    
    public event Action<string, int, int> OnItemCollected; // itemName, collected, target
    public event Action OnBasketUpdated; // currentCount, capacity
    public event Action OnShoppingListComplete;
    
    private void Awake()
    {
        InitializeShoppingList();
    }
    
    private void InitializeShoppingList()
    {
        // Setup default shopping list if empty
        if (shoppingList.Count == 0)
        {
            shoppingList.Add(new ShoppingItem { itemName = "Tomato", targetCount = 3, collectedCount = 0 });
            shoppingList.Add(new ShoppingItem { itemName = "Carrot", targetCount = 2, collectedCount = 0 });
            shoppingList.Add(new ShoppingItem { itemName = "Potato", targetCount = 2, collectedCount = 0 });
            shoppingList.Add(new ShoppingItem { itemName = "Apple", targetCount = 2, collectedCount = 0 });
            shoppingList.Add(new ShoppingItem { itemName = "Banana", targetCount = 3, collectedCount = 0 });
        }
        
        Debug.Log("Shopping list initialized with " + shoppingList.Count + " items");
    }
    
    public void CollectItem(string itemName)
    {
        ShoppingItem item = shoppingList.Find(x => x.itemName == itemName);
        
        if (item != null && currentBasketCount < basketCapacity)
        {
            if (item.collectedCount < item.targetCount)
            {
                item.collectedCount++;
                currentBasketCount++;
                
                OnItemCollected?.Invoke(itemName, item.collectedCount, item.targetCount);
                OnBasketUpdated?.Invoke();
                
                Debug.Log("Collected: " + itemName + " (" + item.collectedCount + "/" + item.targetCount + ")");
                
                if (IsShoppingListComplete())
                {
                    OnShoppingListComplete?.Invoke();
                    Debug.Log("Shopping list complete!");
                }
            }
        }
        else if (currentBasketCount >= basketCapacity)
        {
            Debug.LogWarning("Basket is full!");
        }
    }
    
    public bool IsShoppingListComplete()
    {
        foreach (ShoppingItem item in shoppingList)
        {
            if (item.collectedCount < item.targetCount)
                return false;
        }
        return true;
    }
    
    public int GetBasketCount()
    {
        return currentBasketCount;
    }
    
    public int GetBasketCapacity()
    {
        return basketCapacity;
    }
    
    public List<ShoppingItem> GetShoppingList()
    {
        return shoppingList;
    }
    
    public ShoppingItem GetShoppingItem(string itemName)
    {
        return shoppingList.Find(x => x.itemName == itemName);
    }
}
