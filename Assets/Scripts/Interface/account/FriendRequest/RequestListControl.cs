using UnityEngine;
using System.Collections;

public class RequestListControl : MonoBehaviour, FriendCellInter
{
    public UIGrid RequestGrid;
    public DungeonListAnime ListAnime;
    public GameObject RequestListItem;
    public RequestDetail RequestInfo;

    public void AddFriendItem(RequestInfo r)
    {
        GameObject menuItem;
        menuItem = NGUITools.AddChild(RequestGrid.gameObject, RequestListItem);
        ListAnime.cells.Add(menuItem);

        FriendCell cell = menuItem.GetComponent<FriendCell>();
        cell.SetFriendCell(r);
        cell.RequestInter = this;
    }

    public GameObject ListMask_1;
    public GameObject ListMask_2;

    void OnEnable()
	{
		ListMask_1.transform.localPosition = new Vector3 (0.2f, 0.85f, -0.5f);
		ListMask_2.transform.localPosition = new Vector3 (0.2f, -1.13f, -0.5f);
        ListMask_1.SetActive(true);
        ListMask_2.SetActive(true);
        FriendControl.GetAllRequests((result) =>
        {
            if (result == FriendControl.FriendMessageResult.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    foreach (RequestInfo r in UserManager.CurUserInfo.UserRequests)
                    {
                        AddFriendItem(r);
                    }
                    RequestGrid.Reposition();
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
    }

    public void OnClickRequestCell(RequestInfo r)
    {
        RequestInfo.SetRequestDetail(r);
        SwitchDetailView();
    }


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
