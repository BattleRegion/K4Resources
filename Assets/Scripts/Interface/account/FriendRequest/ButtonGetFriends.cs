using UnityEngine;
using System.Collections;

public class ButtonGetFriends : MonoBehaviour
{
    public UILabel FriendList;

    void OnClick()
    {
        FriendControl.GetFriendList((r) =>
        {
            if (r == FriendControl.FriendMessageResult.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (UserManager.CurUserInfo.UserFriends.Count == 0)
                    {
                        FriendList.text = "空";
                    }
                    else
                    {
                        foreach (FriendInfo f in UserManager.CurUserInfo.UserFriends)
                        {
                            FriendList.text += "[Uid: " + f.FriendId + "]";
                        }
                    }
                });
            }
            else
            {
                Loom.QueueOnMainThread(() =>
                {
                    FriendList.text = "获取好友失败！";
                });
            }
        });
    }
}
