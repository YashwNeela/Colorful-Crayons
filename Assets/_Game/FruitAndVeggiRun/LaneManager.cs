using UnityEngine;

public class LaneManager : MonoBehaviour
{
    [SerializeField] private float laneHeight = 2f;
    [SerializeField] private float laneSpacing = 2f;
    
    private int currentLaneIndex = 1; // Middle lane (0 = top, 1 = middle, 2 = bottom)
    private Vector3[] lanePositions;
    
    private void Awake()
    {
        InitializeLanes();
    }
    
    private void InitializeLanes()
    {
        lanePositions = new Vector3[3];
        
        // Setup 3 lanes (top, middle, bottom)
        // Middle lane is at Y = 0
        lanePositions[0] = new Vector3(0, laneSpacing, 0);      // Upper lane
        lanePositions[1] = new Vector3(0, 0, 0);                // Middle lane
        lanePositions[2] = new Vector3(0, -laneSpacing, 0);     // Lower lane
        
        Debug.Log("Lanes initialized: Top=" + lanePositions[0].y + ", Middle=" + lanePositions[1].y + ", Bottom=" + lanePositions[2].y);
    }
    
    public Vector3 GetLanePosition(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex > 2)
        {
            Debug.LogWarning("Invalid lane index: " + laneIndex);
            return lanePositions[1];
        }
        return lanePositions[laneIndex];
    }
    
    public int GetCurrentLaneIndex()
    {
        return currentLaneIndex;
    }
    
    public void SetCurrentLane(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex <= 2)
        {
            currentLaneIndex = laneIndex;
        }
    }
    
    public int GetLaneCount()
    {
        return 3;
    }
    
    // Get random lane index
    public int GetRandomLane()
    {
        return Random.Range(0, 3);
    }
}
