using UnityEngine;
using System.Collections;

public class SwitchState : MonoBehaviour
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

    public bool ExitCurView = false;

    void OnClick()
    {
        if (ExitCurView)
        {
            AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
            AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
        }
        else
        {
            if (TargetView != null)
            {
                TargetView.SetActive(true);
            }
        }
    }



    public void SceneSwitch()
    {
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
        CurView.SetActive(false);
    }

}
