using UnityEngine;
using System.Collections;
using System;
using SimpleJson;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;

/// <summary>
/// PVP Server Connect 
/// </summary>
/// 
public class PvpConnect
{
    //0 断开 1连上
    static public int ConnectState = 0;
    static public Action<JsonObject> PvpPushHandler;


    public enum ClientPvpConnectCode
    {       
        PvpConnectToGateError = 6,//连接网关服务器失败
        PvpGetConnectorError = 7,//从网关获取游戏前台服务器失败
        PvpConnectServerError = 8,//连接pvp服务器失败,
        PvpConnectSuccess = 9//成功连接pvp服务器
    }

    //排队
    public static void EntryWaiting(Action callback = null, bool autoCo = true)
    {
        JsonObject args = new JsonObject();
        int ticketid = UserManager.CurUserInfo.ArenaPvpTicket;
        args.Add("ticket_id", ticketid);
        PvpConnect.ConnectPvpPlayerInit(() =>
        {
			if(autoCo)
			{
	            SocketCenter_pvp.Request(true, GameRouteConfig.PvpEnterWaiting, args, (result) =>
	            {
	                Debug.Log("pvp,排队 ：" + result.Data.ToString());
				}, callback, true, false, true, true);
			}
        });
    }
	public static void MovePaths(int tableId,JsonArray paths, JsonObject monsters)
    {
        JsonObject args = new JsonObject();
        args.Add("table_id",tableId );
        args.Add("steps", paths);
		args.Add ("monster", monsters);
        SocketCenter_pvp.Request(false, GameRouteConfig.PvpMovePaths, args, (r) =>
        {
			PvpGameControl.messageCallbackStatus = true;
            Debug.Log("PvpMove result :"+r.Data.ToString());
		}, null, true, true, false, true);

    }

	public static void Skill(int tableId, int house_id)
	{
		JsonObject args = new JsonObject();
		args.Add("table_id",tableId );
		args.Add("house_id", house_id);
		SocketCenter_pvp.Request(false, GameRouteConfig.PvpSkill, args, (r) =>
		                         {
			Debug.Log("PvpSkill result :"+r.Data.ToString());
		}, null, true, true, false, true);
	}
	
	public static void Surrender(int tableId)
	{
		JsonObject args = new JsonObject();
		args.Add("table_id",tableId );
		SocketCenter_pvp.Request(false, GameRouteConfig.PvpSurrender, args, (r) =>
		                         {
			Debug.Log("PvpSkill result :"+r.Data.ToString());
		}, null, true, true, false, false);
	}
	
	public static void Face(int tableId, string faceId)
	{
		JsonObject args = new JsonObject();
		args.Add("table_id",tableId );
		args.Add("face_id",faceId );
		SocketCenter_pvp.Request(false, GameRouteConfig.PvpFace, args, (r) =>
		                         {
			Debug.Log("PvpSkill result :"+r.Data.ToString());
		}, null, true, true, false, true);
	}

	public static void ReBound(int tableId, Action<JsonObject> callback)
	{
		JsonObject args = new JsonObject();
		args.Add("table_id",tableId );
		SocketCenter_pvp.Request(false, GameRouteConfig.PvpReBound, args, (r) =>
		{
			if(callback != null) callback(r.Data);
			Debug.Log("PvpReBound result :"+r.Data.ToString());
		}, null, true, true, false, true);
	}

    public static void ConnectPvpPlayerInit(Action callback)
    {
        if (PvpConnect.ConnectState == 1)
        {
            callback();
            return;
        }
        ServerInfo info = LoginControl.ServerList_pvp[Tools.GetRandom_n(LoginControl.ServerList_pvp.Count - 1)];
        PvpConnect.ConnectToPvpServer(info.Host, info.Port, (resultCode) =>
        {
            Debug.Log(" pvp server connect:   " + resultCode);
            if (resultCode == PvpConnect.ClientPvpConnectCode.PvpConnectSuccess)
            {
                JsonObject args = new JsonObject();
				args.Add("user_id",UserManager.CurUserInfo.UserId );
				args.Add("credential", UserManager.CurUserInfo.PlayerCre);
                args.Add("server_id", 1); //UserManager.CurUserInfo.Server_id);

                SocketCenter_pvp.Request(true, GameRouteConfig.PvpPlayerInit, args, (r) =>
                {
                    Debug.Log("pvp:PlayerInit  code:  " + r.Code.ToString());
                    if (r.Code == SocketResult.ResultCode.Success)
                    {
                        PvpConnect.ConnectState = 1;
                        callback();
                        Debug.Log("pvp:PlayerInit " + r.ToString());
                    }
                }, null, true, false, true, true);

                //侦听PUSH数据
                SocketCenter_pvp.CurSocket.on("TableEvent", (data) =>
                {
                    Debug.Log("Pvp Push ：" + data.ToString());
                    if (PvpPushHandler != null)
                    {
                        PvpPushHandler((JsonObject)data);
                    }
                });
            }
            else if (resultCode == PvpConnect.ClientPvpConnectCode.PvpConnectServerError)
            {
            }
            else if (resultCode == PvpConnect.ClientPvpConnectCode.PvpConnectToGateError)
            {
            }
            else if (resultCode == PvpConnect.ClientPvpConnectCode.PvpGetConnectorError)
            {
            }
        });
    }

	/// <summary>
	/// 断开函数
	/// </summary>
	public static Action<JsonObject> CurDicCallback; 

    /// <summary>
    /// 连接pvp
    /// </summary>

    static public void ConnectToPvpServer(string _host, int _port, Action<ClientPvpConnectCode> callback)
    {
		//SocketCenter_pvp.
        //连网关
        SocketCenter_pvp.ConnectToServer(_host, _port, (connectResult) =>
        {
            if (connectResult == SocketClient.ConnectResult.Success)
            {
               // (1)获取网关分配的pvp服务器
                JsonObject args1 = new JsonObject();
                args1.Add("uid", AppMember.MemberId);
				SocketCenter_pvp.Request(true, RequestRoute.GetTogetherRoute, args1, (result1) =>
                {
                    if (result1.Code == SocketResult.ResultCode.Success)
                    {
                        //(2)连接游戏pvp服务器
                        string connectorHost = result1.Data["host"].ToString();
                        int connectorPort = int.Parse(result1.Data["port"].ToString());
                        SocketCenter_pvp.ConnectToServer(connectorHost, connectorPort, (connectResult1) =>
                        {
                            if (connectResult1 == SocketClient.ConnectResult.Success)
                            {
                                callback(ClientPvpConnectCode.PvpConnectSuccess);
                            }
                            else
                            {
                                Debug.LogError("连接游戏pvp服务器失败！！！");
                                callback(ClientPvpConnectCode.PvpConnectServerError);
                            }
                        });
                   }
                    else
                    {
                        Debug.LogError("从网关获取游戏pvp服务器失败！");
                        callback(ClientPvpConnectCode.PvpGetConnectorError);
                    }
                }, null, true, false, true, true);
                   }
            else
            {
                Debug.LogError("连接网关服务器失败！");
                callback(ClientPvpConnectCode.PvpConnectToGateError);
            }
        });
    }

	public static void Disconnect()
	{
		ConnectState = 0;
		SocketCenter_pvp.Clear ();
	}
}
