using UnityEngine;
using System.Collections;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class SearchFriendControl : MonoBehaviour, FriendCellInter, itemInterface
{
    public UILabel PlayerUid;
    public UIInput SearchId;

    public GameObject SearchButton;

    public GameObject AddButton;
    public GameObject CancelButton;

    public FriendCell SearchCell;

    public GameObject ResultBoard;
    public UILabel ResultInfo;
    public GameObject ResultReturnButton;

    public SearchDetail SearchDetail;

    public GameObject PetDetail;



    void OnEnable()
    {
        PlayerUid.text = UserManager.CurUserInfo.UserId.ToString();
    }

    void Start()
    {
        UIEventListener.Get(SearchButton).onClick = (g) =>
        {
            if(!string.IsNullOrEmpty(SearchId.value))
            {
                JsonObject args = new JsonObject();
                args.Add("friend_id", SearchId.value);
                SocketCenter.Request(GameRouteConfig.SearchFriend, args, (r) =>
                {
                    if (r.Code == SocketResult.ResultCode.Success)
                    {
                        Debug.Log(r.Data);
                        Loom.QueueOnMainThread(() =>
                        {
                            FriendInfo f = new FriendInfo(r.Data);
                            ShowCell(f);
                            AddButton.GetComponent<ButtonAddFriend>().Uid = f.FriendId;
                            SearchButton.SetActive(false);
                            AddButton.SetActive(true);
                            CancelButton.SetActive(true);
                        });
                    }
                    else
                    {

                    }
                }, null, true, true);
            }
        };

        UIEventListener.Get(CancelButton).onClick = (g) =>
        {
            ResetCellPosition();
            SearchButton.SetActive(true);
            AddButton.SetActive(false);
            CancelButton.SetActive(false);
        };

        UIEventListener.Get(ResultReturnButton).onClick = (g) =>
        {
            ResultBoard.SetActive(false);
        };
    }

    void ShowCell(FriendInfo f)
    {        
        SearchCell.SetFriendCell(f);
        SearchCell.FriendLeader.itemInter = this;
        if (SearchCell.transform.localPosition.x == -5f)
        {
            SearchCell.transform.localPosition = new Vector3(800f, SearchCell.transform.localPosition.y, SearchCell.transform.localPosition.z);
        }
        AnimationHelper.AnimationMoveTo(new Vector3(-5f, SearchCell.transform.localPosition.y, SearchCell.transform.localPosition.z), SearchCell.gameObject, iTween.EaseType.linear, null, null, 0.2f);
        SearchCell.FriendInter = this;
    }

    void ResetCellPosition()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(800f, SearchCell.transform.localPosition.y, SearchCell.transform.localPosition.z), SearchCell.gameObject, iTween.EaseType.linear, null, null, 0.2f);
    }

    /// <summary>
    /// 提示信息
    /// </summary>
    public void SetResultInfo(string text)
    {
        ResultBoard.SetActive(true);
        ResultInfo.text = text;
    }

    void OnDisable()
    {
        SearchId.value = "";
        SearchCell.transform.localPosition = new Vector3(800f, SearchCell.transform.localPosition.y, SearchCell.transform.localPosition.z);
        ResultBoard.SetActive(false);
        SearchButton.SetActive(true);
        CancelButton.SetActive(false);
        AddButton.SetActive(false);
    }

    public void OnClickFriendCell(FriendInfo f)
    {
        SwitchDetailView();
        SearchDetail.SetFriendDetail(f);
    }

    public void OnClickRequestCell(RequestInfo r)
    { }

    public void _OnClickItem(int UPetId)
    {

    }

    public void _OnLongPressItem(int UPetId)
    {
        PetDetail.SetActive(true);
        PetDetail.GetComponent<SetMonsterDetail>().SetDetail(SearchCell.LeaderPet);
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