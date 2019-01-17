using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class AccountLoginControl : MonoBehaviour
{
    public UIInput Account;
    public UIInput Password;

    string account
    {
        get
        {
            return Account.value;
        }
        set
        {
            Account.value = value;
        }
    }
    string password
    {
        get
        {
            return Password.value;
        }
        set
        {
            Password.value = value;
        }
    }

    void OnEnable()
    {
        string s = PlayerPrefs.GetString("account");
        if (s != null)
        {
            account = s;
        }
        s = PlayerPrefs.GetString("password");
        if (s != null)
        {
            password = s;
        }
    }

    void OnClick()
    {
        LoginControl.Login(account, password, AppMember.AccountType.Normal, SystemInfo.deviceUniqueIdentifier, (r) =>
        {
            if (r == LoginControl.ClientLoginCode.LoginSuccess)
            {
                Loom.QueueOnMainThread(() =>
                {
                    PlayerPrefs.SetString("account", account);
                    PlayerPrefs.SetString("password", password);
                });

                UserManager.UserInit(() =>
                {
                    JsonObject sceneContext = UserManager.CurUserInfo.NotEndDungeon;
                    if (sceneContext != null)
                    {
                        PveGameControl.CurPveData = (JsonObject)sceneContext["drop_list"];
                        PveGameControl.CurDungeonId = sceneContext["dungeon_id"].ToString();
                        Debug.Log(PlayerPrefs.GetInt("GuideID"));
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
                        //ArenaPvpTicketStatus  1报名  2排队中 3进行中 
                        // 状态暂时不检测了 && UserManager.CurUserInfo.ArenaPvpTicketStatus == 2
                        if (UserManager.CurUserInfo.ArenaPvpTicket > 0 && (UserManager.CurUserInfo.ArenaPvpTicketStatus == 3 || UserManager.CurUserInfo.ArenaPvpTicketStatus == 2 || UserManager.CurUserInfo.ArenaPvpTicketStatus == 1))
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
            else
            {
                Debug.Log("登录失败！");
            }

               
        });
    }
}
