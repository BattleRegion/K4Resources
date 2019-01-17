using UnityEngine;
using System.Collections;

public class EnemyBeHitLabel : NumTextureLabel {
    bool hasResertPosition = false;

    #region 重写父类
	public override void SetNum(string numString, int layer = -1, bool oriStatus = false)
    {
        if (numString != "0h")
        {
			base.SetNum(numString, layer, oriStatus);
        }
        else
        {
            NumString = numString;
        }
        if (!hasResertPosition)
        {
            hasResertPosition = true;
            transform.localScale = new Vector3(transform.localScale.x / transform.parent.localScale.x, transform.localScale.y / transform.parent.localScale.y, transform.localScale.z / transform.parent.localScale.z);
        }
        AnimationHitNum();
    }

    /// <summary>
    /// 数字转换索引
    /// </summary>
    public override int NumToSpriteIndex(string s)
    {
        if (s == "h")
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
        if (go.name == "h")
        {
            return new Vector3(go.transform.localPosition.x, go.transform.localPosition.y -4.7f, go.transform.localPosition.z);
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
        return 1;
    }
    #endregion

    #region 私有方法
    void AnimationHitNum()
    {
        gameObject.SetActive(true);
        if (NumString == "0h")
        {
            //消失
            Hashtable args = new Hashtable();
            args.Add("scale", new Vector3(2.2f / transform.parent.localScale.x, 2.2f / transform.parent.localScale.y, 2.2f / transform.parent.localScale.z));
            args.Add("time", 0.25f);
            args.Add("oncompletetarget", gameObject);
            args.Add("oncomplete", "ScaleEnd");
            iTween.ScaleTo(gameObject, args);
            iTween.FadeTo(gameObject, 0, 0.25f);
        }
        else
        {
            foreach (SpriteRenderer sr in NumRenders)
            {
                GameObject go = sr.gameObject;
                if (go.name != "h")
                {
                    iTween.FadeFrom(go, 0.3f, 0.16f);
                    Hashtable args = new Hashtable();
                    args.Add("amount", new Vector3(0, 0.04f, 0));
                    args.Add("time", 0.08f);
                    args.Add("oncompletetarget", gameObject);
                    args.Add("oncomplete", "MoveEnd");
                    args.Add("oncompleteparams", go);
                    iTween.MoveBy(go, args);
                }
            }
        }
    }

    void MoveEnd(GameObject go)
    {
        Hashtable args = new Hashtable();
        args.Add("amount", new Vector3(0, -0.04f, 0));
        args.Add("time", 0.08f);
        iTween.MoveBy(go, args);
    }


    void ScaleEnd()
    {
        transform.localScale = new Vector3(0.8f / transform.parent.localScale.x, 0.8f / transform.parent.localScale.y, 0.8f / transform.parent.localScale.z);
        gameObject.SetActive(false);
    }
    #endregion
}
