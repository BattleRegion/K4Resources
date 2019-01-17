using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainAnime : MonoBehaviour
{
    int BackBarInitX = -800;
    int ListInitX = 800;

    /// <summary>
    /// 返回条和主要界面
    /// </summary>
    public GameObject BackBar;
    public GameObject MainBoard;

    /// <summary>
    /// 气泡提示信息和兔子以及文本
    /// </summary>
    public GameObject Bubble;
    public GameObject Avatar;
    public GameObject InfoText;

    /// <summary>
    /// active时播放动画
    /// </summary>
    void OnEnable()
    {
        //Bubble.SetActive(true);
        //Avatar.SetActive(true);
        //InfoText.SetActive(true);
		if(BackBar != null)
		{
        	AnimationHelper.AnimationMoveTo(new Vector3(-62, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
		}
        AnimationHelper.AnimationMoveTo(new Vector3(0, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, null, null, 0.2f);
    }

    /// <summary>
    /// disable时回到起始位置
    /// </summary>
    void OnDisable()
    {
		if(BackBar != null)
		{
        	BackBar.transform.localPosition = new Vector3(BackBarInitX, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z);
		}
        MainBoard.transform.localPosition = new Vector3(ListInitX, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z);
    }
}
