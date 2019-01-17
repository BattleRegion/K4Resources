using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class FriendInfo //好友详情
{
    /// <summary>
    /// 好友ID
    /// </summary>
    public int FriendId;

    /// <summary>
    /// 好友等级
    /// </summary>
    public int FriendLevel;

    /// <summary>
    /// 好友昵称
    /// </summary>
    public string NickName;

    /// <summary>
    /// PVP等级
    /// </summary>
    public int PvPRank;

    /// <summary>
    /// 好友队长
    /// </summary>
    public UserPet FriendLeader;

    public UserWare FriendWeapon = null;
    public UserWare FriendHelmet = null;
    public UserWare FriendArmor = null;

    public FriendInfo(JsonObject data)
    {
        if(data != null)
        {
            if (data.ContainsKey("user_id"))
            {
                FriendId = int.Parse(data["user_id"].ToString());
            }
            else
            {
                FriendId = int.Parse(data["friend_id"].ToString());
            }

            FriendLevel = int.Parse(data["level"].ToString());
            NickName = data["nickname"].ToString();
            if (data.ContainsKey("weapons"))
            {
                GetFriendWare((JsonArray)data["weapons"]);
            }
            if(data.ContainsKey("leader"))
            {
                if (data["leader"] != null)
                {
                    FriendLeader = new UserPet((JsonObject)data["leader"]);
                }
            }
            else
            {
                FriendLeader = new UserPet((JsonObject)data["pet"]);
            }
            if(data.ContainsKey("star_level"))
            {
                PvPRank = int.Parse(data["star_level"].ToString());
            }
        }
    }

    /// <summary>
    /// 作假好友
    /// </summary>
    public FriendInfo(int friendId, int Lv, string nickname, UserPet friendLeader)
    {
        FriendId = friendId;
        FriendLevel = Lv;
        NickName = nickname;
        FriendLeader = friendLeader;
    }

    /// <summary>
    /// 引导用好友
    /// </summary>
    public static FriendInfo GuiderFriend
    {
        get
        {
            return GuiderFriend_1;
        }
        set
        {
            GuiderFriend = value;
        }
    }

    static UserPet tempLeader_1 = new UserPet("P333_3", 5);
    public static FriendInfo GuiderFriend_1 = new FriendInfo(-1, 9, "未来战士", tempLeader_1);

    //static UserPet tempLeader_2 = new UserPet("P324_1", 5);
    //public static FriendInfo GuiderFriend_2 = new FriendInfo(-1, 10, "小查", tempLeader_2);

    void GetFriendWare(JsonArray wares)
    {
        foreach (JsonObject data in wares)
        {
            UserWare u = new UserWare(data);
            if (u.IsWeapon())
            {
                FriendWeapon = u;
            }
            else if (u.IsHelmet())
            {
                FriendHelmet = u;
            }
            else if(u.IsArmor())
            {
                FriendArmor = u;
            }
        }
    }
}

public class RequestInfo //请求详情
{
    /// <summary>
    /// 请求ID
    /// </summary>
    public int RequestId;

    /// <summary>
    /// UID
    /// </summary>
    public int UserId;

    /// <summary>
    /// 等级
    /// </summary>
    public int Level;

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName;

    /// <summary>
    /// PVP等级
    /// </summary>
    public int PvPRank;

    /// <summary>
    /// 队长
    /// </summary>
    public UserPet Leader = null;

    public UserWare FriendWeapon = null;
    public UserWare FriendHelmet = null;
    public UserWare FriendArmor = null;

    public RequestInfo(JsonObject data)
    {
        RequestId = int.Parse(data["request_id"].ToString());
        UserId = int.Parse(data["user_id"].ToString());
        Level = int.Parse(data["level"].ToString());
        NickName = data["nickname"].ToString();
        GetFriendWare((JsonArray)data["weapons"]);
        if (data["leader"] != null)
        {
            Leader = new UserPet((JsonObject)data["leader"]);
        }
        PvPRank = int.Parse(data["star_level"].ToString());
    }

    void GetFriendWare(JsonArray wares)
    {
        foreach (JsonObject data in wares)
        {
            UserWare u = new UserWare(data);
            if (u.IsWeapon())
            {
                FriendWeapon = u;
            }
            else if (u.IsHelmet())
            {
                FriendHelmet = u;
            }
            else if (u.IsArmor())
            {
                FriendArmor = u;
            }
        }
    }
}

public class FriendControl
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public enum FriendMessageResult
    {
        Success,
        Fail
    }

    /// <summary>
    /// 获取好友列表
    /// </summary>
    public static void GetFriendList(Action<FriendMessageResult> callback)
    {
        SocketCenter.Request(GameRouteConfig.GetFriendList, null, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r.Data);
                Loom.QueueOnMainThread(() =>
                {
                    UserManager.CurUserInfo.RefreshFriendList((JsonArray)r.Data["friends"]);
                    callback(FriendMessageResult.Success);
                });
            }
            else
            {
                Debug.Log("获取好友列表失败！");
                callback(FriendMessageResult.Fail);
            }
        }, null, true, true);
    }

    /// <summary>
    /// 发送好友请求
    /// </summary>
    public static void SendFriendRequest(int friendId, Action<FriendMessageResult> callback)
    {
        JsonObject args = new JsonObject();
        args.Add("friend_id", friendId.ToString());
        SocketCenter.Request(GameRouteConfig.SendFriendRequest, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r.Data);
                Loom.QueueOnMainThread(() =>
                {
                    callback(FriendMessageResult.Success);
                });
            }
            else
            {
                Debug.Log("发送好友请求失败");
                callback(FriendMessageResult.Fail);
            }
        }, null, true, true);
    }

    /// <summary>
    /// 处理好友请求
    /// </summary>
    public static void ReplyFriendRequest(int requestId, bool replyState, Action<FriendMessageResult> callback)
    {
        JsonObject args = new JsonObject();
        args.Add("request_id", requestId.ToString());
        if (replyState == true)
        {
            args.Add("reply_state", 1);
        }
        else
        {
            args.Add("reply_state", 0);
        }
        
        SocketCenter.Request(GameRouteConfig.ReplyFriendRequests, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r.Data);
                Loom.QueueOnMainThread(() =>
                {
                    callback(FriendMessageResult.Success);
                });
            }
            else
            {
                Debug.Log("处理好友请求失败！");
                callback(FriendMessageResult.Fail);
            }
        }, null, true, true);
    }

    /// <summary>
    /// 获取所有好友请求
    /// </summary>
    public static void GetAllRequests(Action<FriendMessageResult> callback)
    {
        SocketCenter.Request(GameRouteConfig.GetFriendRequests, null, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r.Data);
                Loom.QueueOnMainThread(() =>
                {
                    UserManager.CurUserInfo.RefreshRequestList((JsonArray)r.Data["requests"]);
                    callback(FriendMessageResult.Success);
                });
            }
            else
            {
                Debug.Log("获取好友请求失败！");
                callback(FriendMessageResult.Fail);
            }
        }, null, true, true);
    }

    /// <summary>
    /// 删除好友
    /// </summary>
    public static void RemoveFriend(int friendId, Action<FriendMessageResult> callback)
    {
        JsonObject args = new JsonObject();
        args.Add("friend_id", friendId);
        SocketCenter.Request(GameRouteConfig.RemoveFriend, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r.Data);
                Loom.QueueOnMainThread(() =>
                {
                    callback(FriendMessageResult.Success);
                });
            }
            else
            {
                Debug.Log("删除好友失败！");
                callback(FriendMessageResult.Fail);
            }
        }, null, true, true);
    }

    /// <summary>
    /// 删除好友请求
    /// </summary>
    public static void RemoveFriendRequest(List<int> requestIdList, Action<FriendMessageResult> callback)
    {
        JsonObject args = new JsonObject();
        JsonArray list = new JsonArray();
        foreach (int i in requestIdList)
        {
            list.Add(i);
        }
        args.Add("request_id", args);
        SocketCenter.Request(GameRouteConfig.RemoveRequest, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Debug.Log(r.Data);
                Loom.QueueOnMainThread(() =>
                {
                    callback(FriendMessageResult.Success);
                });
            }
            else
            {
                Debug.Log("删除好友请求失败！");
                callback(FriendMessageResult.Fail);
            }
        }, null, true, true);
    }
}
