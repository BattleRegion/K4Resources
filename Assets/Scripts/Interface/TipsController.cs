using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 提示信息
/// </summary>
public class TipsController : MonoBehaviour
{
    public TipsConfig.TipsType type;

    public UILabel TipText;

    public GameObject button_changeTip;

    public Animator TipsAnimation;

    List<TipsData> TipsList;

    void RandomTip()
    {
        TipsList = ConfigManager.TipsConfig.GetTipsByType(type);
        System.Random r = new System.Random();
        int rIndex = r.Next(0, TipsList.Count);
        TipText.text = TipsList[rIndex].TipsText;
    }

    void OnEnable()
    {
        RandomTip();

        UIEventListener.Get(button_changeTip).onClick = (g) => 
        {
            RandomTip();
            TipsAnimation.Play(0);
        };
    }
}