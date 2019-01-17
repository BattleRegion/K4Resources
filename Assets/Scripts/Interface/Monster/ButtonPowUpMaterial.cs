using UnityEngine;
using System.Collections;

/// <summary>
/// 怪物强化界面中作为素材的一排按钮接口
/// </summary>
public interface _PowUpMaterial
{
    void _OnClickMaterial();
}

public class ButtonPowUpMaterial : MonoBehaviour 
{
    /// <summary>
    /// 白框
    /// </summary>
    public UISprite whiteSideFrame;

    /// <summary>
    /// 素材上的按钮接口实例
    /// </summary>
    public _PowUpMaterial PowUpMaterialInter;


    void OnClick()
    {
        PowUpMaterialInter._OnClickMaterial();
    }
}
