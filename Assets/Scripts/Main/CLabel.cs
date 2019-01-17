using UnityEngine;
using System.Collections;

public class CLabel :NumTextureLabel  {

    /// <summary>
    /// 数字转换索引
    /// </summary>
    public override int NumToSpriteIndex(string s)
    {
        if (s == ":")
        {
            return 10;
        }
        else
        {
            return int.Parse(s);
        }
        return 0;
    }


    /// <summary>
    /// 设置层
    /// </summary>
    public override int Layer()
    {
        return LayerHelper.UIFX;
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    /// <returns></returns>
    public override int Sort()
    {
        return 10;
    }
}
