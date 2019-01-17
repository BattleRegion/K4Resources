using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
public class GameConfig  
{
    static public string LoginHost = "121.40.131.81";
    //static public string LoginHost = "192.168.29.48";
    static public int LoginPort = 50010;
    static public string GateHost = "121.40.131.6";
    //static public string GateHost = "192.168.29.48";
    static public int GatePort = 50100;

    /// <summary>
    ///Pvp服务器地址
    /// </summary>

	static public string PvpHost = "121.40.131.6"; //云：115.29.4.97      本地：192.168.29.45

    static public int PvpPort = 17020;

    protected List<ConfigData> Configs = new List<ConfigData>();
    /// <summary>
    /// 配置名字
    /// </summary>
    private string configName;

    public string ConfigName
    {
        get { return configName; }
        set 
        { 
            configName = value;
            ConfigJsonData = JsonDbHelper.OpenJsonTable(configName);
        }
    }

    /// <summary>
    /// 配置的JSON数据
    /// </summary>
    protected object ConfigJsonData;

    /// <summary>
    /// 所有配置数据结构集合
    /// </summary>
    protected List<GameConfig> Datas = new List<GameConfig>();
}

public class ConfigData
{

}
