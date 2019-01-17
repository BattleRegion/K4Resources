using UnityEngine;
using System.Collections;

public class ButtonAcceptRequest : MonoBehaviour
{
    public int RequestId = -1;

    public GameObject ConfirmBoard;
    public UILabel ConfirmInfo;

    void OnClick()
    {
        if (RequestId != -1)
        {
            FriendControl.ReplyFriendRequest(RequestId, true, (r) =>
            {
                if (r == FriendControl.FriendMessageResult.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        ConfirmBoard.SetActive(true);
                        ConfirmInfo.text = "请求已接受";
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        ConfirmBoard.SetActive(true);
                        ConfirmInfo.text = "网络错误";
                    });
                }
            });
        }
    }

    void OnDisable()
    {
        ConfirmBoard.SetActive(false);
    }
}
