using UnityEngine;
using System.Collections;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;
using System.Collections.Generic;
public class UserPet 
{
    public PetData CurPetData;

    public int UserPetId;

    private int level;
    public virtual int Level
    {
        get
        {
            level = ConfigManager.PetLevelConfig.GetCurLevel(Exp);
            return level;
        }
        set
        {
            level = value;
            Exp = ConfigManager.PetLevelConfig.GetExpByLevel(level);
        }
    }

    private int exp;

    public int Exp
    {
        get
        {
            if(exp == null)
            {
                exp = ConfigManager.PetLevelConfig.GetExpByLevel(Level);
            }
            return exp;
        }
        set
        {
            exp = value;
        }
    }

    public bool inParty
    {
        get
        {
            bool temp = false;
            for(int i = 0; i < 5; i++)
            {
                if(UserManager.CurUserInfo.UserPartys[i].pets.Contains(this))
                {
                    temp = true;
                }
            }
            return temp;
        }
        set
        {
        }
    }

    public List<int> inPartys
    {
        get
        {
            List<int> temp = new List<int>();
            for(int i = 0; i < 5; i++)
            {
                if(UserManager.CurUserInfo.UserPartys[i].pets.Contains(this))
                {
                    temp.Add(i);
                }
            }
            return temp;
        }
        set { }
    }


    private int currentExp;
    public int CurrentExp
    {
        get
        {
            currentExp = Exp - ConfigManager.PetLevelConfig.GetExpByLevel(Level);
            return currentExp;
        }
        set
        {
            currentExp = value;
        }
    }

    private int curLvlExp;
    public int CurLvlExp
    {
        get
        {
            curLvlExp = ConfigManager.PetLevelConfig.GetLevelExp(Level);
            return curLvlExp;
        }
        set
        {
            curLvlExp = value;
        }
    }


    private float curHp;

    public float CurHp
    {
        get { curHp = CurPetData.Hp + (Level - 1) * CurPetData.HpUp; return curHp; }
        set { curHp = value; }
    }

    private float curAtk;

    public float CurAtk
    {
        get { curAtk = CurPetData.Attack + (Level - 1) * CurPetData.AtkUp; return curAtk; }
        set { curAtk = value; }
    }

    public int CurWarefare
    {
        get
        {
            return (int)((0.1 * (float)Level + 1f) * (float)CurPetData.Warefare);
        }
        set { CurWarefare = value; }
    }

    public UserPet(JsonObject data)
    {
		if(data == null) return;

		if(data.ContainsKey("house_id")) UserPetId = int.Parse(data["house_id"].ToString());
        if(data.ContainsKey("exp")) Exp = int.Parse(data["exp"].ToString());
        CurPetData = ConfigManager.PetConfig.GetPetById(data["id"].ToString());
    }

    /// <summary>
    /// 本地生成
    /// </summary>
    public UserPet(string Id, int Lv)
    {
        CurPetData = ConfigManager.PetConfig.GetPetById(Id);
        UserPetId = -1;
        Level = Lv;
        Exp = ConfigManager.PetLevelConfig.GetExpByLevel(Level);
    }

    public UserPet(string Id, int exp, int Uid)
    {
        UserPetId = Uid;
        CurPetData = ConfigManager.PetConfig.GetPetById(Id);
        Exp = exp;
    }

    #region 方法

    /// <summary>
    /// 升级
    /// </summary>
    public void Upgrade(List<UserPet> materialPets,Action callback)
    {
        JsonObject args = new JsonObject();
        args.Add("house_id", UserPetId);
        List<int> materialIds = new List<int>();
        foreach (UserPet up in materialPets)
        {
            materialIds.Add(up.UserPetId);
        }
        args.Add("pet_ids", materialIds);
        SocketCenter.Request(GameRouteConfig.UpgradePet, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                foreach (UserPet matiralPet in materialPets)
                {
                    for (int i = UserManager.CurUserInfo.UserPets.Count - 1; i >= 0; i++)
                    {
                        UserPet up = UserManager.CurUserInfo.UserPets[i];
                        if (matiralPet.UserPetId == up.UserPetId)
                        {
                            UserManager.CurUserInfo.UserPets.RemoveAt(i); break;
                        }
                    }
                }
                callback();
            }
        },null,true,true);
    }

    /// <summary>
    /// 进化
    /// </summary>
    public void Evo(Action callback)
    {
        
        JsonObject args = new JsonObject();
        args.Add("house_id", UserPetId);
        SocketCenter.Request(GameRouteConfig.EvoPet, args, (r) =>
        {
        }, null, true,true);
    }
    #endregion
}
