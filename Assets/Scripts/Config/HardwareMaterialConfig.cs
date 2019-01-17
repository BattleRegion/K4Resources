using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class HardwareMaterialConfig : GameConfig
{
    public HardwareMaterialConfig()
    {
        this.ConfigName = "Synthesis";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            HardwareMaterialData h = new HardwareMaterialData(data);
            Configs.Add(h);
        }
    }

    public HardwareMaterialData GetHardwareMaterialById(string Id)
    {
        foreach (HardwareMaterialData h in Configs)
        {
            if (h.Id == Id)
            {
                return h;
            }
        }
        return null;
    }

    /// <summary>
    /// 得到某个素材可制作装备的ID List
    /// </summary>
    /// <param name="MaterialId"></param>
    /// <returns></returns>
    public List<string> GetMaterialTargetIds(string MaterialId)
    {
        List<string> results = new List<string>();
        foreach (HardwareMaterialData h in Configs)
        {
            if (h.MaterialId_1 == MaterialId || h.MaterialId_2 == MaterialId || h.MaterialId_3 == MaterialId || h.MaterialId_4 == MaterialId || h.MaterialId_5 == MaterialId)
            {
                results.Add(h.Id);
            }
        }
        return (results);
    }

    /// <summary>
    /// 得到显示给用户的装备制作列表
    /// </summary>
    public List<string> GetShowHardwareList(BSListControl.HardwareType Type)
    {
        List<string> showList = new List<string>();
        bool jump = false;
        foreach (ItemData id in ConfigManager.ItemConfig.GetOldMaterials())
        {
            foreach (HardwareMaterialData hwd in Configs)
            {
                HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(hwd.Id);
                switch ((int)Type)  //五种类型
                {
                    case 0:
                        {
                            if (hd.Style == HardWareData.HardWareType.Light)
                            {
                                if (hwd.MaterialId_1 == id.Id || hwd.MaterialId_2 == id.Id || hwd.MaterialId_3 == id.Id || hwd.MaterialId_4 == id.Id || hwd.MaterialId_5 == id.Id)
                                {
                                    if (showList.Count != 0)  //避免重复
                                    {
                                        foreach (string s in showList)
                                        {
                                            if (s == hwd.Id)
                                            {
                                                jump = true;
                                                break;
                                            }
                                        }
                                        if (jump)
                                        {
                                            jump = false;
                                            break;
                                        }
                                    }
                                    showList.Add(hwd.Id);
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                            if (hd.Style == HardWareData.HardWareType.Heavy)
                            {
                                if (hwd.MaterialId_1 == id.Id || hwd.MaterialId_2 == id.Id || hwd.MaterialId_3 == id.Id || hwd.MaterialId_4 == id.Id || hwd.MaterialId_5 == id.Id)
                                {
                                    if (showList.Count != 0)  //避免重复
                                    {
                                        foreach (string s in showList)
                                        {
                                            if (s == hwd.Id)
                                            {
                                                jump = true;
                                                break;
                                            }
                                        }
                                        if (jump)
                                        {
                                            jump = false;
                                            break;
                                        }
                                    }
                                    showList.Add(hwd.Id);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            if (hd.Style == HardWareData.HardWareType.Far1 || hd.Style == HardWareData.HardWareType.Far2)
                            {
                                if (hwd.MaterialId_1 == id.Id || hwd.MaterialId_2 == id.Id || hwd.MaterialId_3 == id.Id || hwd.MaterialId_4 == id.Id || hwd.MaterialId_5 == id.Id)
                                {
                                    if (showList.Count != 0)  //避免重复
                                    {
                                        foreach (string s in showList)
                                        {
                                            if (s == hwd.Id)
                                            {
                                                jump = true;
                                                break;
                                            }
                                        }
                                        if (jump)
                                        {
                                            jump = false;
                                            break;
                                        }
                                    }
                                    showList.Add(hwd.Id);
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            if (hd.Style == HardWareData.HardWareType.Head)
                            {
                                if (hwd.MaterialId_1 == id.Id || hwd.MaterialId_2 == id.Id || hwd.MaterialId_3 == id.Id || hwd.MaterialId_4 == id.Id || hwd.MaterialId_5 == id.Id)
                                {
                                    if (showList.Count != 0)  //避免重复
                                    {
                                        foreach (string s in showList)
                                        {
                                            if (s == hwd.Id)
                                            {
                                                jump = true;
                                                break;
                                            }
                                        }
                                        if (jump)
                                        {
                                            jump = false;
                                            break;
                                        }
                                    }
                                    showList.Add(hwd.Id);
                                }
                            }
                            break;
                        }
                    case 4:
                        {
                            if (hd.Style == HardWareData.HardWareType.Cuirass)
                            {
                                if (hwd.MaterialId_1 == id.Id || hwd.MaterialId_2 == id.Id || hwd.MaterialId_3 == id.Id || hwd.MaterialId_4 == id.Id || hwd.MaterialId_5 == id.Id)
                                {
                                    if (showList.Count != 0)  //避免重复
                                    {
                                        foreach (string s in showList)
                                        {
                                            if (s == hwd.Id)
                                            {
                                                jump = true;
                                                break;
                                            }
                                        }
                                        if (jump)
                                        {
                                            jump = false;
                                            break;
                                        }
                                    }
                                    showList.Add(hwd.Id);
                                }
                            }
                            break;
                        }
                    default: break;
                }

            }
        }
        foreach(string s in ConfigManager.DungeonConfig.GetOpenHardwares())
        {
            if(!showList.Contains(s))
            {
                showList.Add(s);
            }
        }
        return showList;
    }
}

public class HardwareMaterialData : ConfigData
{
    /// <summary>
    /// ID
    /// </summary>
    public string Id;

    /// <summary>
    /// 合成价格
    /// </summary>
    public int Price;

    /// <summary>
    /// 素材1ID
    /// </summary>
    public string MaterialId_1;

    /// <summary>
    /// 素材1Rate
    /// </summary>
    public int Rate_1;

    public string MaterialId_2;

    public int Rate_2;

    public string MaterialId_3;

    public int Rate_3;

    public string MaterialId_4;

    public int Rate_4;

    public string MaterialId_5;

    public int Rate_5;

    public HardwareMaterialData(JsonObject data)
    {
        try
        {
            Id = data["Synthesis"].ToString();
            Price = int.Parse(data["SynthesisCoin"].ToString());
            MaterialId_1 = data["SynthesisN1"].ToString();
            Rate_1 = int.Parse(data["SynthesisN1Rate"].ToString());
            MaterialId_2 = data["SynthesisN2"].ToString();
            Rate_2 = int.Parse(data["SynthesisN2Rate"].ToString());
            MaterialId_3 = data["SynthesisN3"].ToString();
            Rate_3 = int.Parse(data["SynthesisN3Rate"].ToString());
            MaterialId_4 = data["SynthesisN4"].ToString();
            Rate_4 = int.Parse(data["SynthesisN4Rate"].ToString());
            MaterialId_5 = data["SynthesisN5"].ToString();
            Rate_5 = int.Parse(data["SynthesisN5Rate"].ToString());
        }
        catch
        {

        }
    }
}