using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendListControl : MonoBehaviour, FriendCellInter
{
    public UIGrid FriendGrid;
    public DungeonListAnime ListAnime;
    public GameObject FriendListItem;
    public FriendDetail FriendInfo;

    public void AddFriendItem(FriendInfo f)
    {
        GameObject menuItem;
        menuItem = NGUITools.AddChild(FriendGrid.gameObject, FriendListItem);
        ListAnime.cells.Add(menuItem);

        FriendCell cell = menuItem.GetComponent<FriendCell>();
        cell.SetFriendCell(f);
        cell.FriendInter = this;
    }

    public GameObject ListMask_1;
    public GameObject ListMask_2;

    void OnEnable()
    {
		ListMask_1.transform.localPosition = new Vector3 (0.2f, 0.85f, -0.5f);
		ListMask_2.transform.localPosition = new Vector3 (0.2f, -1.13f, -0.5f);
        ListMask_1.SetActive(true);
        ListMask_2.SetActive(true);
        FriendControl.GetFriendList((result) =>
        {
            if (result == FriendControl.FriendMessageResult.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    foreach (FriendInfo f in UserManager.CurUserInfo.UserFriends)
                    {
                        AddFriendItem(f);
                    }
                    FriendGrid.Reposition();
                    ListAnime.showCells();
                });
            }
            else
            {

            }
        });
    }

    void OnDisable()
    {
        ListMask_1.SetActive(false);
        ListMask_2.SetActive(false);
        foreach (GameObject g in ListAnime.cells)
        {
            GameObject rm = g;
            Destroy(rm);
        }
        ListAnime.cells.Clear();
    }

    public void OnClickFriendCell(FriendInfo f)
    {
        FriendInfo.SetFriendDetail(f);
        SwitchDetailView();
    }

    public void OnClickRequestCell(RequestInfo R)
    { }

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

    void SwitchDetailView()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
    }

    void SceneSwitch()
    {
        CurView.SetActive(false);
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
    }
}
