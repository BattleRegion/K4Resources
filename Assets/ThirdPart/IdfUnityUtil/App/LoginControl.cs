using UnityEngine;
using System.Collections;
using System;
using SimpleJson;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;

public class ServerInfo
{
    public string ServerName;
    public string Host;
    public int Port;

    public ServerInfo(JsonObject data)
    {
        ServerName = data["name"].ToString();
        Host = data["host"].ToString();
        Port = int.Parse(data["port"].ToString());
    }

    public ServerInfo(string ip, int port)
    {
        Host = ip;
        Port = port;
    }
}

public class LoginControl  
{

    public enum RegisterCode
    {
        RegisterSuccess =1 ,
        RegisterFailed = 2
    }

    public enum ClientLoginCode
    {
        LoginConnectSuccess = 1,//连接登录服务器成功
        GetLoginServerHostFail = 2,//获取登录服务器地址失败
        ConnectToLoginServerError = 3,//连接登录服务器失败
        GetServerListError = 4,//获取服务器列表失败
        UserLoginError =5,//用户登录失败
        ConnectToGateError = 6,//连接网关服务器失败
        GetConnectorError = 7,//从网关获取游戏前台服务器失败
        ConnectToGameServerError = 8,//连接游戏前台服务器失败,
        LoginSuccess = 9//登录成功（成功连接到游戏前台服务器）
    }

    /// <summary>
    /// 服务器列表
    /// </summary>
    static public List<ServerInfo> ServerList = new List<ServerInfo>();

    static public List<ServerInfo> ServerList_pvp = new List<ServerInfo>();
    /// <summary>
    /// 当前选择的服务器
    /// </summary>
    static public ServerInfo CurServer;

    /// <summary>
    /// 登陆服务器地址
    /// </summary>
    static public string LoginHost;

    /// <summary>
    /// 获取登录服务器地址
    /// </summary>
    /// <param name="NavUrl"></param>
    static void GetLoginServerAddress(Action<HttpResult> callback)
    {
        Debug.Log("获取登录服务器地址");
 
        HttpHelper.GETRequest(ApplicationControl.CurApp.Config.NavUrl, (result) =>
        {
            callback(result);
        });
    }


    /// <summary>
    /// 连接登录服务器
    /// </summary>
    public static void ConnectToLoginServer(Action<ClientLoginCode> callback)
    {
        //(1)从导航获取登录服务器地址
        GetLoginServerAddress((result) =>
        {
            if (!result.HasError())
            {
                string loginServerInfoStr = result.ResponseString();
                JsonObject loginServerInfo = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(loginServerInfoStr);
                string hostStr = loginServerInfo["Host"].ToString();
                string[] hostInfo = hostStr.Split(':');
                LoginHost = hostStr;
                Debug.Log(result.ResponseString());

                //连接登陆服务器获取服务器列表
                HttpHelper.GETRequest("http://" + hostStr + "/gate?" + ApplicationControl.CurApp.Config.InfoParam, (hResult) =>
                {
                    if (!hResult.HasError())
                    {
                        JsonObject loginServerInfo_gate = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(hResult.ResponseString());

                        //Debug.Log("----- " + hResult.ResponseString());
                        JsonArray gateList = (JsonArray)SimpleJson.SimpleJson.DeserializeObject(loginServerInfo_gate["pve_gates"].ToString());
                        foreach (JsonObject j in gateList)
                        {
                            ServerInfo s = new ServerInfo(j);
                            ServerList.Add(s);
                        }
                        JsonArray gateList_pvp = (JsonArray)SimpleJson.SimpleJson.DeserializeObject(loginServerInfo_gate["pvp_gates"].ToString());
                        foreach (JsonObject j in gateList_pvp)
                        {
                            ServerInfo s = new ServerInfo(j);
                            ServerList_pvp.Add(s);
                        }

                        Debug.Log("服务器列表：" + gateList);
                        callback(ClientLoginCode.LoginConnectSuccess);
                    }
                    else
                    {
                        Debug.LogError("获取网关服务器列表error！");
                        callback(ClientLoginCode.ConnectToGateError);
                    }
                });
            }
            else
            {
                Debug.LogError("获取登录服务器地址Error!");
                callback(ClientLoginCode.GetLoginServerHostFail);
            }
        });

        //string host = GameConfig.LoginHost;
        //int port = GameConfig.LoginPort;
        //(2)连接登录服务器
        //SocketCenter.ConnectToServer(host, port, (connectResult) =>
        //{
        //    if (connectResult == SocketClient.ConnectResult.Success)
        //    {
        //        //获取服务器列表
        //        SocketCenter.Request(RequestRoute.GetAllServer, null, (r) =>
        //        {
        //            if (r.Code == SocketResult.ResultCode.Success)
        //            {
        //                ServerList.Clear();
        //                JsonArray dataList = (JsonArray)r.Data["server"];
        //                for (int i = 0; i < dataList.Count; i++)
        //                {
        //                    JsonObject data = (JsonObject)dataList[i];
        //                    ServerInfo info = new ServerInfo(data);
        //                    ServerList.Add(info);
        //                }
        //                callback(ClientLoginCode.LoginConnectSuccess);
        //            }
        //            else
        //            {
        //                Debug.LogError("获取服务器列表失败");
        //                callback(ClientLoginCode.GetServerListError);
        //            }
        //        }, null, true, false);
        //    }
        //    else
        //    {
        //        Debug.LogError("连接登录服务器失败");
        //        callback(ClientLoginCode.ConnectToLoginServerError);
        //    }
        //});
    }

    /// <summary>
    /// 选择服务器
    /// </summary>
    static public void SelectServer(ServerInfo server)
    {
        CurServer = server;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <param name="?"></param>
    /// <param name="type"></param>
    /// <param name="deviceId"></param>
    static public void Register(string account,string password,AppMember.AccountType type,string deviceId,Action<RegisterCode> callback)
    {
        //JsonObject args = new JsonObject();
        //args.Add("account", account);
        //args.Add("password", password);
        //args.Add("sourceType", type);
        //args.Add("device_identifier", deviceId);
        //SocketCenter.Request(RequestRoute.UserRegisterRoute, args, (result) =>
        //{
        //    if (result.Code == SocketResult.ResultCode.Success)
        //    {
        //        callback(RegisterCode.RegisterSuccess);
        //    }
        //    else
        //    {
        //        callback(RegisterCode.RegisterFailed);
        //    }
        //}, null, true,false);

        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("account", account);
        args.Add("password", password);
        args.Add("source_type", ((int)type).ToString());
        args.Add("device_id", deviceId);
        Debug.Log("注册" + "http://" + LoginHost + RequestRoute.RegisterRoute);
        HttpHelper.POSTRequest(("http://" + LoginHost + RequestRoute.RegisterRoute), args, (r) =>
        {
            if (r.Code == HttpResult.ResultCode.Success)
            {
                Debug.Log(r.ResponseString());
                callback(RegisterCode.RegisterSuccess);
            }
            else
            {
                Debug.Log(r.ResponseString());
                Debug.LogError("注册失败！");
                callback(RegisterCode.RegisterFailed);
            }
        });
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    static public void Login(string account,string password,AppMember.AccountType type,string deviceId,Action<ClientLoginCode> callback)
    {
        //JsonObject args = new JsonObject();
        //args.Add("account", account);
        //args.Add("password", password);
        //args.Add("sourceType", type);
        //SocketCenter.Request(RequestRoute.UserLoginRoute, args, (result) =>
        //{
        //    if (result.Code == SocketResult.ResultCode.Success)
        //    {
        //        //设置APPMENBER 数据
        //        AppMember.Cre = result.Data["cre"].ToString();
        //        AppMember.Account = result.Data["account"].ToString();
        //        AppMember.MemberId = int.Parse(result.Data["membership_id"].ToString());
        //        AppMember.Type = (AppMember.AccountType)(int.Parse(result.Data["accountType"].ToString()));
        //        //(1)登录成功连接网关服务器
        //        SocketCenter.ConnectToServer(CurServer.Host, CurServer.Port, (connectResult) =>
        //        {
        //            if (connectResult == SocketClient.ConnectResult.Success)
        //            {
        //                //(1)获取网关分配的游戏前台服务器
        //                JsonObject args1 = new JsonObject();
        //                args1.Add("uid", AppMember.MemberId);
        //                SocketCenter.Request(RequestRoute.GetConnectorRoute, args1, (result1) =>
        //                {
        //                    if (result1.Code == SocketResult.ResultCode.Success)
        //                    {
        //                        //(2)连接游戏前台服务器
        //                        string connectorHost = result1.Data["host"].ToString();
        //                        int connectorPort = int.Parse(result1.Data["port"].ToString());
        //                        SocketCenter.ConnectToServer(connectorHost,connectorPort,(connectResult1)=>
        //                        {
        //                            if(connectResult1 == SocketClient.ConnectResult.Success)
        //                            {
        //                                callback(ClientLoginCode.LoginSuccess);
        //                            }
        //                            else
        //                            {
        //                                Debug.LogError("连接游戏前台服务器失败！！！");
        //                                callback(ClientLoginCode.ConnectToGameServerError);
        //                            }
        //                        });
        //                    }
        //                    else
        //                    {
        //                        Debug.LogError("从网关获取游戏前台服务器失败！");
        //                        callback(ClientLoginCode.GetConnectorError);
        //                    }
        //                }, null, true,false);
        //            }
        //            else
        //            {
        //                Debug.LogError("连接网关服务器失败！");
        //                callback(ClientLoginCode.ConnectToGateError);
        //            }
        //        });
        //    }
        //    else
        //    {
        //        Debug.LogError(result.Code.ToString());
        //        Debug.LogError("用户登录失败！");
        //        callback(ClientLoginCode.UserLoginError);
        //    }
        //}, null, true,false);

        Dictionary<string, string> args = new Dictionary<string, string>();
        args.Add("account", account);
        args.Add("password", password);
        args.Add("source_type", ((int)type).ToString());
        args.Add("device_id", deviceId);
        Debug.Log("登陆" + "http://" + LoginHost + RequestRoute.LoginRoute);

        HttpHelper.POSTRequest(("http://" + LoginHost + RequestRoute.LoginRoute), args, (r) =>
        {
            if (r.Code == HttpResult.ResultCode.Success)
            {
                Debug.Log(r.ResponseString());
                AppMember.Cre = r.Data["credential"].ToString();
                AppMember.Account = account;
                AppMember.MemberId = int.Parse(r.Data["membership_id"].ToString());
                AppMember.Type = type;
                SocketCenter.ConnectToServer(CurServer.Host, CurServer.Port, (connectResult) =>
                {
                    if (connectResult == SocketClient.ConnectResult.Success)
                    {
                        //(1)获取网关分配的游戏前台服务器
                        JsonObject args1 = new JsonObject();
                        args1.Add("uid", AppMember.MemberId);
                        SocketCenter.Request(RequestRoute.GetConnectorRoute, args1, (result1) =>
                        {

                            if (result1.Code == SocketResult.ResultCode.Success)
                            {
                                //Debug.Log("1---" + result1.Code);
                                //(2)连接游戏前台服务器
                                string connectorHost = result1.Data["host"].ToString();
                                int connectorPort = int.Parse(result1.Data["port"].ToString());
                                SocketCenter.ConnectToServer(connectorHost, connectorPort, (connectResult1) =>
                                {
                                    if (connectResult1 == SocketClient.ConnectResult.Success)
                                    {
                                        callback(ClientLoginCode.LoginSuccess);
                                    }
                                    else
                                    {
                                        Debug.LogError("连接游戏前台服务器失败！！！");
                                        callback(ClientLoginCode.ConnectToGameServerError);
                                    }
                                });
                            }
                            else
                            {
                                Debug.LogError("从网关获取游戏前台服务器失败！");
                                callback(ClientLoginCode.GetConnectorError);
                            }
                        }, null, true, false);
                    }
                    else
                    {
                        Debug.LogError("连接网关服务器失败！");
                        callback(ClientLoginCode.ConnectToGateError);
                    }
                });
            }
            else
            {
                Debug.Log(r.ResponseString());
                Debug.LogError("登录失败！");
                callback(ClientLoginCode.UserLoginError);
            }
        });
    }
}
