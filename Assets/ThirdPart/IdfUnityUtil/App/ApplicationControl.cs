using UnityEngine;
using System.Collections;
using SimpleJson;
using System;
public class ApplicationControl : MonoBehaviour
{
    #region 属性
    /// <summary>
    /// 当前应用程序指正
    /// </summary>
    public static ApplicationControl CurApp;

    /// <summary>
    /// 客户端配置指正
    /// </summary>
    public ClientConfig Config;

    public GameObject CurLoading;
	public GameObject CurPvpLoading;

    //public GameObject 
    #endregion

    #region 初始化
    public static void Init()
    {
        if (!GameObject.Find("App"))
        {
            GameObject appObject = new GameObject("App");
            GameObject.DontDestroyOnLoad(appObject);
            CurApp = appObject.AddComponent<ApplicationControl>();
            CurApp.Config = GameObject.Find("ClientConfig").GetComponent<ClientConfig>();
        }
    }
    #endregion

    #region 重写MONO

    void Awake()
    {
        //设置自己不被销毁
        GameObject.DontDestroyOnLoad(gameObject);
        //创建HTTP访问对象
        AddHttp();
        //创建LOOM对象
        AddLoom();
    }
    // Use this for initialization
	void Start () 
    {

	}

    public static bool AddEnergyTag = false;

	// Update is called once per frame
	void Update () 
    {
        if(UserManager.AppStartTag) //体力恢复
        {
            if (UserManager.CurUserInfo != null)
            {
                if (UserManager.CurUserInfo.RecoveryTag)
                {
                    if (UserManager.CurUserInfo.RecoveryCutdownTime.Ticks <= 0)
                    {
                        ++UserManager.CurUserInfo.Energy;
                        AddEnergyTag = true;
                        if (UserManager.CurUserInfo.Energy < UserManager.CurUserInfo.EnergyLimit)
                        {
                            UserManager.CurUserInfo.StartRecoveryTime = new DateTime(ConfigManager.LocalTime.LocalTime.Ticks);
                        }
                        else
                        {
                            UserManager.CurUserInfo.RecoveryTag = false;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 创建LOOM
    void AddLoom()
    {
        if (!GameObject.Find("Loom"))
        {
            GameObject loomObject = new GameObject("Loom");
            GameObject.DontDestroyOnLoad(loomObject);
            loomObject.AddComponent<Loom>();
        }
    }
    #endregion

    #region 创建HTTP
    void AddHttp()
    {
        if (!GameObject.Find("Http"))
        {
            GameObject httpObject = new GameObject("Http");
            GameObject.DontDestroyOnLoad(httpObject);
            httpObject.AddComponent<HttpHelper>();
        }
    }
    #endregion

    #region 应用程序回调
    void OnApplicationPause()
    {

    }
    #endregion

    #region 公用方法

	public void BeginLoading()
    {
        Loom.QueueOnMainThread(() =>
            {
                if (CurLoading == null)
                {
                    CurLoading = Instantiate(Resources.Load("PreFabs/UI/LOADING") as GameObject) as GameObject;
                }
            });
    }

	public void BeginPvpLoading()
	{
		Loom.QueueOnMainThread(() =>
		                       {
			if (CurPvpLoading == null)
			{
				CurPvpLoading = Instantiate(Resources.Load("PreFabs/FX/JJC_0") as GameObject) as GameObject;
			}
		});
	}

	public void StopPvpLoading()
	{
		Loom.QueueOnMainThread(() =>
		{
			if (CurPvpLoading)
			{
				Destroy(CurPvpLoading);
				CurPvpLoading = null;
			}
		});
	}
	
	public void StopLoading()
	{
		Loom.QueueOnMainThread(() =>
		{
            if (CurLoading)
            {
                Destroy(CurLoading);
                CurLoading = null;
            }
        });
    }

    public void ShowErrorWindow(SocketResult result, bool pvpStatus = false)
    {
        Loom.QueueOnMainThread(() =>
        {
            if (result.Code != SocketResult.ResultCode.Success)
            {
                GameObject window = NGUITools.AddChild(GameObject.Find("UI Root"), Resources.Load<GameObject>("PreFabs/UI/ErrorWindow")) as GameObject;
                ErrorControl ErrorController = window.GetComponent<ErrorControl>();
                ErrorController.SetInfo(result.ErrorCode, pvpStatus);
                StopLoading();
            }
        });
    }

    public void ShowErrorWindow(int code)
    {
        Loom.QueueOnMainThread(() =>
        {
            if (code != 200)
            {
                StopLoading();
                GameObject window = NGUITools.AddChild(GameObject.Find("UI Root"), Resources.Load<GameObject>("PreFabs/UI/ErrorWindow")) as GameObject;
                window.GetComponent<ErrorControl>().SetInfo(code);
            }
        });
    }

    public void ShowInfoWindow(string info)
    {
        Loom.QueueOnMainThread(() =>
        {
            StopLoading();
            GameObject window = NGUITools.AddChild(GameObject.Find("UI Root"), Resources.Load<GameObject>("PreFabs/UI/ErrorWindow")) as GameObject;
            window.GetComponent<ErrorControl>().SetInfo(info);
        });
    }
    #endregion
}
