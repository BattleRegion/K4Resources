using UnityEngine;
using System.Collections;

public class LinkChainLabel : NumTextureLabel {

    #region 重写父类
	public override void SetNum(string numString, int layer = -1, bool oriStatus = false)
    {
		base.SetNum(numString, layer, oriStatus);
        RenderSet();
    }

    /// <summary>
    /// 数字转换索引
    /// </summary>
    public override int NumToSpriteIndex(string s)
    {
        if (s == "c")
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
    /// 特殊字符
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public override Vector3 ResetSpecialPosition(GameObject go)
    {
        if (go.name == "c")
        {
            return new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - 6.5f, go.transform.localPosition.z);
        }
        return base.ResetSpecialPosition(go);
    }

    /// <summary>
    /// 设置层
    /// </summary>
    public override int Layer()
    {
        return LayerHelper.UnitFX;
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    /// <returns></returns>
    public override int Sort()
    {
        return 999;
    }
    #endregion

    #region 自己的方法
    void RenderSet()
    {
        transform.localScale = new Vector3(1, 1, 1);
        AnimationHelper.AnimationScaleTo(new Vector3(1.3f, 1.3f, 1.3f), gameObject, iTween.EaseType.easeOutExpo, gameObject, "CurComboDetailScaleBigEnd", 0.2f);
        iTween.FadeFrom(gameObject, 0.3f, 0.1f);
    }

    void CurComboDetailScaleBigEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), gameObject, iTween.EaseType.easeInExpo, null, null, 0.15f);
    }
    #endregion
}
