using UnityEngine;
using System.Collections;

public class PowerUpMaterialcontrol : MonoBehaviour
{
    public enum materialType
    {
        hardware = 0,
        item = 1
    }

    public bool up = false;

    public materialType mType;

    public int uID = -1;

    public string SkinId
    {
        get
        {
            if (uID == -1) return null;
            UserWare uw = UserManager.CurUserInfo.FindUserWare(uID);
            if (uw == null)
            {
                UserItem ui = UserManager.CurUserInfo.FindItemById(uID);
                return ui.CurItemData.SkinId;
            }
            else return uw.CurHardWareData.SkinId;
        }
        set
        {
            SkinId = value;
        }
    }

    public UserItem CurMaterialItem;

    public UserWare CurMaterialHardware;

    public equipmentItemInterface equipMaterial;

    public MaterialItemInterface itemMaterial;

    public UISprite whiteFrame;

    public GameObject CurObject;

    public void ShowWhite(bool show)
    {
        whiteFrame.gameObject.SetActive(show);
    }

    public void SetMaterial(materialType type, int Id)
    {
        mType = type;
        uID = Id;
        if (Id == -1) return;
        switch ((int)type)
        {
            case 0:
                {
                    UserWare uw = UserManager.CurUserInfo.FindUserWare(Id);
                    CurMaterialHardware = uw;
                    GameObject tg = NGUITools.AddChild(gameObject, equipMaterial.gameObject);
                    tg.GetComponent<equipmentItemInterface>().SetItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, false, uw.CurHardWareData.Rank, uw.UserWareId);
                    tg.GetComponent<BoxCollider>().enabled = false;
                    tg.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    CurObject = tg;
                    break;
                }
            case 1:
                {
                    UserItem ui = UserManager.CurUserInfo.FindItemById(Id);
                    CurMaterialItem = ui;
                    GameObject tg = NGUITools.AddChild(gameObject, itemMaterial.gameObject);
                    tg.GetComponent<MaterialItemInterface>().SetMaterialItem(ui.CurItemData.Id);
                    tg.GetComponent<MaterialItemInterface>().userMaterialId = ui.UserItemId;
                    tg.GetComponent<BoxCollider>().enabled = false;
                    tg.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    CurObject = tg;
                    break;
                }
        }
        up = true;
    }

    public void RemoveMaterial()
    {
        Destroy(CurObject);
        uID = -1;
        up = false;
    }
}
