using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class HelpData
{
    public enum HelperType
    {
        friend = 1,
        adventurer = 2
    }

    public int Uid;

    public int Level;

    public string NickName;

    public HelperType helpType;

    public UserPet HelpPet;

    public HelpData(JsonObject helpData)
    {
        Uid = int.Parse(helpData["user_id"].ToString());

        Level = int.Parse(helpData["level"].ToString());

        if (helpData["nickname"] != null)
        {
            NickName = helpData["nickname"].ToString();
        }
        else
        {
            NickName = "无名";
        }

        int t = int.Parse(helpData["friend_type"].ToString());

        switch (t)
        {
            case 1:
                {
                    helpType = HelperType.friend;
                    break;
                }
            case 2:
                {
                    helpType = HelperType.adventurer;
                    break;
                }
            default: break;
        }

        HelpPet = new UserPet((JsonObject)helpData["leader"]);
    }

    public static HelpData tempHelper
    {
        get
        {
            return tempHelper_1;
        }
        set
        {
            tempHelper = value;
        }
    }

    public static HelpData tempHelper_1 = new HelpData(-1, 9, "未来战士", "P333_3", 5);
    //public static HelpData tempHelper_2 = new HelpData(-1, 10, "小查", "P324_1", 5);

    /// <summary>
    /// 作假好友
    /// </summary>
    public HelpData(int uid, int friendLv, string nickname, string leaderId, int leaderLv)
    {
        Uid = uid;
        Level = friendLv;
        NickName = nickname;
        helpType = HelperType.adventurer;
        HelpPet = new UserPet(leaderId, leaderLv);
    }
}