using UnityEngine;
using TMPro;

/// <summary>
/// The word at the top of the screen. Letters fill in with the produce colour
/// as the player collects that item — the "picture + word" teaching moment.
/// </summary>
public class TargetWordUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wordText;
    [SerializeField] private string emptyHex = "#FFFFFF";

    private string word = "";
    private int filled;
    private int total = 1;

    public void SetWord(string newWord, int targetCount)
    {
        word = newWord == null ? "" : newWord;
        total = Mathf.Max(1, targetCount);
        filled = 0;
        Redraw();
    }

    public void SetProgress(int collected)
    {
        filled = collected;
        Redraw();
    }

    private void Redraw()
    {
        if (wordText == null || string.IsNullOrEmpty(word)) return;

        float pct = Mathf.Clamp01((float)filled / total);
        int litLetters = Mathf.RoundToInt(pct * word.Length);

        string hex = ColorUtility.ToHtmlStringRGB(GameDefs.ColorOf(word));
        string s = "";
        for (int i = 0; i < word.Length; i++)
        {
            string c = (i < litLetters) ? ("#" + hex) : emptyHex;
            s += "<color=" + c + ">" + word[i] + "</color>";
        }
        wordText.text = s;
    }

    public void Celebrate()
    {
        if (wordText != null) StartCoroutine(Pop());
    }

    private System.Collections.IEnumerator Pop()
    {
        float t = 0f;
        Vector3 baseScale = Vector3.one;
        while (t < 0.45f)
        {
            t += Time.deltaTime;
            float k = 1f + Mathf.Sin(Mathf.Clamp01(t / 0.45f) * Mathf.PI) * 0.35f;
            wordText.transform.localScale = baseScale * k;
            yield return null;
        }
        wordText.transform.localScale = baseScale;
    }
}
