using UnityEngine;
using System.Collections;

public class ButtonAddFriend : MonoBehaviour
{
    public int Uid = -1;

    public GameObject ResultBoard;
    public UILabel Info;

    void OnClick()
    {
        if (Uid != -1)
        {
            FriendControl.SendFriendRequest(Uid, (r) =>
            {
                if (r == FriendControl.FriendMessageResult.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        ResultBoard.SetActive(true);
                        Info.text = "申请已发送";
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        ResultBoard.SetActive(true);
                        Info.text = "申请发送失败";
                    });
                }
            });
        }
    }



    void OnDisable()
    {
        ResultBoard.SetActive(false);
    }
}
