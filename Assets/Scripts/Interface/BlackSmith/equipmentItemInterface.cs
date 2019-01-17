using UnityEngine;
using System.Collections;

public interface EquipmentiIemInterface
{
    void _OnClickEquipmentItem(int UserEquipmentID);
    void _OnLongPressEquipmentItem(int UserEquipmentID);
}

public class equipmentItemInterface : MonoBehaviour
{
    /// <summary>
    /// 接口定义
    /// </summary>
    public EquipmentiIemInterface equipmentItemInter;


    public UISprite coverSprite;
    public UISprite elementSprite;
    public UISprite RankFrame;
    public UILabel levelInfoLbl;

    public UITexture equipmentSprite;

    public UISprite newSprite;
    public UISprite baseSprite;
    public UISprite isEquipSprite;
    public UISprite lockSprite;
    public UISprite materialSprite;
    public UISprite whiteFrame;

    public UILabel sellLabel;

    public UIGrid stars;


    /// <summary>
    /// levelInfoLbl中可能显示的信息
    /// </summary>
    public int level;
    public int hp;
    public int atk;

    public DungeonEnum.ElementAttributes elementType;
    public bool isNew;
    public bool isEquip;
    public bool isLock;
    public bool isMaterial;
    public bool isCover;
    public bool isChosen;
    public bool isBase;

    public int sellSequence;

    /// <summary>
    /// ID
    /// </summary>
    public int userEquipmentID;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Level"></param>
    /// <param name="Atk"></param>
    /// <param name="type"></param>
    /// <param name="MonsterID"></param>
    /// <param name="isNew"></param>
    /// <param name="StarNum"></param>
    /// <param name="UserMonsterID"></param>
    public void SetItem(
        int Level,
        int Atk,
        DungeonEnum.ElementAttributes type,
        string SkinID,
        bool isNew,
        int StarNum,
        int UserEquipmentID
        )
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
        if (uw != null)
        {
            if (Level >= uw.CurHardWareData.LvlMax)
            {
                levelInfoLbl.text = "Lv.MAX";
            }
            else
            {
                levelInfoLbl.text = "Lv." + Level.ToString();
            }
        }
		if(uw != null && uw.CurHardWareData != null)
		{
        	elementSprite.spriteName = Tools.GetHardwareElement(uw.CurHardWareData.Element);
            RankFrame.spriteName = Tools.GetRankFrame(uw.CurHardWareData.Rank);
		}

        IsNew(isNew);

        SetStars set = stars.GetComponent<SetStars>();
        set.SetStar(StarNum);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(SkinID);
        if (skinData != null)
        {
            Texture t = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
            equipmentSprite.mainTexture = t;
        }     
        userEquipmentID = UserEquipmentID;
    }

    public void SetItem(UserWare uw)
    {
        if (uw != null)
        {
            if (uw.Level >= uw.CurHardWareData.LvlMax)
            {
                levelInfoLbl.text = "Lv.MAX";
            }
            else
            {
                levelInfoLbl.text = "Lv." + uw.Level.ToString();
            }
        }

        elementSprite.spriteName = Tools.GetHardwareElement(uw.CurHardWareData.Element);
        RankFrame.spriteName = Tools.GetRankFrame(uw.CurHardWareData.Rank);

        IsNew(isNew);

        SetStars set = stars.GetComponent<SetStars>();
        set.SetStar(uw.CurHardWareData.Rank);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(uw.CurHardWareData.SkinId);
        if (skinData != null)
        {
            Texture t = Resources.Load<Texture>("Atlas/ItemIcons/" + skinData.IconId);
            equipmentSprite.mainTexture = t;
        }
        userEquipmentID = uw.UserWareId;
    }

    public void IsNew(bool isN)
    {
        newSprite.gameObject.SetActive(isN);
        isNew = isN;
    }

    public void IsEquip(bool isE)
    {
        isEquip = isE;
        isEquipSprite.gameObject.SetActive(isE);
    }

    public void IsCover(bool isC)
    {
        isCover = isC;
        coverSprite.gameObject.SetActive(isC);
    }

    public void IsMaterial(bool isM)
    {
        isMaterial = isM;
        materialSprite.gameObject.SetActive(isM);
    }

    public void IsBase(bool isB)
    {
        isBase = isB;
        baseSprite.gameObject.SetActive(isB);
    }

    public void IsChosen(bool isCh)
    {
        isChosen = isCh;
        whiteFrame.gameObject.SetActive(isCh);
    }

    public void SetSelect(bool select)
    {
        IsCover(select);
        IsChosen(select);
    }

    /// <summary>
    /// 设置出售的序号
    /// </summary>
    public void SetSellSequence(bool IsSell, int sellNum)
    {
        sellLabel.gameObject.SetActive(IsSell);
        if (IsSell)
        {
            sellLabel.text = sellNum.ToString();
        }
    }

    public void SetSellSequence(bool IsSell)
    {
        sellLabel.gameObject.SetActive(IsSell);
    }



    #region 点击事件判定
    float pressTime = 0f;
    bool press = false;
    public float longPressTime = 1f;

    /// <summary>
    /// 鼠标移动就无法触发长按事件
    /// </summary>
    Vector3 MouseOriginalPosition = new Vector3(0f, 0f, 0f);

    void OnClick() //短按功能
    {
        if (equipmentItemInter != null)
        {
            equipmentItemInter._OnClickEquipmentItem(userEquipmentID);
        }
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            if (!press)
            {
                MouseOriginalPosition = Input.mousePosition;
            }
            press = true;
        }
        else
        {
            pressTime = 0f;
            press = false;
        }
    }


    void Update()   //计时实现按钮长按功能
    {
        if (press)
        {
            pressTime += Time.deltaTime;
            if (pressTime > longPressTime && MouseOriginalPosition == Input.mousePosition)
            {
                if (equipmentItemInter != null)
                {
                    equipmentItemInter._OnLongPressEquipmentItem(userEquipmentID);
                }
                press = false;
                pressTime = 0f;
            }
        }
    }
    #endregion
}
