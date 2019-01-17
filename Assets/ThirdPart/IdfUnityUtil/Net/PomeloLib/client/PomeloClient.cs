using System.Collections;
using SimpleJson;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace Pomelo.DotNetClient
{
	public class PomeloClient : IDisposable
	{
		public const string EVENT_DISCONNECT = "disconnect";

		protected EventManager eventManager;
        protected Socket socket;
        protected Protocol protocol;
		private bool disposed = false;
		private uint reqId = 1;

		private string route;

        public PomeloClient()
        {

        }

		public PomeloClient(string host, int port) 
		{
			this.eventManager = new EventManager();
			initClient(host, port);
			this.protocol = new Protocol(this, socket);
		}

		private void initClient(string host, int port) {
			this.socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
	        IPEndPoint ie=new IPEndPoint(IPAddress.Parse(host), port);
	        try {
	            this.socket.Connect(ie);
	        }
	        catch (SocketException e) {
                Debug.Log(String.Format("unable to connect to server: {0}", e.ToString()));
	            return;
	        }
		}
		
		public void connect(){
			protocol.start(null, null);
		}
		
		public void connect(JsonObject user){
			protocol.start(user, null);
		}
		
		public void connect(Action<JsonObject> handshakeCallback){
			protocol.start(null, handshakeCallback);
		}
		
		public bool connect(JsonObject user, Action<JsonObject> handshakeCallback){
			try{
				protocol.start(user, handshakeCallback);
				return true;
			}catch(Exception e){
                Debug.Log(e.ToString());
				return false;
			}
		}

        public void request(string route, Action<SocketResult> action)
        {
			this.request(route, new JsonObject(), action);
		}
		
		public void request(string route, JsonObject msg, Action<SocketResult> action)
		{
			this.route = route;
			this.eventManager.AddCallBack(reqId, action);
            this.eventManager.AddRouteMap(reqId, route);
			protocol.send (route, reqId, msg);
			reqId++;
		}
		


		public void notify(string route, JsonObject msg){
			protocol.send (route, msg);
		}
		
		public void on(string eventName, Action<JsonObject> action){
			eventManager.AddOnEvent(eventName, action);
		}

		internal void processMessage(Message msg){
			if(msg.type == MessageType.MSG_RESPONSE){
				eventManager.InvokeCallBack(msg.id, msg.data);
			}else if(msg.type == MessageType.MSG_PUSH){
				eventManager.InvokeOnEvent(msg.route, msg.data);
			}
		}

		public void disconnect(){
			Dispose ();
            eventManager.RemoveAllEventsOn();
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code 
		protected virtual void Dispose(bool disposing) {
			if(this.disposed) return;

			if (disposing) {
                try
                {
                    // free managed resources
                    this.protocol.close();
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                    this.disposed = true;
                }
                catch
                {
                    //服务器主动关闭了。。
                }
				//Call disconnect callback
				JsonObject objectItem = new JsonObject();
				objectItem.Add("route", this.route);
				eventManager.InvokeOnEvent(EVENT_DISCONNECT, objectItem);
			}
		}
	}
}

