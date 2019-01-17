using System;
using System.Collections.Generic;
using System.Text;
using SimpleJson;
using UnityEngine;

namespace Pomelo.DotNetClient
{
	public class EventManager : IDisposable
	{
        private Dictionary<uint, Action<SocketResult>> callBackMap;
        private  Dictionary<uint, string> routeMap;
		private  Dictionary<string, List<Action<JsonObject>>> eventMap;

		public EventManager()
		{
            this.callBackMap = new Dictionary<uint, Action<SocketResult>>();
			this.eventMap = new  Dictionary<string, List<Action<JsonObject>>>();
            this.routeMap = new Dictionary<uint, string>();
		}

        public void AddRouteMap(uint id,string route)
        {
             this.routeMap.Add(id,route);
        }

		//Adds callback to callBackMap by id.
        public void AddCallBack(uint id, Action<SocketResult> callback)
		{
			if (id > 0 && callback != null) {
				this.callBackMap.Add(id, callback);
			}
		}

		/// <summary>
		/// Invoke the callback when the server return messge .
		/// </summary>
		/// <param name='pomeloMessage'>
		/// Pomelo message.
		/// </param>
		public void InvokeCallBack(uint id, JsonObject data)
		{
            Debug.Log(" ’µΩ∑µªÿ:" + data.ToString());

			if(!callBackMap.ContainsKey(id)) return;
            try
            {
                SocketResult result = new SocketResult(data);
                Action<SocketResult> callback = callBackMap[id];
                callBackMap.Remove(id);

                callback.Invoke(result);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            string route = routeMap[id];
            routeMap.Remove(id);
            JsonObject d = new JsonObject();
            d.Add("route", route);
            InvokeOnEvent("Invoke", d);
		}

        public void RemoveAllEventsOn()
        {
            this.eventMap.Clear();
        }

        public void ErrorCallBackAll(JsonObject data)
        {
            JsonObject errorData = new JsonObject();
            int codeInt = (int)SocketResult.ResultCode.TimeOut;
            errorData.Add("code", codeInt);
            errorData.Add("data", new JsonObject());
            foreach (uint key in callBackMap.Keys)
            {
                SocketResult result = new SocketResult(errorData);
                callBackMap[key].Invoke(result);
            }
        }

		//Adds the event to eventMap by name.
		public void AddOnEvent(string eventName, Action<JsonObject> callback)
		{
			List<Action<JsonObject>> list = null;
			if (this.eventMap.TryGetValue(eventName, out list)) {
				list.Add(callback);
			} else {
				list = new List<Action<JsonObject>>();
				list.Add(callback);
				this.eventMap.Add(eventName, list);
			}
		}

		/// <summary>
		/// If the event exists,invoke the event when server return messge.
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		///
		public void InvokeOnEvent (string route, JsonObject msg) {
			if(!this.eventMap.ContainsKey(route)) return;

			List<Action<JsonObject>> list = eventMap[route];
			foreach(Action<JsonObject> action in list) action.Invoke(msg);
		}

		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected void Dispose(bool disposing)
		{
			this.callBackMap.Clear();
			this.eventMap.Clear();
		}
	}
}

