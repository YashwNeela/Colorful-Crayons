using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.ShapeSorting
{
    public class ShapeCollectible : Collectible
    {
        [SerializeField] protected ShapeType m_ShapeType;

        public ShapeType ShapeType => m_ShapeType;

        Vector3 m_StartPos;

        protected override void Awake()
        {
            base.Awake();
            m_StartPos = transform.position;
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();
            gameObject.SetActive(false);
            gameObject.SetActive(true);

        }


        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollectorOnTriggerEnter(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            HandleCollectorOnTriggerExit(other);
        }

        
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            HandleCollectorOnTriggerEnter(other);
            
        }

        protected override void HandleCollectorOnTriggerEnter(Component collider)
        {
            base.HandleCollectorOnTriggerEnter(collider);
            ShapeCollector collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as ShapeCollector;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as ShapeCollector;

            if (collectorBox != null)
            {
                m_ValidCollector = collectorBox;
            }
        }

        protected override void HandleCollectorOnTriggerExit(Component collider)
        {
            base.HandleCollectorOnTriggerExit(collider);
            m_ValidCollector = null;
            m_CurrentCollector = null;
        }

        protected override void OnPlacedCorrectly()
        {
            base.OnPlacedCorrectly();
            ParticleEffectManager.Instance.PlayParticleEffect(0,transform.position,new Vector3(100,100,100),null);
        }

        protected override void PlaceInCorrectly(Collector collector)
        {
            base.PlaceInCorrectly(collector);

        }


        protected override void HandleDragEnd()
        {
            
            Debug.Log("Handle drag end");
            if(m_IsPlacedInsideCollector)
            {
                draggable.ResetToStartDraggingValues();
                return;
            }
            if (m_CurrentCollector != null && m_CurrentCollector.IsSlotAvailable())
            {
                if (m_ValidCollector != null)
                {
                   
                    if ((m_CurrentCollector as ShapeCollector).ShapeType.HasFlag(m_ShapeType))
                        m_CurrentCollector.SnapCollectibleToCollector(this, () => {
                            OnPlacedCorrectly();
                            });
                    else
                        m_CurrentCollector.SnapCollectibleToCollector(this, () => {});
                    PlaceInCorrectly(m_CurrentCollector);

                }
            }else
            {
                draggable.ResetToStartDraggingValues();

            }
        }

        


        protected override void OnDragStartedStaticAction(Transform transform)
        {
            if (transform == this.transform)
                return;

            if (m_Collider is Collider)
                ((Collider)m_Collider).enabled = false;
            if (m_Collider is Collider2D)
                ((Collider2D)m_Collider).enabled = false;
        }

        protected override void OnDragEndStaticAction(Transform transform)
        {
            if (transform == this.transform)
                return;

            if (m_Collider is Collider)
                ((Collider)m_Collider).enabled = true;
            if (m_Collider is Collider2D)
                ((Collider2D)m_Collider).enabled = true;
        }

        
        
    }
}
