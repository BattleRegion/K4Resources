using UnityEngine;
using System.Collections;

public class AttackComboLabel : NumTextureLabel
{

    #region 属性
    public int CurAttackNum = 0;
    public GameObject ComBoString = null;
    #endregion

    #region 重写父类
	public override void SetNum(string numString, int layer = -1, bool oriStatus = false)
    {
		base.SetNum(numString, layer, oriStatus);
        AnimationRender();
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
        //if (go.name == "c")
        //{
        //    return new Vector3(0, -18, 0);
        //}
        //else
        //{
        //    return new Vector3(go.transform.localPosition.x + 59.5f, go.transform.localPosition.y, go.transform.localPosition.z);
        //}
        return base.ResetSpecialPosition(go);
    }

    /// <summary>
    /// 设置层
    /// </summary>
    public override int Layer()
    {
        return LayerHelper.Top;
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    /// <returns></returns>
    public override int Sort()
    {
        return 1;
    }
    #endregion

    #region 动画渲染
    void AnimationRender()
    {
        ComBoString.SetActive(true);
        AnimationHelper.AnimationFadeTo(1, transform.parent.gameObject, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationScaleTo(new Vector3(2f, 2f, 2), gameObject, iTween.EaseType.easeOutExpo, gameObject, "ComboScaleEnd", 0.1f);
        iTween.FadeFrom(gameObject, 0.5f, 0.2f);
    }

    void ComboScaleEnd()
    {
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), gameObject, iTween.EaseType.easeInExpo, null, null, 0.08f);
    }

    public void SetAttackNum()
    {
        CurAttackNum++;
        SetNum(CurAttackNum.ToString());
    }

    public void SetAddNum(int add)
    {
        CurAttackNum+=add;
        SetNum(CurAttackNum.ToString());
    }

    public void ResertParent()
    {
        }

    public void Out()
    {
        CurAttackNum = 0;
        AnimationHelper.AnimationFadeTo(0, transform.parent.gameObject, iTween.EaseType.easeInExpo, null, null, 0.8f);
    }
    #endregion
}
