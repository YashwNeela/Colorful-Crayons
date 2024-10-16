using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting
{

    public class CollectibleSelectionButton : MonoBehaviour
    {
        [SerializeField] Image m_Image;
        private CollectibleDataSO m_CurrentCollectibleDataSO;

        CollectibleSelectionUI m_CollectibleSelectionUI;

        protected bool m_IsInteractable;

        void Awake()
        {
            m_Image = GetComponent<Image>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SpawnCollectible()
        {
            if(!m_IsInteractable)
                return;

            Debug.Log("Collectible spawned");
            Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(transform.position);
            
           DraggableUI2D draggable = Instantiate(m_CurrentCollectibleDataSO.collectiblePrefab,
           new Vector3(spawnPoint.x,spawnPoint.y,10),Quaternion.identity)
           .GetComponent<DraggableUI2D>();
            draggable.OnSpawned();

            

            m_CollectibleSelectionUI.OnButtonPressed();
        }

        public void SetData(CollectibleDataSO collectibleDataSO, CollectibleSelectionUI collectibleSelectionUI)
        {
            m_CurrentCollectibleDataSO = collectibleDataSO;

            m_Image.sprite = m_CurrentCollectibleDataSO.collectibleUISprite;

            m_CollectibleSelectionUI = collectibleSelectionUI;
            
        }

        public void ToggleInteractableButton(bool value)
        {
            m_IsInteractable = value;
        }
    }
}
