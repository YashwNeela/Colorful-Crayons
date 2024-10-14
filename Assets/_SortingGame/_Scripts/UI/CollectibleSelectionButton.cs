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
            Debug.Log("Collectible spawned");
           DraggableUI2D draggable = Instantiate(m_CurrentCollectibleDataSO.collectiblePrefab,Camera.main.ScreenToWorldPoint(transform.position),Quaternion.identity)
           .GetComponent<DraggableUI2D>();
            draggable.OnSpawned();
        }

        public void SetData(CollectibleDataSO collectibleDataSO)
        {
            m_CurrentCollectibleDataSO = collectibleDataSO;

            m_Image.sprite = m_CurrentCollectibleDataSO.collectibleUISprite;
            
        }
    }
}
