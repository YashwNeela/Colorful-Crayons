using UnityEngine;

namespace TMKOC.Cases_of_Popatlal.Tutorial{

public class TutorialHighlighter : MonoBehaviour
{
    private GameObject currentTarget;

    public void Highlight(GameObject target)
    {
        currentTarget = target;

        RectTransform targetRect = target.GetComponent<RectTransform>();

        
    }

    public void ClearHighlight()
    {
        currentTarget = null;

    }

    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }
}

}