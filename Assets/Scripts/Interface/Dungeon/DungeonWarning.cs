using UnityEngine;
using System.Collections;

public class DungeonWarning : MonoBehaviour
{
    #region Warning
    public GameObject WarningBoard;
    public GameObject ChallengeButton;
    public GameObject ReturnButton;
    public DungeonSelectUI DungeonUI;
    public UILabel WarningInfo;
    public DungeonData CurDungeonData = null;

    string WarningText = "副本难度过大，建议强化宠物和装备，累积实力后再来挑战";
    string ForbiddenText = "战力值过低，无法进入副本，请强化宠物和装备，累积实力后再来挑战";

    /// <summary>
    /// 警告
    /// </summary>
    public void ShowWarning(DungeonData d)
    {
        CurDungeonData = d;
        WarningBoard.SetActive(true);
        WarningInfo.text = WarningText;
        ChallengeButton.SetActive(true);
        ReturnButton.transform.localPosition = new Vector3(140f, -80f, 0f);
        WarningBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        AnimationHelper.AnimationScaleTo(new Vector3(1f, 1f, 1f), WarningBoard, iTween.EaseType.spring, null, null, 0.1f);
    }

    /// <summary>
    /// 禁止
    /// </summary>
    public void ShowForbidden()
    {
        WarningBoard.SetActive(true);
        WarningInfo.text = ForbiddenText;
        ChallengeButton.SetActive(false);
        ReturnButton.transform.localPosition = new Vector3(0f, -80f, 0f);
        WarningBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        AnimationHelper.AnimationScaleTo(new Vector3(1f, 1f, 1f), WarningBoard, iTween.EaseType.spring, null, null, 0.1f);
    }
    #endregion

    #region elementConfirm
    public GameObject ConfirmBoard;
    public GameObject ConfirmButton;
    public GameObject CancelButton;

    public GameObject BackBoard;

    public GameObject AchievementBoard;
    public UILabel AchievementTips;
    public UISprite AchievementDiamond;

    public GameObject BossTipsBoard;
    public UITexture BossTips;

    public GameObject ElementConfirmBoard;

    Vector3 AchievementBoard_p1 = new Vector3(0f, 130f, 0f);
    Vector3 AchievementBoard_p2 = new Vector3(0f, -25f, 0f);

    Vector3 ElementConfirmBoard_p1 = new Vector3(0f, -395f, 0f);
    Vector3 ElementConfirmBoard_p2 = new Vector3(0f, -180f, 0f);

    /// <summary>
    /// 判断机制
    /// </summary>
    public bool ShouldConfirm(DungeonData d)
    {
        switch (d.Element)
        {
            case DungeonEnum.ElementAttributes.Earth: return ElementConfirm(DungeonEnum.ElementAttributes.Fire);
            case DungeonEnum.ElementAttributes.Fire: return ElementConfirm(DungeonEnum.ElementAttributes.Water);
            case DungeonEnum.ElementAttributes.None: return false;
            case DungeonEnum.ElementAttributes.Water: return ElementConfirm(DungeonEnum.ElementAttributes.Wind);
            case DungeonEnum.ElementAttributes.Wind: return ElementConfirm(DungeonEnum.ElementAttributes.Earth);
            default: return false;
        }
    }

    /// <summary>
    /// 总判断机智
    /// </summary>
    public bool ShouldShowConfirm(DungeonData d)
    {
        //if ((!string.IsNullOrEmpty(d.Quest) && !UserManager.CurUserInfo.HasAchievedDungeon(d.Id)) || !string.IsNullOrEmpty(d.BossTips) || ShouldConfirm(d))
        //{
        //    return true;
        //}
        if (ShouldConfirm(d)) return true;
        return false;
    }

    bool ElementConfirm(DungeonEnum.ElementAttributes counterElement)
    {

        if (((UserManager.CurUserInfo.CurWeapon == null) ? false : (UserManager.CurUserInfo.CurWeapon.CurHardWareData.Element != counterElement)) || ((UserManager.CurUserInfo.CurHelmet == null) ? false : (UserManager.CurUserInfo.CurHelmet.CurHardWareData.Element != counterElement)) || ((UserManager.CurUserInfo.CurArmor == null) ? false : (UserManager.CurUserInfo.CurArmor.CurHardWareData.Element != counterElement)))
        {
            return true;
            //int count1 = 0, count2 = 0, count3 = 0;
            //foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
            //{
            //    if (count1 * count2 * count3 != 0)
            //    {
            //        return true;
            //    }
            //    if (uw.IsWeapon() && uw.CurHardWareData.Element == counterElement)
            //    {
            //        count1++;
            //    }
            //    if (uw.IsHelmet() && uw.CurHardWareData.Element == counterElement)
            //    {
            //        count2++;
            //    }
            //    if (uw.IsArmor() && uw.CurHardWareData.Element == counterElement)
            //    {
            //        count3++;
            //    }
            //}
        }
        return false;
    }

    public void ShowConfirm(DungeonData d)
    {
        CurDungeonData = d;
        ConfirmBoard.SetActive(true);

        //if(!string.IsNullOrEmpty(CurDungeonData.Quest) && !UserManager.CurUserInfo.HasAchievedDungeon(CurDungeonData.Id))
        if(false)
        {
            AchievementBoard.SetActive(true);
            if (UserManager.CurUserInfo.HasAchievedDungeon(CurDungeonData.Id))
            {
                AchievementDiamond.spriteName = "level_diamond";
            }
            else
            {
                AchievementDiamond.spriteName = "level_diamond_back";
            }
            AchievementTips.text = CurDungeonData.QuestTips;
        }
        else
        {
            AchievementBoard.SetActive(false);
        }

        //if (!string.IsNullOrEmpty(CurDungeonData.BossTips))
        if(false)
        {
            AchievementBoard.transform.localPosition = AchievementBoard_p1;
            ElementConfirmBoard.transform.localPosition = ElementConfirmBoard_p1;
            BossTipsBoard.SetActive(true);
            BossTips.mainTexture = Resources.Load<Texture>("Atlas/BossTips/" + d.BossTips);
        }
        else
        {
            AchievementBoard.transform.localPosition = AchievementBoard_p2;
            ElementConfirmBoard.transform.localPosition = ElementConfirmBoard_p2;
            BossTipsBoard.SetActive(false);
        }

        if(ShouldConfirm(CurDungeonData))
        {
            ElementConfirmBoard.SetActive(true);
        }
        else
        {
            ElementConfirmBoard.SetActive(false);
        }

        ConfirmBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        AnimationHelper.AnimationScaleTo(new Vector3(1f, 1f, 1f), ConfirmBoard, iTween.EaseType.spring, null, null, 0.1f);
    }
    #endregion

    void Start()
    {
        UIEventListener.Get(ChallengeButton).onClick = (g) =>
        {
            //if (ShouldShowConfirm(CurDungeonData))
            //{
            //    WarningBoard.SetActive(false);
            //    ShowConfirm(CurDungeonData);
            //}
            //else
            //{
            //    PveGameControl.CurDungeonId = CurDungeonData.Id;
            //    DungeonUI.ShowHelpList();
            //    WarningBoard.SetActive(false);
            //}

            PveGameControl.CurDungeonId = CurDungeonData.Id;
            DungeonUI.ShowHelpList();
            WarningBoard.SetActive(false);
        };

        UIEventListener.Get(ReturnButton).onClick = (g) =>
        {
            WarningBoard.SetActive(false);
        };

        UIEventListener.Get(BackBoard).onClick = (g) =>
        {
            PveGameControl.CurDungeonId = CurDungeonData.Id;
            DungeonUI.ShowHelpList();
            ConfirmBoard.SetActive(false);
        };

        UIEventListener.Get(CancelButton).onClick = (g) =>
        {
            ConfirmBoard.SetActive(false);
        };
    }
}
