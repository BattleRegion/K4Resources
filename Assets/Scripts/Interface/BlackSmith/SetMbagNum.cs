using UnityEngine;
using System.Collections;

public class SetMbagNum : MonoBehaviour
{
    public MaterialBagControl bag;

    public UILabel count;

    void OnEnable()
    {
        count.text = bag.materialItems.Count.ToString() + "/" + UserManager.CurUserInfo.WareLimit.ToString();
    }

    public void RefreshNum()
    {
        count.text = bag.materialItems.Count.ToString() + "/" + UserManager.CurUserInfo.WareLimit.ToString();
    }
}
