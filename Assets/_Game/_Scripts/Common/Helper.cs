using System;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public static class Helper
{
    public  static void TypeWriterAnimation(TextMeshProUGUI tmp,string value, float speed, Action callback = null)
    {
        string text = "";

        DOTween.To(()=> text, x=> text= x, value, value.Length/speed).OnUpdate(()=>
        {
            tmp.text = text;
            
        })
        .OnComplete(()=>
        {
            callback?.Invoke();
            

        });
    }

    public  static void TypeWriterAnimation(Text tmp,string value, float speed, Action callback = null)
    {
        string text = "";

        DOTween.To(()=> text, x=> text= x, value, value.Length/speed).OnUpdate(()=>
        {
            tmp.text = text;
        })
        .OnComplete(()=>
        {
            callback?.Invoke();
        });
    }
}
