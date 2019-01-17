using UnityEngine;
using System.Collections;


/// <summary>
/// 怪物栏中怪物item的点击接口
/// </summary>
public interface itemInterface
{
    void _OnClickItem(int UserMonsterID);
    void _OnLongPressItem(int UserMonsterID);
}

public class ItemInterface : MonoBehaviour {

    /// <summary>
    /// 接口定义
    /// </summary>
    public itemInterface itemInter;

    
    public UISprite coverSprite;
    public UISprite elementSprite;
    public UISprite rankFrame;
    public UILabel levelInfoLbl;
    
    public UITexture monsterSprite;

    public UISprite newSprite;
    public UISprite baseSprite;
    public UISprite partySprite;
    public UISprite lockSprite;
    public UISprite materialSprite;
    public UISprite whiteFrameSprite;
    public UILabel sellLabel;

    public UIGrid stars;

    /// <summary>
    /// levelInfoLbl中可能显示的信息
    /// </summary>
    public int level;
    public int cost;
    public int hp;
    public int atk;
    public int rank;


    public DungeonEnum.ElementAttributes elementType;
    public bool isNew;
    public bool isParty;
    public bool isBase;
    public bool isLock;
    public bool isMaterial;
    public bool isCover;
    public bool isChosen;

    /// <summary>
    /// ID
    /// </summary>
    public int userMonsterID;

    public void SetItem(
        int Level,
        int Cost,
        int Hp,
        int Atk,
        DungeonEnum.ElementAttributes type,
        string MonsterID,
        bool isNew,
        int StarNum,
        int UserMonsterID
        )
    {
        UserPet up = UserManager.CurUserInfo.FindPetById(UserMonsterID);
        rank = StarNum;
        if (up != null)
        {
            if (up.Level >= up.CurPetData.MaxLevel) levelInfoLbl.text = "Lv.MAX";
            else levelInfoLbl.text = "Lv." + Level.ToString();
        }
        else
        {
            levelInfoLbl.text = "Lv." + Level.ToString();
        }
        elementSprite.spriteName = Tools.GetHardwareElement(up.CurPetData.PetPro);
        rankFrame.spriteName = Tools.GetRankFrame(up.CurPetData.Rank);

        IsNew(isNew);

        SetStars set = stars.GetComponent<SetStars>();
        set.SetStar(StarNum);
        PetData pdata = ConfigManager.PetConfig.GetPetById(MonsterID);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(pdata.SkinId);
        if (skinData!=null)
        {
            Texture t = Resources.Load<Texture>("Atlas/PetAvatars/" + skinData.IconId);
            monsterSprite.mainTexture = t;
        }
        userMonsterID = UserMonsterID;
        
    }

    public void SetItem(UserPet up)
    {
        rank = up.CurPetData.Rank;
        if (up != null)
        {
            if (up.Level >= up.CurPetData.MaxLevel) levelInfoLbl.text = "Lv.MAX";
            else levelInfoLbl.text = "Lv." + up.Level.ToString();
        }
        else
        {
            levelInfoLbl.text = "Lv." + up.Level.ToString();
        }

        elementSprite.spriteName = Tools.GetHardwareElement(up.CurPetData.PetPro);
        rankFrame.spriteName = Tools.GetRankFrame(up.CurPetData.Rank);

        IsNew(isNew);

        SetStars set = stars.GetComponent<SetStars>();
        set.SetStar(up.CurPetData.Rank);
        PetData pdata = ConfigManager.PetConfig.GetPetById(up.CurPetData.Id);
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(pdata.SkinId);
        if (skinData != null)
        {
            Texture t = Resources.Load<Texture>("Atlas/PetAvatars/" + skinData.IconId);
            monsterSprite.mainTexture = t;
        }
        userMonsterID = up.UserPetId;

    }

    /// <summary>
    /// “new”闪烁
    /// </summary>
    public void IsNew(bool isN)
    {
        isNew = isN;
        newSprite.gameObject.SetActive(isNew);
    }

    /// <summary>
    /// 设置在队伍中
    /// </summary>
    /// <param name="inP"></param>
    public void InParty(bool inP)
    {
        isParty = inP;
        partySprite.gameObject.SetActive(inP);
    }

    /// <summary>
    /// 设置是base
    /// </summary>
    /// <param name="isB"></param>
    public void IsBase(bool isB)
    {
        isBase = isB;
        baseSprite.gameObject.SetActive(isB);
    }

    /// <summary>
    /// 设置已被锁
    /// </summary>
    /// <param name="isL"></param>
    //public void IsLock(bool isL)
    //{
    //    isLock = isL;
    //    lockSprite.gameObject.SetActive(isL);
    //}

    /// <summary>
    /// 设置已是material
    /// </summary>
    /// <param name="isM"></param>
    public void IsMaterial(bool isM)
    {
        isMaterial = isM;
        materialSprite.gameObject.SetActive(isM);
    }

    /// <summary>
    /// 设置有暗色覆盖
    /// </summary>
    /// <param name="isC"></param>
    public void IsCover(bool isC)
    {
        isCover = isC;
        coverSprite.gameObject.SetActive(isC);
    }

    /// <summary>
    /// 被选中的白框
    /// </summary>
    public void IsChosen(bool isCh)
    {
        isChosen = isCh;
        whiteFrameSprite.gameObject.SetActive(isCh);
    }


    public void SetSelect(bool select)
    {
        IsCover(select);
        IsChosen(select);
    }


    public void SetSell(bool IsSell, int sellNum)
    {
        sellLabel.gameObject.SetActive(IsSell);
        if (IsSell)
        {
            sellLabel.text = sellNum.ToString();
        }
    }

    public void SetSell(bool IsSell)
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
        if (itemInter != null)
        {
            if (!isParty || BagControl.NotInParty)  //在队伍中不响应点击事件
            {
                itemInter._OnClickItem(userMonsterID);
            }
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
                if (itemInter != null)
                {
                    itemInter._OnLongPressItem(userMonsterID);
                }
                press = false;
                pressTime = 0f;
            }
        }
    }
    #endregion
}
