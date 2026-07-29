using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private float laneChangeSpeed = 12f;
    [SerializeField] private bool useKeyboardFallback = true;

    private int currentLaneIndex = 1;
    private float targetY;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (inputManager == null) inputManager = FindObjectOfType<InputManager>();
        if (laneManager == null) laneManager = FindObjectOfType<LaneManager>();
    }

    private void OnEnable()
    {
        if (inputManager != null)
        {
            inputManager.OnSwipeUp += MoveToUpperLane;
            inputManager.OnSwipeDown += MoveToLowerLane;
        }
    }

    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnSwipeUp -= MoveToUpperLane;
            inputManager.OnSwipeDown -= MoveToLowerLane;
        }
    }

    private void Start()
    {
        if (laneManager != null)
        {
            targetY = laneManager.GetLanePosition(currentLaneIndex).y;
            Vector3 p = transform.position;
            p.y = targetY;
            transform.position = p;
        }
    }

    private void Update()
    {
        if (useKeyboardFallback)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) MoveToUpperLane();
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) MoveToLowerLane();
        }

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, laneChangeSpeed * Time.deltaTime);
        transform.position = pos;
    }

    private void MoveToUpperLane()
    {
        if (currentLaneIndex > 0) { currentLaneIndex--; UpdateTargetY(); }
    }

    private void MoveToLowerLane()
    {
        if (currentLaneIndex < 2) { currentLaneIndex++; UpdateTargetY(); }
    }

    private void UpdateTargetY()
    {
        if (laneManager != null) targetY = laneManager.GetLanePosition(currentLaneIndex).y;
    }

    public int GetCurrentLaneIndex() { return currentLaneIndex; }
}
