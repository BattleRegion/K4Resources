using UnityEngine;
using System.Collections;

public class EnergyJudge : MonoBehaviour
{
    public GameObject ResultBoard;
    public UILabel ResultInfo;

    void OnEnable()
    {
        /*if (UserManager.CurUserInfo.Energy == UserManager.CurUserInfo.EnergyLimit)
        {
            ResultBoard.SetActive(true);
            ResultInfo.text = "体力已满";
        }
        else
        {
            ResultInfo.text = "购买成功";
        }*/
    }
}
