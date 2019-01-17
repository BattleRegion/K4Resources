using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 控制背包在退出时位置重置
/// </summary>
public class BagReset : MonoBehaviour
{
    public UIScrollView bag;
    public UIGrid bagGrid;

    void RePosition()
    {
        bag.SetDragAmount(0, 0, false);
        bag.UpdateScrollbars();
        Invoke("GridReposition", 0.25f);
    }


    void OnEnable()
    {
        RePosition();
    }

    void GridReposition()
    {
        if (bagGrid != null)
            bagGrid.Reposition();
        bag.ResetPosition();
        bag.SetDragAmount(0, 0, false);
    }

    //void Update()
    //{
    //    if (!bag.isDragging)
    //    {
    //        bag.gameObject.GetComponent<SpringPanel>().enabled = true;
    //    }
    //}
    
}
