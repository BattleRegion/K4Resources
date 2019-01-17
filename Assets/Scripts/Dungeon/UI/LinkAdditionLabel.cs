using UnityEngine;
using System.Collections;

public class LinkAdditionLabel : NumTextureLabel
{
    #region 属性
    public Sprite[] ShineNumSprites;

    bool shine = true;
    #endregion 

    #region 重写父类
	public override void SetNum(string numString, int layer = -1, bool oriStatus = false)
    {
        CancelInvoke("ShineChange");
		base.SetNum(numString, layer, oriStatus);
        Invoke("ShineChange", 0.1f);
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    /// <returns></returns>
    public override int Sort()
    {
        return 999;
    }


    /// <summary>
    /// 数字转换索引
    /// </summary>
    public override int NumToSpriteIndex(string s)
    {
        if (s == ".")
        {
            return 11;
        }
        else if (s == "x")
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
        return LayerHelper.BasicFX;
    }


    public override void Clear()
    {
        CancelInvoke("ShineChange");
        base.Clear();
    }
    /// <summary>
    /// 特殊字符
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public override Vector3 ResetSpecialPosition(GameObject go)
    {
        if (go.name == ".")
        {
            return new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - 7, go.transform.localPosition.z);
        }
        else if (go.name == "x")
        {
            return new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - 4, go.transform.localPosition.z);
        }
        return base.ResetSpecialPosition(go);
    }
    #endregion

    #region AdditionShinie
    void ShineChange()
    {
        if (shine)
        {
            shine = false;
            foreach (SpriteRenderer sr in NumRenders)
            {
                sr.sprite = ShineNumSprites[NumToSpriteIndex(sr.name)];
            }
        }
        else
        {
            shine = true;
            foreach (SpriteRenderer sr in NumRenders)
            {
                sr.sprite = LabelAllSprites[NumToSpriteIndex(sr.name)];
            }
        }
        Invoke("ShineChange",0.1f);
    }
    #endregion
}
