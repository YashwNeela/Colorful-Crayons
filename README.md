# Code Changes Done

This document provides a complete and detailed record of all code modifications performed to resolve the reported issues in the project.

---

## 1. Localized Score Fixes (Crayon & Fruit Sorting)

**Objective:** Fix "Score Theft" (Hover Decrement) and "Free Score" (Hover Increment) bugs.

### A. Ownership Check (Fixes "Hover Decrement" / "Phantom Score")

**Bug:** `OnCollectibleExited` decrements score whenever a matching item exits the collider, *even if the item was never inside the box/basket*. This happens because `OnTriggerExit` fires for any item leaving the area, regardless of ownership.
**Files:** `CrayonBox2D.cs`, `FruitCollector.cs`

**Old Code:**

```csharp
public override void OnCollectibleExited(Collectible collectible)
{
    collectible.RemoveFromSnapPoint(); // Resets item state immediately!
    if (CalculatedColorMatches(collectible)) 
    {
        OnItemRemoved(); // Decrements score purely based on color
    }
}
```

**New Implemented Code:**

```csharp
public override void OnCollectibleExited(Collectible collectible)
{
    // [NEW] Verify ownership: Is the collectible actually modifying THIS box?
    bool belongsToBox = false;
    if (collectible.CurrentSnapPoint != null)
    {
        foreach (var sp in snapPoints)
        {
            if (sp == collectible.CurrentSnapPoint)
            {
                belongsToBox = true;
                break;
            }
        }
    }

    if (!belongsToBox) return; // [NEW] Ignore exit if not owned by us

    collectible.RemoveFromSnapPoint();
    // ... Proceed to decrement score
}
```

### B. Double Score Fix (Crayon Only)

**Bug:** Score incremented on **Enter** AND **Snap**, leading to double points.
**Old Code:** `OnCollectibleEntered` called `OnItemCollected`.
**New Code:** `OnCollectibleEntered` does nothing. `SnapCollectibleToCollector` calls `OnItemCollected`.

### C. Logic Reversal (Crayon Only)

**Bug:** `HasValidCollectible` checked `Item.HasFlag(Box)` (wrong direction).
**Old Code:** `if (itemColor.HasFlag(boxColor))`
**New Code:** `if (boxColor.HasFlag(itemColor))`

### D. Helper Accessor

**File:** `Collectible.cs`
**Change:** Added `public SnapPoint CurrentSnapPoint => m_CurrentSnapPoint;` to support valid ownership checks.

---

## 3. UI Button Fixes (Double Click Prevention)

**Objective:** Prevent "Next Level" or "Restart" buttons from being clicked multiple times, causing game loop errors.

**File:** `Assets/_Game/_Scripts/Common/LevelWinUI.cs`
**Change:** Added a listener to immediately disable the button (make it non-interactable) upon the first click.

```csharp
// In OnEnable:
m_NextLevelButton.interactable = false;
m_NextLevelButton.onClick.RemoveAllListeners();
// [NEW CODE]
m_NextLevelButton.onClick.AddListener(()=> { m_NextLevelButton.interactable = false; });
```

**File:** `Assets/_Game/_Scripts/Common/LevelCompletedPopup.cs`
**Change:** Cleared previous listeners before adding new ones to prevent stacking calls.

```csharp
// In SetData:
// [NEW CODE]
m_LevelCompletedButton.onClick.RemoveAllListeners();
m_LevelCompletedButton.onClick.AddListener(() => levelCompletedButtonAction?.Invoke());
```

---

## 4. Input Handling Fixes (Game State Check)

**Objective:** Prevent players from interacting with game items after the game has ended (Win/Loss).

**Files:**

- `Assets/_Game/_Scripts/Sorting/SelectAndSort/FruitSelect.cs`
- `Assets/_Game/_Scripts/Sorting/ShapeSorting/ShapeSelect.cs`
- `Assets/_Game/_Scripts/Sorting/CrayonSorting/CrayonSelect.cs`

**Change:** Added a check for `GameState.Playing` at the start of `OnMouseDown`.

```csharp
private void OnMouseDown()
{
    // [NEW CODE]
    if (SortingGameManager.Instance.CurrentGameState != GameState.Playing)
        return;
    
    // ... selection logic ...
}
```

---
