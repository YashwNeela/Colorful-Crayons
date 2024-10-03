using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;

public class Draggable2D : Draggable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Update()
    {
        
    }

     /// <summary>
    /// Called every frame while the mouse is over the GUIElement or Collider.
    /// </summary>
    void OnMouseOver()
    {
        Debug.Log("On MOuse");
    }
}
