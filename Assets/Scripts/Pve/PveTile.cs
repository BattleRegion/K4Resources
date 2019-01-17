using UnityEngine;
using System.Collections;

public class PveTile : PveGameObject
{
    #region 属性
    public bool CanMoveOn = true;
    public bool CanMoveOnWithBoss = true;
    #endregion

    #region 重写
    public override void SetName()
    {
        name = "Tile:" + XPosition + "," + YPosition;
    }

    public override void SetOrder()
    {
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 1;
    }
    #endregion
}
