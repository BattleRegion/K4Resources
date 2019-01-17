using Pomelo.DotNetClient;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
namespace PomeloSocketCenter.PomeloLib
{
    class SocketClient:PomeloClient
    {
        #region 枚举
        public enum ConnectResult
        {
            Success = 1,
            Failed = 2
        }
        #endregion

        #region 属性
        /// <summary>
        /// 主机地址
        /// </summary>
        string Host;

        /// <summary>
        /// 端口
        /// </summary>
        int Port;

        /// <summary>
        /// 连接结果回调
        /// </summary>
        Action<ConnectResult> ConnectResultCallback;

        /// <summary>
        /// 连接断开的回调
        /// </summary>
        Action<JsonObject> DisConnectCallback;
        #endregion

        #region  构造函数
        public SocketClient(string host, int port)
        {
            Host = host;
            Port = port;
            this.eventManager = new EventManager();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.protocol = new Protocol(this, socket);
		}
        #endregion

        #region 连接
        public void Connect(Action<ConnectResult> callback,Action<JsonObject> dicCallback)
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(Host), Port);
            this.socket.BeginConnect(ie, new AsyncCallback(ConnectCallback), socket);
            ConnectResultCallback = callback;
            DisConnectCallback = dicCallback;
        }

        void ConnectCallback(IAsyncResult iar)
        {
            try
            {
                Socket connectSocket = (Socket)iar.AsyncState;
                connectSocket.EndConnect(iar);
                //打开断开连接监听
                this.on(EVENT_DISCONNECT, (disRes) =>
                {
                    Debug.Log("监听到连接断开" + disRes);
                    this.eventManager.ErrorCallBackAll(null);
                    if (DisConnectCallback != null)
                    {
                        DisConnectCallback(disRes);
                    }
                });

                this.on("Invoke", (data) =>
                {
                    string route = data["route"].ToString();
                    SocketCenter.RemoveLocker(route);
                });

                //定义CALLBACK
                Action<JsonObject> callback = (result) =>
                {
                    if (ConnectResultCallback != null)
                    {
                        Debug.Log("连接服务器" + Host + ":" + Port.ToString() + "成功");
                        ConnectResultCallback(ConnectResult.Success);
                    }
                };
                this.protocol.start(null,callback);
            }
            catch (Exception e)
            {
                if (ConnectResultCallback!=null)
                {
                    Debug.Log("连接服务器" + Host + ":" + Port.ToString() + "失败");
                    ConnectResultCallback(ConnectResult.Failed);
                }
            }
        }
        #endregion

        #region 发送
        public void request(string route, JsonObject msg, Action<SocketResult> action)
        {
            if (msg == null)
            {
                msg = new JsonObject();
            }
            Debug.Log("发送请求 route:" + route + "msg:" + msg.ToString());

            base.request(route, msg, action);
        }
        #endregion
    }
}
