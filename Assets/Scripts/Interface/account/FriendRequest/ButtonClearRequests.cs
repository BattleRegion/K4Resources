using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonClearRequests : MonoBehaviour
{
    public UILabel Requests;

    void OnClick()
    {
        List<int> Ids = new List<int>();
        foreach (RequestInfo r in UserManager.CurUserInfo.UserRequests)
        {
            Ids.Add(r.RequestId);
        }
        if (Ids.Count > 0)
        {
            FriendControl.RemoveFriendRequest(Ids, (r) =>
            {
                if (r == FriendControl.FriendMessageResult.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        Requests.text = "好友请求已全部清空！";
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        Requests.text = "好友请求清空失败！";
                    });
                }
            });
        }
        else
        {
            Requests.text = "好友请求已全部清空！";
        }
    }
}
