using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting
{

    public class CollectibleSelectionUI : SerializedSingleton<CollectibleSelectionUI>
    {
        [SerializeField] private ScrollRect m_ScrollRect;

        [SerializeField] CollectibleSelectionButton m_CollectibleSelectionButtonPrefab;

        private List<CollectibleSelectionButton> m_CollectibleSelectionButtonList = new List<CollectibleSelectionButton>();

        [SerializeField] HorizontalLayoutGroup m_HorizontalLayourGroup;

        protected override void Awake()
        {
            base.Awake();
            m_HorizontalLayourGroup = GetComponentInChildren<HorizontalLayoutGroup>();
        }

        public void ClearSelectionUI()
        {
            GameObject content = m_ScrollRect.content.gameObject;
            int contentChildCount = content.transform.childCount;
            CollectibleSelectionButton[] buttons = content.GetComponentsInChildren<CollectibleSelectionButton>();
            foreach (CollectibleSelectionButton b in buttons)
            {
                Destroy(b.gameObject);
            }
        }

        public void AddData(CollectibleDataSO collectibleDataSO, int count, float layoutSpacing)
        {
            m_HorizontalLayourGroup.spacing = layoutSpacing;
            for (int i = 0; i < count; i++)
            {
                CollectibleSelectionButton button = Instantiate(m_CollectibleSelectionButtonPrefab, m_ScrollRect.content.gameObject.transform);

                button.transform.localScale = collectibleDataSO.scale;
                // Set the image for the button
                button.SetData(collectibleDataSO,this);
                button.ToggleInteractableButton(true);

                m_CollectibleSelectionButtonList.Add(button);

                // Get the RectTransform component of the button
                //RectTransform buttonRectTransform = button.GetComponent<RectTransform>();

                // Change the anchor to the left center
                // buttonRectTransform.anchorMin = new Vector2(0.5f, 0.5f); // Left center anchor
                // buttonRectTransform.anchorMax = new Vector2(0.5f, 0.5f); // Left center anchor

                // // Set the anchoredPosition to (0, 0) to place the button at the center of the anchor

                // Debug.Log("Value is " + i);
                // buttonRectTransform.anchoredPosition = new Vector2(i*(400 * (i/2==0?1:-1)), 0); // Adjust Y as needed for vertical positioning

            }
        }

        public void OnButtonPressed()
        {
            StartCoroutine(Co_OnButtonPressed());
        }

        private IEnumerator Co_OnButtonPressed()
        {
            for(int i = 0;i<m_CollectibleSelectionButtonList.Count;i++)
            {
                m_CollectibleSelectionButtonList[i].ToggleInteractableButton(false);
            }

            yield return new WaitForSeconds(1);
            for(int i = 0;i<m_CollectibleSelectionButtonList.Count;i++)
            {
                m_CollectibleSelectionButtonList[i].ToggleInteractableButton(true);
            }
        }
    }
}
