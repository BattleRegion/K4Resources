using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PartyAnime : MonoBehaviour 
{
    int BackBarInitX = -800;
    int ListInitX = 800;

    /// <summary>
    /// 返回条
    /// </summary>
    public GameObject BackBar;

    /// <summary>
    /// 主面板
    /// </summary>
    public GameObject Board;

    /// <summary>
    /// 入场动画
    /// </summary>
    void OnEnable()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-62, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(0, Board.transform.localPosition.y, Board.transform.localPosition.z), Board, iTween.EaseType.linear, gameObject, "ActionEnd", 0.2f);
    }

    public static Action moveEnd = null;

    void ActionEnd()
    {
        if(moveEnd != null)
        {
            PartyAnime.moveEnd();
            PartyAnime.moveEnd = null;
        }
        Invoke("UnloadAssets", 0.5f);
    }

    void UnloadAssets()
    {
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 出场时回到最初位置
    /// </summary>
    void OnDisable()
    {
        BackBar.transform.localPosition = new Vector3(BackBarInitX, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z);
        Board.transform.localPosition = new Vector3(ListInitX, Board.transform.localPosition.y, Board.transform.localPosition.z);
    }
}