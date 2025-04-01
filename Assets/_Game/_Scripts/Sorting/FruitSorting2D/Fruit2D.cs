using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class Fruit2D : Collectible
    {

        [SerializeField] private FruitType m_FruitType;

        public FruitType FruitType => m_FruitType;

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollectorOnTriggerEnter(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            HandleCollectorOnTriggerExit(other);
        }

        protected override void HandleCollectorOnTriggerEnter(Component collider)
        {
            base.HandleCollectorOnTriggerEnter(collider);
            FruitCollector collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as FruitCollector;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as FruitCollector;

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

        }
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            Debug.Log("Collider name " + other.name);

            HandleCollectorOnTriggerEnter(other);
        }

        protected override void PlaceInCorrectly(Collector collector)
        {
            base.PlaceInCorrectly(collector);

        }


        protected override void HandleDragStart()
        {
            this.GetComponent<SpriteRenderer>().sortingOrder = 101;


        }
        protected override void HandleDragEnd()
        {
            if (m_IsPlacedInsideCollector)
            {
                draggable.ResetToStartDraggingValues();
                return;
            }
            this.GetComponent<SpriteRenderer>().sortingOrder = 0;

            if (m_IsPlacedInsideCollector)
            {
                draggable.ResetToStartDraggingValues();
                return;
            }
            if (m_CurrentCollector != null)
            {
                if (m_ValidCollector != null)
                {
                    if (m_ValidCollector.IsSlotAvailable())
                    {
                        if ((m_CurrentCollector as FruitCollector).FruitType.HasFlag(m_FruitType))
                            m_CurrentCollector.SnapCollectibleToCollector(this, () => OnPlacedCorrectly());
                        else
                            m_CurrentCollector.SnapCollectibleToCollector(this, () => { });
                        PlaceInCorrectly(m_CurrentCollector);
                    }
                    else
                    {
                        draggable.ResetToStartDraggingValues();

                        m_ValidCollector = null;
                        m_CurrentCollector = null;
                    }
                }
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

        // 

        private bool m_IsSelected;

        private void OnMouseDown()
        {
            m_IsSelected = m_IsSelected ? false : true;

            if (m_IsSelected)
                FruitSelected();
            else
                FruitUnselected();
        }

        private void FruitSelected() { }
        private void FruitUnselected() { }
    }

}