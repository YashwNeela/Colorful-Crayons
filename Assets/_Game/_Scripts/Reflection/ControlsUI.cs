using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection{

public class ControlsUI : SerializedSingleton<ControlsUI>
{
    [SerializeField] private Dictionary<string, GameObject> controlsDict;

    public void EnableControls(string controlsId)
    {
        try{
        controlsDict[controlsId].gameObject.SetActive(true);
        }
        catch
        {
            Debug.LogError("No controls with Id: " + controlsId+ "Found");
        }
    }

    public void DisableControls(string controlsId)
    {
         try{
        controlsDict[controlsId].gameObject.SetActive(false);
        }
        catch
        {
            Debug.LogError("No controls with Id: " + controlsId+ "Found");
        }
    }
}

public class ControlsUIConstants
{
    public static string movememnt = "controls_movement";
    public static string jump = "controls_jump";

    public static string mirror = "controls_mirror";
}
}
