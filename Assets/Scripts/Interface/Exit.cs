using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour
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

    public GameObject Bubble;

    public GameObject Avatar;

    public GameObject InfoText;

    public GameObject CurButton;

    void OnClick()
    {
		if(BackBar != null)
		{
        	AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
		}
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, CurButton, "SceneSwitch", 0.2f);
        //Bubble.SetActive(false);
        //Avatar.SetActive(false);

    }
    void SceneSwitch()
    {
        CurView.SetActive(false);
        if(TargetView != null)
        {
            TargetView.SetActive(true);
        }
    }

    void Update()
    {
        if (MainButtons.exit)
        {
            MainButtons.exit = false;
			if(BackBar != null)
			{
            	AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
			}
            AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, null, null, 0.2f);
            //Bubble.SetActive(false);
            //Avatar.SetActive(false);
        }
    }
}
