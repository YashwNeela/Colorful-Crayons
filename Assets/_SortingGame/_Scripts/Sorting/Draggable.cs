using UnityEngine;
using System;

namespace TMKOC.Sorting
{
    public class Draggable : MonoBehaviour
    {
        protected Camera m_Camera;
        protected Rigidbody m_Rigidbody;

        public bool m_CanDrag;
        protected bool m_isDragging = false;
        protected bool m_hasDragStarted = false;
        protected float m_ZPosition;

        public bool HasDragStarted => m_hasDragStarted;
        public bool IsDragging => m_isDragging;

        public Action OnDragStarted;
        public static Action<Transform> OnDragStartedStaticAction;
        public Action OnDragging;
        public static Action<Transform> OnDraggingStaticAction;
        public Action OnDragEnd; // Event to notify when dragging ends
        public static Action<Transform> OnDragEndStaticAction;


        protected Component m_Collider;
        public float screenLeft = 0.0f;
        public float screenRight = 0.0f;


        public float GroundLevel = 0.0f;

        public Vector3 m_DragStartPos;

        public Quaternion m_DragStartRotation;

        public Vector3 m_DragStartScale;


        void Awake()
        {
            m_Camera = Camera.main;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            if (m_Collider == null)
            {
                m_Collider = GetComponent<Collider2D>();
            }
        }

        public virtual void Update()
        {
            // Check for mouse button down to start dragging
            if (Input.GetMouseButtonDown(0) && m_CanDrag)
            {
                StartDragging();
            }
            // Handle the dragging when the mouse is held down
            if (m_isDragging && m_CanDrag)
            {
                DragObject();
            }
            // Check for mouse button up to stop dragging
            if (m_isDragging && Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }

        protected virtual void StartDragging()
        {
            // Raycast to detect if the mouse is over this object
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == m_Collider)
            {


                m_ZPosition = transform.position.z;
                HandleStartDragging();


            }
        }

        protected virtual void HandleStartDragging()
        {
            if (SortingGameManager.Instance.CurrentGameState != GameState.Playing)
                return;

            if (m_Collider is Collider)
                ((Collider)m_Collider).isTrigger = true;
            else if (m_Collider is Collider2D)
                ((Collider2D)m_Collider).isTrigger = true;

            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = true;
            }
            m_isDragging = true;
            m_hasDragStarted = true;

            m_DragStartPos = transform.position;
            m_DragStartRotation = transform.rotation;
            m_DragStartScale = transform.lossyScale;

            OnDragStarted?.Invoke();
            OnDragStartedStaticAction?.Invoke(transform);
        }

        public void ResetToStartDraggingValues()
        {
            transform.position = m_DragStartPos;
            m_DragStartRotation = transform.rotation;
            m_DragStartScale = transform.lossyScale;
        }

        public void ResetToPointValues(Vector3 position)
        {
            transform.position = position;
        }

        protected virtual void DragObject()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = m_Camera.WorldToScreenPoint(transform.position).z;
            Vector3 worldPosition = m_Camera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = m_ZPosition;

            // Ensure the object doesn't go below the ground level
            if (worldPosition.y < GroundLevel)
            {
                worldPosition.y = GroundLevel;
            }

            if (worldPosition.x < screenLeft)
            {
                worldPosition.x = screenLeft;
            }

            if (worldPosition.x > screenRight)
            {
                worldPosition.x = screenRight;
            }

            transform.position = worldPosition;

            OnDragging?.Invoke();
            OnDraggingStaticAction?.Invoke(transform);
        }

        protected virtual void StopDragging()
        {
            HandleStopDragging();
        }

        protected virtual void HandleStopDragging()
        {
            m_isDragging = false;
            m_hasDragStarted = false;

            if (m_Collider is Collider)
                ((Collider)m_Collider).isTrigger = false;
            else if (m_Collider is Collider2D)
                ((Collider2D)m_Collider).isTrigger = false;

            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = false;
            }

            // Trigger the drag end event
            OnDragEnd?.Invoke();
            OnDragEndStaticAction?.Invoke(transform);
        }

        public void HandleRigidbodyKinematic(bool value)
        {
            m_Rigidbody.isKinematic = value;
        }
    }
}
