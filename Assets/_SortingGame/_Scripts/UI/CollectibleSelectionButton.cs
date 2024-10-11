using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting{

public class CollectibleSelectionButton : MonoBehaviour
{

    [SerializeField] Image m_Image;

    void Awake()
    {
        m_Image = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCollectible()
    {
        Debug.Log("SpawnCollectible");
    }

    public void SetImage(Sprite sprite)
    {
        m_Image.sprite = sprite;
    }
}
}
