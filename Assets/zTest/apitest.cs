using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
using PomeloSocketCenter.PomeloLib;

public class apitest : MonoBehaviour
{

    public GameObject LoginBorad;
    public GameObject LoginSwitch;
    public GameObject LoginButton;

    void OnEnable()
    {
        UIEventListener.Get(LoginSwitch).onClick = (g) =>
        {
            if (LoginBorad.activeSelf == false) LoginBorad.SetActive(true);
            else LoginBorad.SetActive(false);
        };
    }

    // Use this for initialization
    void Awake()
    {
        ApplicationControl.Init();
    }

    public GameObject LoginAnime;
    public NoticeController Notice;

    bool hasReady = false;
    bool hasIn = false;
    void Start()
    {
       
        GameObjectEnabled(false);
        //ApplicationControl.CurApp.BeginLoading();

        Invoke("ConnectLoginServer", 0.1f);
       
    }
    void ConnectLoginServer()
    {
        LoginControl.ConnectToLoginServer((loginCode) =>
        {
            if (loginCode == LoginControl.ClientLoginCode.LoginConnectSuccess)
            {
                SetProgressText("--");
                Notice.RequestNoticeInfo();

                //选择第一个区

                ServerInfo info = LoginControl.ServerList[Tools.GetRandom_n(LoginControl.ServerList.Count - 1)];
                //ServerInfo info = new ServerInfo("121.40.131.6", 50100);
                //ServerInfo info = new ServerInfo(GameConfig.GateHost, GameConfig.GatePort);
                LoginControl.SelectServer(info);
                hasReady = true;
                ApplicationControl.CurApp.StopLoading();
                Loom.QueueOnMainThread(() =>
                {
                    //Invoke("DelayLogin", 1f);
                    LoadConfigs();
                });
            }
        });
    }
    void GameObjectEnabled(bool b)
    {
        gameObject.GetComponent<BoxCollider>().enabled = b;
        if (LoginButton) LoginButton.GetComponent<BoxCollider>().enabled = b;

    }
    void LoadConfigs()
    {
        ConfigManager.RefreshConfig((success) =>
        {
            if (success)
            {
                DelayLogin();
                GameObjectEnabled(true);
            }
        }, (n, total) =>
        {
            ProgressLabel(n, total);
        });

    }
    //configProgress
    public void ProgressLabel(int n, int total)
    {       
        //Debug.Log("xx "+n+"  "+total);
        float per = (float)n / (float)total;
        string s=(int)(per * 100) + " %";
        SetProgressText(s);
    }
    void SetProgressText(string s)
    {
        GameObject pl = GameObject.Find("ProgressLabel");
        UILabel ul = pl.GetComponent<UILabel>();
        ul.text = s;
    }

    void DelayLogin()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        LoginAnime.SetActive(true);

        UIEventListener.Get(gameObject).onClick = (go) =>
        {
            ApplicationControl.CurApp.BeginLoading();
            LoginControl.Login("", "", AppMember.AccountType.Anonymous, SystemInfo.deviceUniqueIdentifier, (loginResultCode) =>
            {
                if (loginResultCode == LoginControl.ClientLoginCode.LoginSuccess)
                {   
                            UserManager.UserInit(() =>
                            {
                                JsonObject sceneContext = UserManager.CurUserInfo.NotEndDungeon;
                                if (sceneContext != null)
                                {
                                    ///助战怪
                                    PveGameControl.CurPveData = (JsonObject)sceneContext["drop_list"];
                                    PveGameControl.CurDungeonId = sceneContext["dungeon_id"].ToString();
                                    if (GuiderLocal.TriggerPve())
                                    {
                                        PveGameControl.CurFriend = FriendInfo.GuiderFriend;
                                    }
                                    else
                                    {
                                        PveGameControl.CurFriend = new FriendInfo((JsonObject)sceneContext["friend"]);
                                    }
                                    UserPet fp = PveGameControl.CurFriend.FriendLeader;

                                    Application.LoadLevel("Pve");
                                    ApplicationControl.CurApp.StopLoading();
                                }
                                else
                                {
                                    if (UserManager.CurUserInfo.ArenaPvpTicket > 0)
                                    {
                                        Application.LoadLevel("Pvp");
                                        ApplicationControl.CurApp.StopLoading();
                                    }
                                    else
                                    {
                                        Application.LoadLevel("MainScene");
                                        ApplicationControl.CurApp.StopLoading();
                                    }
                                }
                            });
                }
            });
        };
    }
}
