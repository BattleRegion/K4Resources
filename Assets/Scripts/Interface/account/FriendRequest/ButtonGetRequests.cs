using UnityEngine;
using System.Collections;

public class ButtonGetRequests : MonoBehaviour
{
    public UILabel PlayerUid;

    void OnEnable()
    {
        PlayerUid.text = "Your ID: " + UserManager.CurUserInfo.UserId.ToString();
    }


    public UILabel Requests;

    void OnClick()
    {
        Requests.text = "Requests: ";
        FriendControl.GetAllRequests((r) =>
        {
            if (r == FriendControl.FriendMessageResult.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (UserManager.CurUserInfo.UserRequests.Count == 0)
                    {
                        Requests.text = "空";
                    }
                    else
                    {
                        foreach (RequestInfo rInfo in UserManager.CurUserInfo.UserRequests)
                        {
                            Requests.text += "[" + rInfo.RequestId.ToString() + "]";
                        }
                    }
                });
            }
            else
            {
                Loom.QueueOnMainThread(() =>
                {

                });
            }
        });
    }
}
