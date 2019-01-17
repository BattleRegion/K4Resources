using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
public class UserWare  
{
    public int UserWareId;

    private int exp;

    public int Exp
    {
        get
        {
            if(exp == null)
            {
                exp = ConfigManager.HardwareLevelConfig.GetHardwareExpByLv(Level);
            }
            return exp;
        }
        set
        {
            exp = value;
        }
    }

    private int curExp;
    public int CurExp
    {
        get
        {
            curExp = Exp - ConfigManager.HardwareLevelConfig.GetHardwareExpByLv(Level);
            return curExp;
        }
        set
        {
            curExp = value;
        }
    }

    private int levelExp;
    public int LevelExp
    {
        get
        {
            levelExp = ConfigManager.HardwareLevelConfig.GetHardwareLvUpNeedByLv(Level);
            return levelExp;
        }
        set
        {
            levelExp = value;
        }
    }

    protected int level;
    public virtual int Level
    {
        get
        {
            level = ConfigManager.HardwareLevelConfig.GetLevelByExp(Exp);
            return level;
        }
        set
        {
            level = value;
            Exp = ConfigManager.HardwareLevelConfig.GetHardwareExpByLv(level);
        }
    }

    public virtual bool IsWare
    {
        get
        {
            bool temp = false;
            for(int i = 0; i < 5; i++)
            {
                if (UserManager.CurUserInfo.UserPartys[i].weapon == this || UserManager.CurUserInfo.UserPartys[i].helmet == this || UserManager.CurUserInfo.UserPartys[i].armor == this)
                    temp = true;
            }
            return temp;
        }
        set
        {

        }
    }

    public List<int> warePartys
    {
        get
        {
            List<int> temp = new List<int>();
            for(int i = 0; i < 5; i++)
            {
                if (UserManager.CurUserInfo.UserPartys[i].weapon == this || UserManager.CurUserInfo.UserPartys[i].helmet == this || UserManager.CurUserInfo.UserPartys[i].armor == this)
                {
                    temp.Add(i);
                }
            }
            return temp;
        }
        set
        {

        }
    }

    //public int Attack;

    //public int Def;

    //public int Hp;

    //public int Color;

    public HardWareData CurHardWareData;

    private int curAtk;

    public int CurAtk
    {

        get {  
			return CurHardWareData.Attack + (Level - 1) * CurHardWareData.AttackUp; 
		}
        set { curAtk = value; }
    }

    private int curDef;

    public int CurDef
    {
        get { return CurHardWareData.Defend + (Level - 1) * CurHardWareData.DefendUp; }
        set { curDef = value; }
    }

    private int curHp;

    public int CurHp
    {
        get { return CurHardWareData.Hp; }
        set { curHp = value; }
    }

    public int CurWarefare
    {
        get
        {
            return (int)((0.1 * (float)Level + 1f) * (float)CurHardWareData.Warefare);
        }
        set { CurWarefare = value; }
    }

    public UserWare(JsonObject data)
    {
		if(data == null) return;

        try
        {
            if (data.ContainsKey("house_id")) UserWareId = int.Parse(data["house_id"].ToString());
            CurHardWareData = ConfigManager.HardWareConfig.GetHardWareById(data["id"].ToString());
            if (data.ContainsKey("exp")) Exp = int.Parse(data["exp"].ToString());
            //if (data.ContainsKey("attack")) Attack = int.Parse(data["attack"].ToString());
            //if (data.ContainsKey("def")) Def = int.Parse(data["def"].ToString());
            //if (data.ContainsKey("hp")) Hp = int.Parse(data["hp"].ToString());
            //if (data.ContainsKey("color")) Color = int.Parse(data["color"].ToString());

            //int ware = 0;
            //if (data.ContainsKey("is_wear")) ware = int.Parse(data["is_wear"].ToString());
            //if (ware == 0)
            //{
            //    IsWare = false;
            //}
            //else
            //{
            //    IsWare = true;
            //}
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// 本地生成
    /// </summary>
    public UserWare(string Id, int hLevel)
    {
        UserWareId = -1;
        CurHardWareData = ConfigManager.HardWareConfig.GetHardWareById(Id);
        Level = hLevel;
        Exp = ConfigManager.HardwareLevelConfig.GetHardwareExpByLv(Level);
        //IsWare = false;
    }

    public UserWare(string Id, int exp, int Uid)
    {
        UserWareId = Uid;
        CurHardWareData = ConfigManager.HardWareConfig.GetHardWareById(Id);
        Exp = exp;
    }

    public bool IsWeapon()
    {
        if (CurHardWareData.Style == HardWareData.HardWareType.Far1 || CurHardWareData.Style == HardWareData.HardWareType.Far2 || CurHardWareData.Style == HardWareData.HardWareType.Heavy || CurHardWareData.Style == HardWareData.HardWareType.Light)
        {
            return true;
        }
        return false;
    }

    public bool IsHelmet()
    {
        if (CurHardWareData.Style == HardWareData.HardWareType.Head)
        {
            return true;
        }
        return false;
    }

    public bool IsArmor()
    {
        if (CurHardWareData.Style == HardWareData.HardWareType.Cuirass)
        {
            return true;
        }
        return false;
    }
}
