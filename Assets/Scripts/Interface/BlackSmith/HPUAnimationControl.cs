using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HPUAnimationControl : MonoBehaviour, EquipmentForgeInter
{
    public enum AnimationType
    {
        PowerUp = 0,
        Make = 1,
        Evolution = 3
    }

    public AnimationType type;

    #region Animation
    public GameObject BaseBoard;

    public GameObject InfoBoard;

    Vector3 info_position_1 = Vector3.zero;
    Vector3 info_position_2 = new Vector3(0, -400, 0);

    public EquipmentForgeController ForgeAnimation;

    bool isOver = false;

    public void SetAnimation(string Id_base, string Id_m1, string Id_m2, string Id_m3, string Id_m4, string Id_m5)
    {
        ForgeAnimation.animationInter = this;

        List<string> mIds = new List<string>();
        mIds.Add(Id_m1);
        mIds.Add(Id_m2);
        mIds.Add(Id_m3);
        mIds.Add(Id_m4);
        mIds.Add(Id_m5);

        if (type == AnimationType.Make)
        {
            ForgeAnimation.SetAnimation(mIds, Id_base, null);
        }
        else if (type == AnimationType.Evolution)
        {
            ForgeAnimation.SetAnimation(mIds, HLvUp.CurHardWareData.SkinId, Id_base);
        }
        else
        {
            ForgeAnimation.SetAnimation(mIds, HLvUp.CurHardWareData.SkinId, Id_base);
        }
    }

    public void OnFinish()
    {
        if(type == AnimationType.Make)
        {
            AnimationHelper.AnimationMoveTo(info_position_1, InfoBoard, iTween.EaseType.linear, gameObject, "SetOver", 0.2f);
        }
        else if(type == AnimationType.PowerUp)
        {
            AnimationHelper.AnimationMoveTo(info_position_1, InfoBoard, iTween.EaseType.linear, gameObject, "SetPowUpInfo", 0.2f);
        }
        else
        {
            AnimationHelper.AnimationMoveTo(info_position_1, InfoBoard, iTween.EaseType.linear, gameObject, "SetOver", 0.2f);
        }
    }

    void SetOver()
    {
        isOver = true;
    }

    void PlayAudio()
    {
        AudioClip Audio = Resources.Load<AudioClip>("Audio/UIAudio/Synthesis");
        audio.clip = Audio;
        audio.Play();
    }


    public GameObject PlayerAnimation;

    public List<GameObject> HiddenObjects;

    void OnEnable()
    {
        foreach(GameObject g in HiddenObjects)
        {
            g.SetActive(false);
        }
        if(PlayerAnimation != null)
        {
            PlayerAnimation.SetActive(false);
        }
        isOver = false;
        if (type == AnimationType.PowerUp)
        {
            Invoke("SetPowUpStartInfo", 0.01f);
        }
        else if(type == AnimationType.Make)
        {
            Invoke("SetMakeInfo", 0.01f);
            MakeBoard.SetActive(true);
        }
        UIEventListener.Get(BaseBoard).onClick = (g) =>
        {
            if (isOver)
            {
                gameObject.SetActive(false);
                if (CurView != null && TargetView != null)
                {
                    CurView.SetActive(false);
                    TargetView.SetActive(true);
                }

            }
        };

        if (PartyAnime.moveEnd != null)
        {
            PartyAnime.moveEnd();
            PartyAnime.moveEnd = null;
        }

    }

    void OnDisable()
    {
        foreach (GameObject g in HiddenObjects)
        {
            g.SetActive(true);
        }
        if(PlayerAnimation != null)
        {
            PlayerAnimation.SetActive(true);
        }
        InfoBoard.transform.localPosition = info_position_2;
    }

    public GameObject MakeBoard;
    public GameObject PowUpBoard;

    public GameObject CurView;
    public GameObject TargetView;

    public void SetPowUpStartInfo()
    {
        if ((int)Hbase.CurHardWareData.Style < 5)
        {
            SetWeaponStartInfo();
        }
        else
        {
            SetArmorStartInfo();
        }
    }

    public void SetPowUpInfo()
    {
        if ((int)Hbase.CurHardWareData.Style < 5)
        {
            SetWeaponBuffInfo();
        }
        else
        {
            SetArmorBuffInfo();
        }
        Invoke("SetOver", 1f);
    }

    public void SetMakeInfo()
    {
        if ((int)Hbase.CurHardWareData.Style < 5)
        {
            SetMakeWeaponInfo();
        }
        else
        {
            SetMakeArmorInfo();
        }
        UIEventListener.Get(BaseBoard).onClick = (g) =>
        {
            if (isOver)
            {
                gameObject.SetActive(false);
                if (CurView != null && TargetView != null)
                {
                    CurView.SetActive(false);
                    TargetView.SetActive(true);
                }
            }
        };
    }

    public UILabel PreName;
    public UILabel LaterName;
    public equipmentItemInterface PreIcon;
    public equipmentItemInterface LaterIcon;

    public void SetEvolutionInfo(UserWare Pre, UserWare Later)
    {
        Hbase = Pre;
        HLvUp = Later;
        PreName.text = Pre.CurHardWareData.Name;
        LaterName.text = Later.CurHardWareData.Name;
        PreIcon.SetItem(Pre);
        LaterIcon.SetItem(Later);
    }

    #endregion

    #region NGUI
    public UILabel Name;
    public equipmentItemInterface Icon;
    public UILabel PreLv;
    public UILabel PreAtk;

    public UILabel AtkOrDef;

    public UILabel LaterLv;
    public UILabel LaterAtk;
    public UILabel LvBuff;
    public UILabel AtkBuff;

    public AlphaMaskBar ExpBar;
    public UILabel ExpLeft;
    public UILabel MaxLevelTips;

    public GameObject CurBoard;
    public GameObject AfterBoard;

    public UserWare Hbase = null;
    public UserWare HLvUp = null;

    //制造
    public UILabel AtkOrDef_make;
    public UILabel Name_make;
    public equipmentItemInterface Icon_make;
    public UILabel PreLv_make;
    public UILabel PreAtk_make;


    void SetMakeWeaponInfo()
    {
        AtkOrDef_make.text = "攻击";
        Name_make.text = Hbase.CurHardWareData.Name;
        Icon_make.SetItem(Hbase.Level, (int)Hbase.CurAtk, Hbase.CurHardWareData.Element, Hbase.CurHardWareData.SkinId, false, Hbase.CurHardWareData.Rank, Hbase.UserWareId);
        PreLv_make.text = Hbase.Level.ToString();
        PreAtk_make.text = Hbase.CurAtk.ToString();
    }

    void SetMakeArmorInfo()
    {
        AtkOrDef_make.text = "防御";
        Name_make.text = Hbase.CurHardWareData.Name;
        Icon_make.SetItem(Hbase.Level, (int)Hbase.CurAtk, Hbase.CurHardWareData.Element, Hbase.CurHardWareData.SkinId, false, Hbase.CurHardWareData.Rank, Hbase.UserWareId);
        PreLv_make.text = Hbase.Level.ToString();
        PreAtk_make.text = Hbase.CurDef.ToString();
    }

    void SetWeaponStartInfo()
    {
        AtkOrDef.text = "攻击";
        AfterBoard.SetActive(false);
        Name.text = Hbase.CurHardWareData.Name;
        Icon.SetItem(Hbase.Level, (int)Hbase.CurAtk, Hbase.CurHardWareData.Element, Hbase.CurHardWareData.SkinId, false, Hbase.CurHardWareData.Rank, Hbase.UserWareId);
        PreLv.text = Hbase.Level.ToString();
        PreAtk.text = Hbase.CurAtk.ToString();
        ExpBar.value = (float)Hbase.CurExp / (float)Hbase.LevelExp;

        ExpLeft.gameObject.SetActive(true);
        ExpLeft.text = (Hbase.LevelExp - Hbase.CurExp).ToString();
        MaxLevelTips.text = "";
    }

    void SetArmorStartInfo()
    {
        AtkOrDef.text = "防御";
        AfterBoard.SetActive(false);
        Name.text = Hbase.CurHardWareData.Name;
        Icon.SetItem(Hbase.Level, (int)Hbase.CurAtk, Hbase.CurHardWareData.Element, Hbase.CurHardWareData.SkinId, false, Hbase.CurHardWareData.Rank, Hbase.UserWareId);
        PreLv.text = Hbase.Level.ToString();
        PreAtk.text = Hbase.CurDef.ToString();
        ExpBar.value = (float)Hbase.CurExp / (float)Hbase.LevelExp;

        ExpLeft.gameObject.SetActive(true);
        ExpLeft.text = (Hbase.LevelExp - Hbase.CurExp).ToString();
        MaxLevelTips.text = "";
    }

    void SetWeaponBuffInfo()
    {
        AfterBoard.SetActive(true);
        AnimationHelper.LabelUpdate(LaterLv, Hbase.Level, HLvUp.Level, 1f, "LaterLvUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LaterAtk, Hbase.CurAtk, HLvUp.CurAtk, 1f, "LaterAtkUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LvBuff, 0, (HLvUp.Level - Hbase.Level), 1f, "LvBuffUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(AtkBuff, 0, (HLvUp.CurAtk - Hbase.CurAtk), 1f, "AtkBuffUpdate", gameObject, null, null);
        ExpBarUpdate(Hbase.Level, HLvUp.Level);

        if(HLvUp.Level < HLvUp.CurHardWareData.LvlMax)
        {
            ExpLeft.text = (HLvUp.LevelExp - HLvUp.CurExp).ToString();
        }
        else if(HLvUp.CurHardWareData.Rank < HLvUp.CurHardWareData.MaxRank)
        {
            ExpLeft.gameObject.SetActive(false);
            MaxLevelTips.text = "已满级，请前去进化";
        }
        else
        {
            ExpLeft.gameObject.SetActive(false);
            MaxLevelTips.text = "已达终极状态";
        }
        Icon.SetItem(HLvUp.Level, (int)HLvUp.CurAtk, HLvUp.CurHardWareData.Element, HLvUp.CurHardWareData.Id, false, HLvUp.CurHardWareData.Rank, HLvUp.UserWareId);

    }

    void SetArmorBuffInfo()
    {
        Debug.Log(HLvUp);
        AfterBoard.SetActive(true);
        AnimationHelper.LabelUpdate(LaterLv, Hbase.Level, HLvUp.Level, 1f, "LaterLvUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LaterAtk, Hbase.CurDef, HLvUp.CurDef, 1f, "LaterAtkUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LvBuff, 0, (HLvUp.Level - Hbase.Level), 1f, "LvBuffUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(AtkBuff, 0, (HLvUp.CurDef - Hbase.CurDef), 1f, "AtkBuffUpdate", gameObject, null, null);
        ExpBarUpdate(Hbase.Level, HLvUp.Level);

        if (HLvUp.Level < HLvUp.CurHardWareData.LvlMax)
        {
            ExpLeft.text = (HLvUp.LevelExp - HLvUp.CurExp).ToString();
        }
        else if (HLvUp.CurHardWareData.Rank < HLvUp.CurHardWareData.MaxRank)
        {
            ExpLeft.gameObject.SetActive(false);
            MaxLevelTips.text = "已满级，请前去进化";
        }
        else
        {
            ExpLeft.gameObject.SetActive(false);
            MaxLevelTips.text = "已达终极状态";
        }
        Icon.SetItem(HLvUp.Level, (int)HLvUp.CurAtk, HLvUp.CurHardWareData.Element, HLvUp.CurHardWareData.Id, false, HLvUp.CurHardWareData.Rank, HLvUp.UserWareId);

    }

    /// <summary>
    /// 文本动画
    /// </summary>
    void LaterLvUpdate(int value)
    {
        LaterLv.text = value.ToString();
    }

    void LaterAtkUpdate(int value)
    {
        LaterAtk.text = value.ToString();
    }

    void LvBuffUpdate(int value)
    {
        LvBuff.text = "+" + value.ToString();
    }

    void AtkBuffUpdate(int value)
    {
        AtkBuff.text = "+" + value.ToString();
    }

    /// <summary>
    /// 经验条动画
    /// </summary>
    void ExpBarUpdate(int fromLv, int toLv)
    {
        if (fromLv < toLv)
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, ((float)Hbase.CurExp / (float)Hbase.LevelExp), 1f, 0.5f, "ExpUpdate", gameObject, "UpdateOver", gameObject, toLv - fromLv - 1);
        }
        else
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, ((float)Hbase.CurExp / (float)Hbase.LevelExp), ((float)HLvUp.CurExp / (float)HLvUp.LevelExp), 0.5f, "ExpUpdate", gameObject, null, null, null);
        }
    }

    void UpdateOver(int tag)
    {
        if (tag > 0)
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, 0f, 1f, 0.5f, "ExpUpdate", gameObject, "UpdateOver", gameObject, tag - 1);
        }
        else if(HLvUp.Level < HLvUp.CurHardWareData.LvlMax)
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, 0f, (float)HLvUp.CurExp / (float)HLvUp.LevelExp, 0.5f, "ExpUpdate", gameObject, null, null, null);
        }
        else
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, 0f, 1f, 0.5f, "ExpUpdate", gameObject, null, null, null);
        }
    }

    void ExpUpdate(float value)
    {
        ExpBar.value = value;
    }
    #endregion
}
