# TMKOC Code Changes Documentation

This document serves as the official changelog for the code modifications applied to resolve critical crashes, gameplay bugs, and UI issues in the project.

---

## 1. Core System Fixes (Critical Stability)

**Objective:** Prevent "Ghost Object" creation and `NullReferenceException` crashes during scene transitions and application exit.

### A. Singleton Lazy-Loading Fix

**File:** `Assets/_Game/_Scripts/Common/SerializedSingleton.cs`
**Issue:** The `Instance` getter used "Lazy Loading" (auto-creation) without checking if the application was quitting. This caused destroyed Singletons (like `TutorialManager` or `CollisionMatrixManager`) to be resurrected as empty "Ghost" objects during the destruction phase, leading to crashes in other scenes.
**Fix:** Implemented an `m_applicationIsQuitting` flag (using `Application.quitting` event) to abort Lazy Loading during shutdown.

```csharp
private static bool m_applicationIsQuitting = false;

protected virtual void OnEnable() {
    Application.quitting -= OnApplicationQuitting;
    Application.quitting += OnApplicationQuitting;
}

protected virtual void OnDisable() {
    Application.quitting -= OnApplicationQuitting;
}

private void OnApplicationQuitting() {
    m_applicationIsQuitting = true;
}

public static T Instance {
    get {
        if (m_applicationIsQuitting) return null; // [FIX] Abort creation if quitting
        // ... existing creation logic ...
    }
}
```

### B. GameManager Safety Update

**File:** `Assets/_Game/_Scripts/Common/GameManager.cs`
**Issue:** `GameManager` unconditionally called `CollisionMatrixManager` during transitions, forcing Ghost creation.
**Fix:** Added `FindObjectOfType` check to prevent access if the manager doesn't exist.

```csharp
if(Object.FindObjectOfType<CollisionMatrixManager>() != null)
    CollisionMatrixManager.Instance.LoadPlayschoolData();
```

---

## 2. Reflection Game Fixes

**Objective:** Resolve memory leaks and persistence issues specific to the Reflection Game.

### A. SunlightSource Memory Leak

**File:** `Assets/_Game/_Scripts/Reflection/SunlightSource.cs`
**Issue:** The script subscribed to `TutorialEventManager` using a lambda expression (`() => SetStartRotation()`) but tried to unsubscribe using a *new* lambda instance. This failed, leaving the listener active forever.
**Fix:** Cached the lambda `Action` to ensure the same reference is used for both subscription and unsubscription.

```csharp
// Subscribing
TutorialEventManager.Instance.Subscribe("event_mirror_info", m_SetStartRotationAction);

// Unsubscribing
TutorialEventManager.Instance.Unsubscribe("event_mirror_info", m_SetStartRotationAction);
```

---

## 3. Gameplay Logic Fixes (Score & Sorting)

**Objective:** Fix "Score Theft", "Phantom Score", and "Free Score" bugs in sorting levels.

### A. Ownership Check (Crayon & Fruit Sorting)

**Files:** `CrayonBox2D.cs`, `FruitCollector.cs`
**Issue:** Items passing through a box's trigger decreed score even if they weren't snapped to that box.
**Fix:** Added strict ownership validation in `OnCollectibleExited`.

```csharp
// Check if the exiting item actually belongs to this box
if (collectible.CurrentSnapPoint != null) {
   // verify ownership logic...
}
```

### B. Logic Reversal & Helper (Crayon Sorting)

**Files:** `CrayonSnapPoint.cs`, `Collectible.cs`
**Fixes:**

* Inverted `HasFlag` check in `CrayonSnapPoint` to correctly validate multi-colored items.
* Added `CurrentSnapPoint` accessor in `Collectible.cs` to support the ownership check.

---

## 4. UI & Input Fixes

**Objective:** Prevent multi-click errors and post-game interaction.

### A. UI Double-Click Prevention

**Files:** `LevelWinUI.cs`, `LevelCompletedPopup.cs`
**Fix:** Added listeners to immediately disable buttons upon click to prevent multiple execution triggering game loop errors.

```csharp
m_NextLevelButton.onClick.AddListener(()=> { m_NextLevelButton.interactable = false; });
```

### B. Input Blocking on Game Over

**Files:** `FruitSelect.cs`, `ShapeSelect.cs`, `CrayonSelect.cs`
**Fix:** Added `GameState` checks to input handlers to ignore clicks when the game is not in the `Playing` state.

```csharp
if (SortingGameManager.Instance.CurrentGameState != GameState.Playing) return;
```
