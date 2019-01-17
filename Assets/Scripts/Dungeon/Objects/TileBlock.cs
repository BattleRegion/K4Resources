using UnityEngine;
using System.Collections;

public class TileBlock : DungeonObject
{
    public float CurAddition;

    #region  重写父类
    /// <summary>
    /// 设置精灵
    /// </summary>
    /// <param name="spritePath"></param>
    public override void SetSprite()
    {
        Sprite s = Resources.Load<Sprite>(DungeonSpritePathManager.TilePath);
        ObjectSprite.sprite = s;
        base.SetSprite();
    }

    /// <summary>
    /// 设置名字
    /// </summary>
    public override void SetObjectName()
    {
        name = "Tile:" + XPosition + "," + YPosition;
    }
    #endregion
}
