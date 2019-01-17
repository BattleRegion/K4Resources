using SimpleJson;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PomeloSocketCenter.PomeloLib
{
    class SocketCenter
    {

        static List<string> RequestLockList = new List<string>();
        //记录当前连接
        static public SocketClient CurSocket = null;

        //当前连接的HOST
        static public string Host;

        //当前连接的端口
        static public int Port;

        //超时间隔
        static public float TimeOutInterval = 20;

        //当前连接断开事件
        static Action<JsonObject> CurDicCallback = (result) =>
        {
            Debug.Log("连接断开");
        };
        //连接
        static public void ConnectToServer(string host, int port, Action<SocketClient.ConnectResult> callback)
        {
            if (CurSocket != null)
            {
                Disconnect();
            }
            Debug.Log("准备连接服务器:" + host + ":" + port.ToString());

            Host = host;
            Port = port;
            CurSocket = new SocketClient(host, port);
            Action<JsonObject> CenterDisCallback = (disResult) =>
            {
                CurDicCallback(disResult);
                SocketCenter.Clear();
            };
            CurSocket.Connect((r) =>
            {
                callback(r);
            }, CenterDisCallback);
        }

        //清理
        static public void Clear()
        {
            RequestLockList.Clear();
            CurSocket = null;
            GC.Collect();
        }

        //断开
        static public void Disconnect()
        {
            if (CurSocket != null)
            {
                Debug.Log("主动Dis");
                CurSocket.disconnect();
                Clear();
            }
        }

        //请求时发生异常重连
        static void RequestReconnect(Action reConnectCallback)
        {
            Debug.Log("请求时检测到断开准备重连");
            ConnectToServer(Host, Port, (connectRes) =>
            {
                if (connectRes == SocketClient.ConnectResult.Success)
                {
                    Debug.Log("重连成功");
                    //重连请求
                    //重连成功后执行回调
                    JsonObject args = new JsonObject();
                    args.Add("open_id", AppMember.MemberId);
                    args.Add("credential", UserManager.CurUserInfo.PlayerCre);
                    args.Add("user_id",UserManager.CurUserInfo.UserId);
                    Request(GameRouteConfig.PlayerReconnect, args, (r) =>
                    {
                        if (r.Code == SocketResult.ResultCode.Success)
                        {
                            if (reConnectCallback != null)
                            {
                                reConnectCallback();
                            }
                        }
                        else
                        {
                            Debug.Log("请求重连失败");
                            ApplicationControl.CurApp.StopLoading();
                            Clear();
                        }
                    }, null, true, false);
                }
                else
                {
                    Debug.Log("重连失败");
                    ApplicationControl.CurApp.StopLoading();
                    Clear();
                }
            });
        }

        static bool IfLocked(string route)
        {
            if (RequestLockList.Contains(route))
            {
                return true;
            }
            return false;
        }

        static public void RemoveLocker(string route)
        {
            RequestLockList.Remove(route);
        }

        static void AddToLocker(string route)
        {
            RequestLockList.Add(route);
        }

        //发送
        static public void Request(string route, JsonObject msg, Action<SocketResult> action, Action reConnectCallback, bool needLock,bool needRemoveLoading)
        {
            ApplicationControl.CurApp.BeginLoading();
            //检查是否被锁
            if (IfLocked(route))
            {
                Debug.Log("不能连续发送" + route + "请求");
            }
            else
            {
                try
                {
                    //添加LOCK
                    if (needLock)
                    {
                        AddToLocker(route);
                    }

                    //开始请求
                    if (CurSocket != null)
                    {
                        CurSocket.request(route, msg, (r) =>
                            {
                                if (needRemoveLoading)
                                {
                                    ApplicationControl.CurApp.StopLoading();
                                }
                                ApplicationControl.CurApp.ShowErrorWindow(r);
                                action(r);                  
                            });
                    }
                    else
                    {
                        Debug.Log("NULL RECONNECT");
                        if (reConnectCallback == null)
                        {
                            reConnectCallback = () =>
                            {
                                CurSocket.request(route, msg, (r) =>
                                    {
                                        if (needRemoveLoading)
                                        {
                                            ApplicationControl.CurApp.StopLoading();
                                        }
                                        ApplicationControl.CurApp.ShowErrorWindow(r);
                                        action(r);
                                    });
                            };
                        }
                        RequestReconnect(reConnectCallback);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("ERROR RECONNECT");
                    if (reConnectCallback == null)
                    {
                        reConnectCallback = () =>
                        {
                            CurSocket.request(route, msg, (r) =>
                            {
                                if (needRemoveLoading)
                                {
                                    ApplicationControl.CurApp.StopLoading();
                                }
                                ApplicationControl.CurApp.ShowErrorWindow(r);
                                action(r);
                            });
                        };
                    }
                    RequestReconnect(reConnectCallback);
                }
            }
        }
    }
}
