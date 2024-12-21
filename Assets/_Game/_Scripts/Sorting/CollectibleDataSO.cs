using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace TMKOC.Sorting{

[CreateAssetMenu(menuName = "ScriptableObject/Collectible")]
public class CollectibleDataSO : SerializedScriptableObject
{
    public Collectible collectiblePrefab;

    public Sprite collectibleUISprite;

    public Vector3 scale;
}
}
