using UnityEngine;
using System.Collections;

public class Tools  {

    /// <summary>
    /// 设置transform的layer为BasicFX以及子物体Sprite renderer组件的sorting layer渲染层级为top（不为default即可）
    /// </summary>
    /// <param name="trans">GameObject的transform</param>
    static public void SetLayer(Transform trans, int tarLayer)
    {

        trans.gameObject.layer = tarLayer;
        if (trans.childCount == 0)
        {
            return;
        }
        foreach (Transform t in trans.transform)
        {
            //t.gameObject.layer = tarLayer;
            SetLayer(t, tarLayer);           
        }
    }
    static public void SetChildrenDepth(Transform trans,int addDepth)
    {
        
        if (trans.childCount == 0)
        {
            trans.GetComponent<UIWidget>().depth = trans.GetComponent<UIWidget>().depth + addDepth;
            return;
        }
        foreach (Transform t in trans.transform)
        {
            t.GetComponent<UIWidget>().depth = t.GetComponent<UIWidget>().depth + addDepth;
            SetChildrenDepth(t, addDepth);            
        }        
    }
    static public bool NullOrEmp(string k)
    {
        return string.IsNullOrEmpty(k);
    }

    /// <summary>
    /// 获取装备球
    /// </summary>
    public static string GetHardwareTypeIcon(HardWareData.HardWareType type)
    {
        switch(type)
        {
            case HardWareData.HardWareType.Light: return "hardware_icon_light";
            case HardWareData.HardWareType.Heavy: return "hardware_icon_heavy";
            case HardWareData.HardWareType.Far1: return "hardware_icon_remote";
            case HardWareData.HardWareType.Far2: return "hardware_icon_remote";
            case HardWareData.HardWareType.Head: return "hardware_icon_helmet";
            case HardWareData.HardWareType.Cuirass: return "hardware_icon_armor";
            default: return null;
        }
    }

    /// <summary>
    /// 获取装备元素球
    /// </summary>
    public static string GetHardwareElement(DungeonEnum.ElementAttributes element)
    {
        switch(element)
        {
            case DungeonEnum.ElementAttributes.None: return null;
            case DungeonEnum.ElementAttributes.Fire: return "element_type_fire";
            case DungeonEnum.ElementAttributes.Earth: return "element_type_earth";
            case DungeonEnum.ElementAttributes.Water: return "element_type_water";
            case DungeonEnum.ElementAttributes.Wind: return "element_type_wind";
            default: return null;
        }
    }

    /// <summary>
    /// 根据装备rank获得边框样式
    /// </summary>
    public static string GetRankFrame(int rank)
    {
        switch(rank)
        {
            case 1: return "rank_frame_1";
            case 2: return "rank_frame_2";
            case 3: return "rank_frame_3";
            case 4: return "rank_frame_4";
            case 5: return "rank_frame_5";
            default: return null;
        }
    }

    /// <summary>
    /// Icon路径
    /// </summary>
    public static readonly string IconPath = "Atlas/ItemIcons/";
    public static readonly string PetIconPath = "Atlas/PetAvatars/";
    public static readonly string LotteryIconPath = "Atlas/LotteryIcons/";

    /// <summary>
    /// 获得skinId
    /// </summary>
    public static string GetSkinIdById(string Id)
    {
        if (ConfigManager.PetConfig.GetPetById(Id) != null) return ConfigManager.PetConfig.GetPetById(Id).SkinId;
        else return null;
    }

    /// <summary>
    /// 根据ID获取Skinpath
    /// </summary>
    public static string GetSkinPathById(string Id)
    {
        if (ConfigManager.HardWareConfig.GetHardWareById(Id) != null || ConfigManager.ItemConfig.GetItemById(Id) != null) return IconPath + GetSkinIdById(Id);
        else return null;
    }

    /// <summary>
    /// 根据skinID获取Skinpath
    /// </summary>
    public static string GetSkinPathBySkinId(string sId)
    {
        if (string.IsNullOrEmpty(sId)) return null;
        else return IconPath + sId;
    }

    /// <summary>
    /// 获得IconId
    /// </summary>
    public static string GetIconIdById(string Id)
    {
        if (ConfigManager.PetConfig.GetPetById(Id) != null) return ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.PetConfig.GetPetById(Id).SkinId).IconId;
        else if (ConfigManager.HardWareConfig.GetHardWareById(Id) != null) return ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.HardWareConfig.GetHardWareById(Id).SkinId).IconId;
        else if (ConfigManager.ItemConfig.GetItemById(Id) != null) return ConfigManager.ItemConfig.GetItemById(Id).Icon;
        else return null;
    }

    /// <summary>
    /// 获得Icon sprite路径
    /// </summary>
    public static string GetIconPath(string Id)
    {
        if (ConfigManager.PetConfig.GetPetById(Id) != null) return PetIconPath + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.PetConfig.GetPetById(Id).SkinId).IconId;
        else if (ConfigManager.HardWareConfig.GetHardWareById(Id) != null) return IconPath + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.HardWareConfig.GetHardWareById(Id).SkinId).IconId;
        else if (ConfigManager.ItemConfig.GetItemById(Id) != null) return IconPath + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.ItemConfig.GetItemById(Id).SkinId).IconId;
        else return null;
    }

    /// <summary>
    /// 获得装备素材icon Sprite by skinId
    /// </summary>
    public static string GetIconBySkinId(string Sid)
    {
        if (string.IsNullOrEmpty(Sid)) return null;
        else return IconPath + ConfigManager.SkinConfig.GetSkinDataById(Sid).IconId;
    }

    /// <summary>
    /// 获得宠物icon Sprite by skinId
    /// </summary>
    public static string GetPetIconBySkinId(string Sid)
    {
        if (string.IsNullOrEmpty(Sid)) return null;
        else return PetIconPath + ConfigManager.SkinConfig.GetSkinDataById(Sid).IconId;
    }

    /// <summary>
    /// 获得Icon Texture路径
    /// </summary>
    public static string GetIconTexturePath(string Id)
    {
        if (ConfigManager.PetConfig.GetPetById(Id) != null) return PetIconPath + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.PetConfig.GetPetById(Id).SkinId).IconId;
        else if (ConfigManager.HardWareConfig.GetHardWareById(Id) != null) return IconPath + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.HardWareConfig.GetHardWareById(Id).SkinId).IconId;
        else if (ConfigManager.ItemConfig.GetItemById(Id) != null) return IconPath + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.ItemConfig.GetItemById(Id).SkinId).IconId;
        else return null;
    }

    /// <summary>
    /// 获得抽奖Icon路径
    /// </summary>
    public static string GetLotteryIconPath(string Id)
    {
        if (ConfigManager.PetConfig.GetPetById(Id) != null) return LotteryIconPath + "G" + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.PetConfig.GetPetById(Id).SkinId).IconId;
        else if (ConfigManager.HardWareConfig.GetHardWareById(Id) != null) return LotteryIconPath + "G" + ConfigManager.SkinConfig.GetSkinDataById(ConfigManager.HardWareConfig.GetHardWareById(Id).SkinId).IconId;
        else if (ConfigManager.ItemConfig.GetItemById(Id) != null) return LotteryIconPath + "G" + ConfigManager.ItemConfig.GetItemById(Id).Icon;
        else return null;
    }

    /// <summary>
    /// 获取PVP等级ICON
    /// </summary>
    /// <param name="rank">PVP等级</param>
    /// <returns></returns>
    public static Texture GetPvPIcon(int rank)
    {
        ArenaData ad = ConfigManager.ArenaConfig.GetArenaByLv(rank);
        if (ad != null)
        {
            return Resources.Load<Texture>("Atlas/ArmyIcons/" + ad.RankIcon);
        }
        else return null;
    }

    //随机0.0-1.0
    public static double GetRandom()
    {
        System.Random rd = new System.Random();
        return  rd.NextDouble();       
    }
    //0-n
    public static int GetRandom_n(int n)
    {
		System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
        return  r.Next(0, n);
    }

}