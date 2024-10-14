using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class DraggableUI2D : Draggable2D
    {
        public override void Update()
        {
             if (m_isDragging && m_CanDrag)
            {
                DragObject();
            }
            if(m_isDragging && Input.GetMouseButtonUp(0))
                Destroy(gameObject);
        }

        public void OnSpawned()
        {
            m_isDragging = true;
            m_CanDrag = true;
            StartDragging();
            
        }
        
    }
}
