using System.Collections;
using System.Collections.Generic;
using TMKOC;
using UnityEngine;

namespace TMKOC.Reflection{
    [System.Serializable]
    public enum ReflectionTagsEnum
    {
        Mirror
    }
public class ReflectionTags : Tags
{
    public ReflectionTagsEnum m_Tag;
}
}
