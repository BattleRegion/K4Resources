using UnityEngine;
using System.Collections;

public interface _MaterialItemInterface
{
    void _OnClickMaterial(int Id);
    void _OnLongPressMaterial(int Id);
}

public class MaterialItemInterface : MonoBehaviour
{
    public UITexture icon;
    public UISprite cover;
    public UISprite lockSprite;
    public UISprite materialSprite;
    public UISprite rankFrame;
    public UILabel sellSequenceLabel;
    public SetStars stars;
    public UISprite whiteFrame;
    public UISprite newSprite;

    public string materialId;
    public int userMaterialId;
    public int rank;


    public bool isMaterial;
    public bool isLock;
    public bool isCover;
    public bool isChosen;
    public bool isNew;

    public _MaterialItemInterface materialItemInter;

    public void SetMaterialItem(string Id)
    {
        ItemData id = ConfigManager.ItemConfig.GetItemById(Id);
        rank = id.Rank;
        SkinConfigData skin = ConfigManager.SkinConfig.GetSkinDataById(id.SkinId);
        if (skin != null)
        {
            icon.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + skin.IconId);
        }
        rankFrame.spriteName = Tools.GetRankFrame(id.Rank);
        stars.SetStar(id.Rank);
    }

    public void IsNew(bool isN)
    {
        isNew = isN;
        newSprite.gameObject.SetActive(isN);
    }

    public void IsMaterial(bool isM)
    {
        isMaterial = isM;
        materialSprite.gameObject.SetActive(isM);
    }

    public void IsLock(bool isL)
    {
        isLock = isL;
        lockSprite.gameObject.SetActive(isL);
    }

    public void IsCover(bool isC)
    {
        isCover = isC;
        cover.gameObject.SetActive(isC);
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
        sellSequenceLabel.gameObject.SetActive(IsSell);
        if (IsSell)
        {
            sellSequenceLabel.text = sellNum.ToString();
        }
    }

    public void SetSellSequence(bool IsSell)
    {
        sellSequenceLabel.gameObject.SetActive(IsSell);
    }

    #region 点击事件判定
    float pressTime = 0f;
    bool press = false;
    public float longPressTime = 1f;
    Vector3 MouseOriginalPosition = new Vector3(0f, 0f, 0f);

    void OnClick() //短按功能
    {
        if (materialItemInter != null)
        {
            materialItemInter._OnClickMaterial(userMaterialId);
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
                if (materialItemInter != null)
                {
                    materialItemInter._OnLongPressMaterial(userMaterialId);
                }
                press = false;
                pressTime = 0f;
            }
        }
    }
    #endregion
}
