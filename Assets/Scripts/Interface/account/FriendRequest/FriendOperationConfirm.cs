using UnityEngine;
using System.Collections;

public class FriendOperationConfirm : MonoBehaviour
{
    public GameObject UnitCamera;
    public UILabel info;
    public GameObject RmButton;
    public GameObject ReturnButton_1;
    public GameObject ReturnButton_2;
    public GameObject ConfirmBoard;

    void OnEnable()
    {
        UnitCamera.SetActive(false);
    }

    void OnDisable()
    {
        UnitCamera.SetActive(true);
        info.text = "确定删除好友吗？";
        if (RmButton != null)
        {
            RmButton.SetActive(true);
        }
        if (ReturnButton_1 != null)
        {
            ReturnButton_1.SetActive(true);
        }
        if (ReturnButton_2 != null)
        {
            ReturnButton_2.SetActive(false);
        }
        ConfirmBoard.SetActive(false);
    }
}
