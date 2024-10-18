using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.CarSorting
{
    public class CarTire : Collectible
    {
        [SerializeField] protected CarType m_CarType;

        public CarType CarType => m_CarType;

        protected bool m_CanDestroy = true;

        protected Coroutine CanDestoryRef;



        protected override void Start()
        {
            base.Start();
         //   CanDestoryRef = StartCoroutine(Co_Destroy());
        }
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollectorOnTriggerEnter(other);
        }

        /// <summary>
        /// Sent each frame where another object is within a trigger collider
        /// attached to this object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            Debug.Log("Collider name " + other.name);

            HandleCollectorOnTriggerStay(other);
        }
        protected override void HandleCollectorOnTriggerEnter(Component collider)
        {
            base.HandleCollectorOnTriggerEnter(collider);
            Car collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as Car;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as Car;

            if (collectorBox != null)
            {
                // if (collectorBox.CrayonColor.HasFlag(this.CrayonColor))
                // {
                //     m_ValidCollector = collectorBox;
                // }
                // else
                //     m_IsTryingToPlaceWrong = true;
                m_ValidCollector = collectorBox;
            }
        }
        protected override void OnGameStart()
        {
            base.OnGameStart();
            Destroy(gameObject);
        }
        protected void BaseHandleCollectorOnTriggerEnter(Component collider)
        {
            base.HandleCollectorOnTriggerEnter(collider);

        }
        

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            HandleCollectorOnTriggerExit(other);
        }

        protected override void HandleCollectorOnTriggerExit(Component collider)
        {
            Debug.Log("Collider Exited " + collider.name);
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
        protected override void HandleDragStart()
        {
            base.HandleDragStart();
            if(CanDestoryRef != null)
                StopCoroutine(CanDestoryRef);
        }

        protected override void HandleDragEnd()
        {
            if (m_IsPlacedInsideCollector)
            {
              //  draggable.ResetToStartDraggingValues();
                draggable.ResetToPointValues(m_CurrentSnapPoint.transform.position);
                return;
            }
            if (m_CurrentCollector != null && m_CurrentCollector.IsSlotAvailable())
            {
                if (m_ValidCollector != null)
                {

                    if ((m_CurrentCollector as Car).CarType.HasFlag(m_CarType))
                        m_CurrentCollector.SnapCollectibleToCollector(this, () => OnPlacedCorrectly());
                    else
                        m_CurrentCollector.SnapCollectibleToCollector(this, () => { });
                    PlaceInCorrectly(m_CurrentCollector);

                }


            }else
            {
                // m_CanDestroy = true;
                // CanDestoryRef = StartCoroutine(Co_Destroy());
                
            }
        }

       protected IEnumerator Co_Destroy()
        {
            yield return new WaitForSeconds(2);
            gameObject.SetActive(false);
           // Destroy(gameObject);
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

        public void OnCrossButtonPressed()
        {
            //m_CurrentSnapPoint.ResetSnapPoint();
            if(m_CurrentCollector != null)
                m_CurrentCollector.OnCollectibleExited(this);
            ParticleEffectManager.Instance.PlayParticleEffect(1,transform.position,new Vector3(200,200,200),null);
            Destroy(gameObject);
        }

        protected override void OnLevelCompleteCheck()
        {
            base.OnLevelCompleteCheck();
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }
    }
}
