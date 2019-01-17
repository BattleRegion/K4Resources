using UnityEngine;
using System.Collections;

/// <summary>
/// 强化界面中的强化按钮接口
/// </summary>
public interface _PowerUp
{
    void _OnClickPowerUp();
}

public class ButtonPowerUp : MonoBehaviour
{
    public _PowerUp PowerUpInter;

    void OnClick()
    {
        PowerUpInter._OnClickPowerUp();
    }
}