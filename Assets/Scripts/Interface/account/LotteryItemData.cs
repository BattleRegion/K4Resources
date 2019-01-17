using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;

public class LotteryItemData 
{
    /// <summary>
    /// 奖品ID
    /// </summary>
    public string Id;

    /// <summary>
    /// 奖品数量
    /// </summary>
    public int Count;

    /// <summary>
    /// 奖品状态，true代表未抽中在奖池中，false代表已抽中移出奖池
    /// </summary>
    public bool Status;

    /// <summary>
    /// 类型
    /// </summary>
    public LotteryEnum.AwardType AwardType
    {
        get
        {
            if (ConfigManager.PetConfig.GetPetById(Id) != null) return LotteryEnum.AwardType.Pet;
            else if (ConfigManager.HardWareConfig.GetHardWareById(Id) != null) return LotteryEnum.AwardType.Hardware;
            else if (ConfigManager.ItemConfig.GetItemById(Id) != null) return LotteryEnum.AwardType.Item;
            else return LotteryEnum.AwardType.None;
        }
        set
        {
            AwardType = value;
        }
    }

    public LotteryItemData(JsonObject data)
    {
        try
        {
            Id = data["id"].ToString();
            Count = int.Parse(data["count"].ToString());
            Status = int.Parse(data["status"].ToString()) == 0 ? true : false;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}

public class LotteryEnum
{
    public enum AwardType
    {
        None = 0,
        Pet = 1,
        Hardware = 2,
        Item = 3
    }
}
