using UnityEngine;
using UnityEngine.UI;


namespace TMKOC.Sorting
{
    [System.Serializable]
    public struct GridItemData
    {
        public Sprite Sprite;
        public bool IsCorrect;

        public GridItemData(Sprite sprite, bool isCorrect)
        {
            Sprite = sprite;
            IsCorrect = isCorrect;
        }
    }

    public class Grid_ItemController : MonoBehaviour
    {
        [SerializeField] private Image _imageSprite;
        [SerializeField] private GameObject _correctOption;
        [SerializeField] private GameObject _incorrectOption;

        public void SetItem(GridItemData gridItemData)
        {
            _imageSprite.sprite = gridItemData.Sprite;
            if (gridItemData.IsCorrect)
            {
                _correctOption.SetActive(true);
                _incorrectOption.SetActive(false);
            }
            else
            {
                _correctOption.SetActive(false);
                _incorrectOption.SetActive(true);
            }
        }
    }
}
