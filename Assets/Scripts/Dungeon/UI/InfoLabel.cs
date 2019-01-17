using UnityEngine;
using System.Collections;

public class InfoLabel : NumTextureLabel {
    public float BeginX = 0;

    #region 重写父类

	public override void SetNum(string numString, int layer = -1, bool oriStatus = false)
    {
        if (numString == "") return;
		base.SetNum(numString, layer, oriStatus);
		transform.localPosition = new Vector3(!oriStatus ? (BeginX + TotalWidth/2) : 0f, transform.localPosition.y, transform.localPosition.z);
    }

    /// <summary>
    /// 数字转换索引
    /// </summary>
    public override int NumToSpriteIndex(string s)
    {
        if (s == "/")
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
		if(PvpGameControl.PvpStatus)
		{
        	return LayerHelper.Top;
		}else{
			return LayerHelper.UI;
		}
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    /// <returns></returns>
    public override int Sort()
    {
        return 20;
    }
    #endregion
}
