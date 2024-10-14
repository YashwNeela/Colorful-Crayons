using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting
{

    public class CollectibleSelectionUI : SerializedSingleton<CollectibleSelectionUI>
    {
        [SerializeField] private ScrollRect m_ScrollRect;

        [SerializeField] CollectibleSelectionButton m_CollectibleSelectionButtonPrefab;

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

        public void AddData(CollectibleDataSO collectibleDataSO, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CollectibleSelectionButton button = Instantiate(m_CollectibleSelectionButtonPrefab, m_ScrollRect.content.gameObject.transform);

                // Set the image for the button
                button.SetData(collectibleDataSO);

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
    }
}
