using UnityEngine;
using System.Collections;

public class ButtonOverExit : MonoBehaviour
{
    /// <summary>
    /// 当前场景 不是结算面板
    /// </summary>
    public GameObject curView;

    public GameObject dungeonView;
    public GameObject partyView;

    void OnClick()
    {
        curView.SetActive(false);
        if (OverControl.overDone)
        {
            dungeonView.SetActive(true);
            partyView.SetActive(false);
        }
    }
}
