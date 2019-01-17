using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    bool canLogin = false;

    void Awake()
    {
        ApplicationControl.Init();
    }
	// Use this for initialization
	void Start () 
    {
        LoginControl.ConnectToLoginServer((loginCode) =>
        {
            if (loginCode == LoginControl.ClientLoginCode.LoginConnectSuccess)
            {
                canLogin = true;
            }
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    bool hasSelectServer = false;
    string curLoginServerInfo = "没有选择服务器";
    void OnGUI()
    {
        GUI.Label(new Rect(60, 450, 200, 40), curLoginServerInfo);

        if (GUI.Button(new Rect(60, 226, 100, 50), "登录"))
        {
            if (canLogin == true && hasSelectServer == true)
            {
                LoginControl.Login("frame@1.com", "1234", AppMember.AccountType.Normal, SystemInfo.deviceUniqueIdentifier, (loginResultCode) =>
                {
                    if (loginResultCode == LoginControl.ClientLoginCode.LoginSuccess)
                    {
                        Debug.Log("登录成功");
                    }
                });
            }
        }
        if (canLogin == true)
        {
            for (int i = 0; i < LoginControl.ServerList.Count; i++)
            {
                ServerInfo info = LoginControl.ServerList[i];
                if (GUI.Button(new Rect(200, 226 + i * 55, 100, 50), info.ServerName))
                {
                    LoginControl.SelectServer(info);
                    curLoginServerInfo = info.ServerName + info.Host + ":" + info.Port.ToString();
                    hasSelectServer = true;
                }
            }
            
        }
    }

}
