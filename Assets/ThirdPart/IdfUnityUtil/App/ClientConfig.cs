using UnityEngine;
using System.Collections;

public class ClientConfig : MonoBehaviour
{

    #region 配置属性
    /// <summary>
    /// 导航api 接口
    /// </summary>
    /// 
    //string NavApi = "http://parouteoutside.azurewebsites.net/Route.ashx?";
    //string NavApi = "http://121.40.131.6:6000/route?";
    //string NavApi = "http://192.168.29.45:6000/route?";
    //string NavApi = "http://115.29.4.97:6000/route?";

    string NavApi;

    /// <summary>
    /// 导航API 接口 参数
    /// </summary>
    public ClientType ApiArg;

    /// <summary>
    /// 地区信息
    /// </summary>
    public RegionType Region;

    /// <summary>
    /// 内网测试外网发布
    /// </summary>
    public ServerType Server;

    public string NavUrl
    {
        get { return NavApi + InfoParam; }
        set { NavUrl = value; }
    }

    /// <summary>
    /// 参数
    /// </summary>
    public string InfoParam
    {
        get { return "map=" + Region.ToString() + "&version=" + ApiArg.ToString(); }
        set { InfoParam = value; }
    }

    public enum ServerType
    {
        local,
        local2,
        cloud
    }

    /// <summary>
    /// 连接服务器类型枚举
    /// </summary>
    public enum ClientType
    {
        lizi,
        biaoge,
        QA,
        Jason,
        fenghuang,
        andy
    }
    #endregion 

    /// <summary>
    /// 地区类型枚举
    /// </summary>
    public enum RegionType
    {
        cn,
        tw,
        hk,
        jp,
        us
    }

    public int FrameLimit = 0;

	public int MoveSpeed = 500;

	public bool CacheStatus = true;

    void Awake()
    {
        if(FrameLimit > 0)
        {
            Application.targetFrameRate = FrameLimit;
        }
        switch (Server)
        {
            case ServerType.local: NavApi = "http://192.168.29.18:6000/route?"; break;
            case ServerType.local2: NavApi = "http://192.168.29.13:6000/route?"; break;
            case ServerType.cloud: NavApi = "http://115.29.4.97:6000/route?"; break;
            default: NavApi = "http://115.29.4.97:6000/route?"; break;
        }
		PvpMoveManager.MOVE_SPEED = MoveSpeed;
		PvpGameObjectManager.CACHE_STATUS = CacheStatus;
    }

    // Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
	}
}