using UnityEngine;
using System.Collections;

public class ButtonRmFriend : MonoBehaviour
{
    public int Uid;
    public UILabel info;

    public GameObject RmButton;
    public GameObject ReturnButton_1;
    public GameObject ReturnButton_2;

    public GameObject ConfirmBoard;

    public GameObject UnitCamera;

    void OnClick()
    {
        if (Uid != null)
        {
            FriendControl.RemoveFriend(Uid, (r) =>
            {
                if (r == FriendControl.FriendMessageResult.Success)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        info.text = "删除好友成功!";
                        ReturnButton_1.SetActive(false);
                        ReturnButton_2.SetActive(true);
                        RmButton.SetActive(false);
                    });
                }
                else
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        info.text = "删除好友失败!";
                        ReturnButton_1.SetActive(false);
                        ReturnButton_2.SetActive(true);
                        RmButton.SetActive(false);
                    });
                }
            });
        }
    }
}
