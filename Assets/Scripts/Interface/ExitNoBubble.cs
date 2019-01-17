using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitNoBubble : MonoBehaviour
{
    /// <summary>
    /// 当前场景
    /// </summary>
    public GameObject CurView;

    /// <summary>
    /// 目标场景
    /// </summary>
    public GameObject TargetView;

    /// <summary>
    /// 返回条（用于动画退出）
    /// </summary>
    public GameObject BackBar;

    /// <summary>
    /// 主要界面（动画退出）
    /// </summary>
    public GameObject MainBoard;

    /// <summary>
    /// 搭载oncomplete的按钮
    /// </summary>
    public GameObject CurButton;

    void OnClick()
    {
        if(CurView.GetComponent<PartyInfo>() != null)
        {
            CurView.GetComponent<PartyInfo>().partyLock = true;
        }
        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, CurButton, "SceneSwitch", 0.2f);
    }



    public void SceneSwitch()
    {
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
        CurView.SetActive(false);
    }

    void Update()
    {
        if (MainButtons.exit)
        {
            AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
            AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, null, null, 0.2f);
            MainButtons.exit = false;
        }
    }

    //    public List<MainButtons> mainButtons = new List<MainButtons>();
    /*    public void Handler()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, null, null, 0.2f);
    }*/
    /*    void Start()
    {
        foreach (MainButtons mb in mainButtons)
        {
            mb.inter = this;
        }
    }*/

}
