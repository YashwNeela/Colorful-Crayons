# Code Changes Done

This document provides a complete and detailed record of all code modifications performed to resolve the reported issues in the project.

---

## 1. Localized Score Fixes (Crayon & Fruit Sorting)

**Objective:** Fix "Score Theft" (Hover Decrement) and "Free Score" (Hover Increment) bugs.

### A. Ownership Check (Fixes "Hover Decrement" / "Phantom Score")

**Bug:** `OnCollectibleExited` decrements score whenever a matching item exits the collider, *even if the item was never inside the box/basket*. This happens because `OnTriggerExit` fires for any item leaving the area, regardless of ownership.
**Files:** `CrayonBox2D.cs`, `FruitCollector.cs`

**Old Buggy Code:**

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

## 5. Reflection Game Crash Fixes (Ghost & Leak Prevention)

**Objective:** Fix `NullReferenceException` crashes caused by Singleton re-creation ("Ghosting") during scene unload and memory leaks from persistent event listeners.

**Constraints:** No changes allowed to `SerializedSingleton` base class or `PlaySchoolAPI`. All fixes implemented in `ReflectionRequest`.

### A. CollisionMatrixManager Ghost Fix

**File:** `Assets/_Game/_Scripts/Reflection/ReflectionGameManager.cs`
**Change:** Overrode `GoBackToPlayschool` to include a safety check before accessing `CollisionMatrixManager.Instance`.

```csharp
public override void GoBackToPlayschool()
{
    // ... Load Scene ...
    // [NEW] Check existence before access
    if(FindObjectOfType<CollisionMatrixManager>())
        CollisionMatrixManager.Instance.LoadPlayschoolData();
}
```

### B. TutorialManager Ghost Fix

**Files:** `PlayerStateMachine.cs`, `PlayerController.cs`
**Change:** Replaced unsafe `TutorialManager.Instance` access in `OnDisable` with `FindObjectOfType` check.

```csharp
public void OnDisable()
{
    // [NEW] Check existence before unsubscribe
    var tm = FindObjectOfType<TutorialManager>();
    if (tm != null)
    {
        tm.OnTutorialStarted -= OnTutorialStarted;
        // ...
    }
}
```

### C. SunlightSource Memory Leak

**File:** `SunlightSource.cs`
**Change:** Cached the lambda `Action` to ensure correct unsubscription from `TutorialEventManager`.

### D. Static State Cleanup

**File:** `ReflectionGameManager.cs`
**Change:** Added `OnDestroy` to call `TutorialEventManager.Reset()` and destroy `MirrorSlider`.

---

## 6. Critical Note for Main App Developer (Playschool API Bugs)

**To:** Main App Developer / API Maintainer
**From:** Integration Team
**Context:** We encountered crashes in the specific game integration due to Singleton behavior in the Core API. We worked around this by overriding methods in our local managers, but the core issue remains in the API.

### Issue 1: Singleton "Ghosting" on Unload

**Class:** `SerializedSingleton<T>` and `Singleton<T>`
**Problem:** The `Instance` getter creates a new `GameObject` if `_instance` is null. It does **not** check if the application is quitting or if the scene is unloading.
**Result:** When scripts call `.Instance` inside `OnDestroy` or `OnDisable` (during scene transition), a new "Ghost" Singleton is created after the original was destroyed. This Ghost often crashes because it initializes with missing dependencies (e.g., null `Camera.main`).

**Recommended Fix (In Base Class):**

```csharp
// SerializedSingleton.cs
private static bool m_ApplicationIsQuitting = false;

public void OnDestroy() {
    m_ApplicationIsQuitting = true;
}

public static T Instance {
    get {
        if (m_ApplicationIsQuitting) return null; // [FIX] Don't create if quitting
        // ... existing creation logic ...
    }
}
```

### Issue 2: CollisionMatrixManager Resurrection

**Class:** `CollisionMatrixManager.cs`
**Problem:** The `OnDestroy` method calls `LoadPlayschoolData()`, which calls `.Instance` on itself (indirectly or via other managers).
**Result:** This guarantees that even if destroyed correctly, the object resurrects itself during the destruction phase.

**Recommended Fix:**
Remove logic from `OnDestroy` that requires the Singleton instance to exist.

```csharp
// CollisionMatrixManager.cs
private void OnDestroy()
{
    // REMOVE THIS:
    // LoadPlayschoolData(); 
}
```

**Alternate Fix / Root Cause Node:**
The **Lazy-Loading** pattern in the `Instance` getter is the primary culprit.
- **Problem:** `get { if (_instance == null) _instance = new GameObject... }`
- **Why it fails here:** When `OnDestroy` calls `Instance`, the original is already destroyed (or marked for destruction), so the getter assumes it needs to create a *new* one.
- **Solution:** Removing the lazy instantiation (or adding an `isQuitting` check) prevents this "Zombie" creation loop.
