using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
public class ItemConfig : GameConfig {

    public ItemConfig()
    {
        this.ConfigName = "Item";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            ItemData m = new ItemData(data);
            Configs.Add(m);
            if (!PlayerPrefs.HasKey(AppMember.MemberId.ToString() + m.Id))
            {
                PlayerPrefs.SetInt(AppMember.MemberId.ToString() + m.Id, 0);
            }
        }
    }

    public ItemData GetItemById(string Id)
    {
        foreach (ItemData i in Configs)
        {
            if (i.Id == Id)
            {
                return i;
            }
        }
        return null;
    }

    /// <summary>
    /// 得到获得过的item数组
    /// </summary>
    public List<ItemData> GetOldMaterials()
    {
        List<ItemData> olddata = new List<ItemData>();
        foreach(ItemData id in Configs)
        {
            if(!IsNew(id.Id))
            {
                olddata.Add(id);
            }
        }
        return olddata;
    }

    /// <summary>
    /// 判断是否为新获得
    /// </summary>
    public bool IsNew(string Id)
    {
        ItemData id = GetItemById(Id);
        if (id == null) return false;
        else
        {
            switch (PlayerPrefs.GetInt(AppMember.MemberId.ToString() + id.Id))
            {
                case 0: return true;
                case 1: return false;
                default: return false;
            }
        }
    }

    /// <summary>
    /// 标记已经获得过
    /// </summary>
    public bool SetNotNew(string Id)
    {
        ItemData id = GetItemById(Id);
        if (id == null) return false;
        else
        {
            PlayerPrefs.SetInt(AppMember.MemberId.ToString() + id.Id, 1);
            return true;
        }
    }

    /// <summary>
    /// 清除所有标记
    /// </summary>
    public void ClearAllKeys()
    {
        foreach(ItemData i in Configs)
        {
            if (PlayerPrefs.HasKey(AppMember.MemberId.ToString() + i.Id))
            {
                PlayerPrefs.DeleteKey(AppMember.MemberId.ToString() + i.Id);
            }
        }
    }
}

public class ItemData:ConfigData
{
    /// <summary>
    /// ID
    /// </summary>
    public string Id;

    /// <summary>
    /// 皮肤ID
    /// </summary>
    public string SkinId;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon;

    /// <summary>
    /// 价格
    /// </summary>
    public int Price;

    /// <summary>
    /// 星级
    /// </summary>
    public int Rank;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description;

    /// <summary>
    /// 装备经验
    /// </summary>
    public int HardwareEXP;

    public string TipsRank;

    public string Tips;

    public ItemData(JsonObject data)
    {
        try
        {
            Id = data["ItemId"].ToString();
            SkinId = data["SkinId"].ToString();
            Icon = data["TipsIcon"].ToString();
            Price = int.Parse(data["Price"].ToString());
            Rank = int.Parse(data["Rank"].ToString());
            Description = data["Description"].ToString();
            HardwareEXP = int.Parse(data["HardwareEXP"].ToString());
            TipsRank = data["TipsRank"].ToString();
            Tips = data["Tips"].ToString();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
