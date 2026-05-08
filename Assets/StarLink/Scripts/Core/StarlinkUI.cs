using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TMKOC.StarLink
{
public class StarlinkUI : SerializedSingleton<StarlinkUI>
{
    public GameObject Container;

    public GameObject HUDUI;

    public Image image;

    public Button nextButton;


    protected override void Awake()
        {
            nextButton.onClick.AddListener(()=>
            {
                StarLinkGameManager.Instance.LoadNextLevel(LevelManager.Instance.CurrentLevelIndex+1);
                Hide();
            });
        }

    public void Show(Sprite img)
        {
            HUDUI.SetActive(false);
            image.sprite = img;
            Container.SetActive(true);
        }

        public void Hide()
        {
            HUDUI.SetActive(true);

            Container.SetActive(false);
            
        }
}
}