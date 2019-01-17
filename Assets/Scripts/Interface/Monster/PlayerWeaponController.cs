using UnityEngine;
using System.Collections;

public interface PartyWeapon
{
    void _OnClickWeapon();
    void _OnLongPressWeapon(int UserWareId);
}

public class PlayerWeaponController : MonoBehaviour 
{
    public UITexture weapon;
    public UILabel weaponLevel;
    public UISprite weaponElementType;
    public PartyWeapon weaponInter;

    public EquipmentBagControl bag;
    public UIGrid EquipmentBag;

    public int userEquipmentID = -1;

    public void PartyWeaponInfo(string WeaponID, int WeaponLevel, DungeonEnum.ElementAttributes ElementType, int UserEquipmentID)
    {
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(WeaponID);
        if (skinData!=null)
        {
            weapon.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
        }
        UserWare u = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
        if(WeaponLevel == u.CurHardWareData.LvlMax)
        {
            weaponLevel.text = "Lv.MAX";
        }
        else
        {
            weaponLevel.text = "Lv." + WeaponLevel.ToString();
        }
        if(UserManager.CurUserInfo.FindUserWare(UserEquipmentID) != null)
        {
            weaponElementType.spriteName = Tools.GetHardwareElement(UserManager.CurUserInfo.FindUserWare(UserEquipmentID).CurHardWareData.Element);
        }
        else
        {
            weaponElementType.spriteName = "";
        }
        
        userEquipmentID = UserEquipmentID;
    }

    float pressTime = 0f;
    bool press = false;
    public float longPressTime = 1f;

    void OnClick() //短按功能
    {
		if (weaponInter != null && bag != null)
        {
            bag.ClickUserEquipmentID = userEquipmentID;
            weaponInter._OnClickWeapon();
        }
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            press = true;
        }
        else
        {
            press = false;
            pressTime = 0f;
        }
    }


    void Update()   //计时实现按钮长按功能
    {
        if (press)
        {
            pressTime += Time.deltaTime;
            if (pressTime > longPressTime)
            {
                if (weaponInter != null)
                    weaponInter._OnLongPressWeapon(userEquipmentID);
                press = false;
                pressTime = 0f;
            }
        }
    }
}
