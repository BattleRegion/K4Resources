using UnityEngine;
using System.Collections;

public interface AddMonster
{
    void _OnClickBlank(int position);
    void _OnLongPressBlank(int position);
}

public class ButtonAddMonster : MonoBehaviour 
{
    public int position;
    public AddMonster blankInter;

    #region 点击事件判定
    float pressTime = 0f;
    bool press = false;
    public float longPressTime = 1f;

    void OnClick() //短按功能
    {
        blankInter._OnClickBlank(position);
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            press = true;
        }
        else
        {
            pressTime = 0f;
            press = false;
        }
    }


    void Update()   //计时实现按钮长按功能
    {
        if (press)
        {
            pressTime += Time.deltaTime;
            if (pressTime > longPressTime)
            {
                blankInter._OnLongPressBlank(position);
                pressTime = 0f;
                press = false;
            }
        }
    }
    #endregion
}
