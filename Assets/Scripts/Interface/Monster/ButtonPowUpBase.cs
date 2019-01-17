using UnityEngine;
using System.Collections;

/// <summary>
/// 强化界面中被强化物体位置的按钮接口
/// </summary>
public interface _PowUpBase
{
    void _OnClickBase();
}

public class ButtonPowUpBase : MonoBehaviour 
{
    /// <summary>
    /// 接口实例
    /// </summary>
    public _PowUpBase PowUpBaseIner;

    /// <summary>
    /// 点击之后的白框
    /// </summary>
    public UISprite whiteSideFrame;

    void OnClick()
    {
        //if (whiteSideFrame.gameObject != null)
        //{
        //    whiteSideFrame.gameObject.SetActive(true);
        //}
        PowUpBaseIner._OnClickBase();
    }
}
