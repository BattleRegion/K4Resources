using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;

public class SetUser : MonoBehaviour {

    //currentExp 当前经验值
    //maxExp 最大经验值
    //currentStrength 当前体力
    //maxStrength 最大体力
    //UserName 用户名
    //Gold 金币数量
    //Diamonds 钻石数量
    //Level 等级
    public AlphaMaskBar exp;
    public AlphaMaskBar strength;
    public UILabel strengthText;
    public UILabel userName;
    public UILabel gold;
    public UILabel diamonds;
    public UILabel level;
    public UILabel RecoveryTime;

    public UILabel VersionLabel;

    public UILabel Warfare;

    public UISprite NewNotice;

    public GameObject button_recover_energy;
    public UILabel value_energy_pool;

    /// <summary>
    /// 动画
    /// </summary>
    public void SetInfo()
    {
        if(UserManager.CurUserInfo.Gold > 999999)
        {
            gold.text = "??????";
        }
        else
        {
            SetGoldText();
        }
        if(UserManager.CurUserInfo.Diamond > 9999)
        {
            diamonds.text = "????";
        }
        else
        {
            SetDiamondText();
        }
        strength.value = (float)UserManager.CurUserInfo.Energy / (float)UserManager.CurUserInfo.EnergyLimit;

        strengthText.text = UserManager.CurUserInfo.Energy.ToString() + "/" + UserManager.CurUserInfo.EnergyLimit.ToString();

        //int currentExp = UserManager.CurUserInfo.Exp - UserManager.CurUserInfo.CurHero.Exp;
        //int curLevelExp = ConfigManager.HeroConfig.GetHeroByLvl(UserManager.CurUserInfo.Level + 1).Exp - UserManager.CurUserInfo.CurHero.Exp;
        //exp.value = (float)currentExp / (float)curLevelExp;

        level.text = UserManager.CurUserInfo.Level.ToString();
        userName.text = UserManager.CurUserInfo.NickName;
        Warfare.text = UserManager.CurUserInfo.CurWarfare.ToString();
    }

    void PlayVoice()
    {
        AudioClip Ac = Resources.Load<AudioClip>("Audio/UIAudio/SellCoins");
        audio.clip = Ac;
        audio.Play();
    }

    /// <summary>
    /// 金币滚动
    /// </summary>
    void SetGoldText()
    {
        int from = int.Parse(gold.text);
        int to = UserManager.CurUserInfo.Gold;
        if(to > from)
        {
            //PlayVoice();
        }
        AnimationHelper.LabelUpdate(gold, from, to, 1f, "GoldUpdate", gameObject, null, null);
    }

    void GoldUpdate(int value)
    {
        gold.text = value.ToString();
    }


    void SetDiamondText()
    {
        int from = int.Parse(diamonds.text);
        int to = UserManager.CurUserInfo.Diamond;
        if (to > from)
        {
            //PlayVoice();
        }
        AnimationHelper.LabelUpdate(diamonds, from, to, 1f, "DiamondUpdate", gameObject, null, null);
    }

    void DiamondUpdate(int value)
    {
        diamonds.text = value.ToString();
    }

    //设置人物信息
    public void SetUserInfo(
        int currentExp,
        int maxExp,
        int currentStrength,
        int maxStrength,
        string UserName,
        int Gold,
        int Diamonds,
        int Level)
    {
        //int curExp = UserManager.CurUserInfo.CurExp;
        //int curLevelExp = UserManager.CurUserInfo.CurLevelExp;
        //exp.value = (float)curExp / (float)curLevelExp;

        strength.value = (float)currentStrength / (float)maxStrength;

        if (Gold > 999999)
        {
            gold.text = "??????";
        }
        else
        {
            gold.text = Gold.ToString();
        }
        if(Diamonds > 9999)
        {
            diamonds.text = "????";
        }
        else
        {
            diamonds.text = Diamonds.ToString();
        }
        level.text = Level.ToString();
        strengthText.text = currentStrength.ToString() + "/" + maxStrength.ToString();
        userName.text = UserName;
        Warfare.text = UserManager.CurUserInfo.CurWarfare.ToString();
    }

    bool RefreshRecovery
    {
        get
        {
            if (UserManager.CurUserInfo.Energy >= UserManager.CurUserInfo.EnergyLimit)
                return true;
            return false;
        }
        set
        {
            RefreshRecovery = value;
        }
    }

    public static DateTime NextHeartBeat = new DateTime(0);


    /// <summary>
    /// 体力恢复
    /// </summary>
    void Update()
    {
        Warfare.text = UserManager.CurUserInfo.CurWarfare.ToString();
        if (ApplicationControl.AddEnergyTag)
        {
            SetInfo();
            ApplicationControl.AddEnergyTag = false;
        }
        if(UserManager.CurUserInfo.RecoveryTag)
        {
            if (RecoveryTime.gameObject.activeSelf == false) RecoveryTime.gameObject.SetActive(true);
            RecoveryTime.text = UserManager.CurUserInfo.RecoveryCutdownTime.Minutes.ToString() + ":" + (UserManager.CurUserInfo.RecoveryCutdownTime.Seconds < 10 ?  ("0" + UserManager.CurUserInfo.RecoveryCutdownTime.Seconds.ToString()) : UserManager.CurUserInfo.RecoveryCutdownTime.Seconds.ToString());
        }
        else
        {
            if (RecoveryTime.gameObject.activeSelf == true) RecoveryTime.gameObject.SetActive(false);
        }

        //if (ConfigManager.LocalTime.LocalTime > NextHeartBeat)
        //{
        //    NextHeartBeat = new DateTime(ConfigManager.LocalTime.LocalTime.Ticks + TimeSpan.TicksPerMinute);
        //    SocketCenter.Request(GameRouteConfig.GetEnergyPool, null, (r) => 
        //    {
        //        if(r.Code == SocketResult.ResultCode.Success)
        //        {
        //            Loom.QueueOnMainThread(() => 
        //            {
        //                UserManager.CurUserInfo.EnergyPool = int.Parse(r.Data["energy"].ToString());
        //                value_energy_pool.text = r.Data["energy"].ToString();
        //            });
        //        }
        //    }, null, true, true);
        //}
    }

    void Start()
    {
        VersionLabel.text = ConfigManager.ParamConfig.GetParam().ConfigVersion;
        SetUserInfo(UserManager.CurUserInfo.Exp, UserManager.CurUserInfo.CurLevelExp, UserManager.CurUserInfo.Energy, UserManager.CurUserInfo.EnergyLimit, UserManager.CurUserInfo.NickName, UserManager.CurUserInfo.Gold, UserManager.CurUserInfo.Diamond, UserManager.CurUserInfo.Level);
        value_energy_pool.text = UserManager.CurUserInfo.EnergyPool.ToString();

        foreach(UserTask ut in UserManager.CurUserInfo.UserDailyTasks)
        {
            if(ut.newTask)
            {
                NewNotice.gameObject.SetActive(true);
                break;
            }
            NewNotice.gameObject.SetActive(false);
        }

        UIEventListener.Get(button_recover_energy).onClick = (g) => 
        {
            if(UserManager.CurUserInfo.EnergyPool > 0)
            {
                SocketCenter.Request(GameRouteConfig.RecoverEnergy, null, (r) => 
                {
                    if(r.Code == SocketResult.ResultCode.Success)
                    {
                        Loom.QueueOnMainThread(() => 
                        {
                            UserManager.CurUserInfo.AddElements((JsonArray)r.Data["elements"]);
                            value_energy_pool.text = "0";
                            SetInfo();
                        });
                    }
                }, null, true, true);
            }
        };
    }
}