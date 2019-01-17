using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using SimpleJson;
/** struct of htttp response */
public class HttpResult
{
    public enum ResultCode
    {
        Success = 200
    }

    /// <summary>
    /// ·µ»ØÖµ
    /// </summary>
    public ResultCode Code
    {
        get
        {
            return (ResultCode)int.Parse(Data["code"].ToString());
        }
        set { Code = value; }
    }

    public JsonObject Data
    {
        get
        {
            return (JsonObject)SimpleJson.SimpleJson.DeserializeObject(ResponseString());
        }
        set { Data = value; }
    }

	public Texture2D ResponseTexture()
	{
		return request.texture;
	}
	public string ResponseString()
	{
		return request.text;
	}
    public byte[] ResponseBytes()
	{
		return request.bytes;
	}
    public AssetBundle ResponseAssetBundle()
	{
		return request.assetBundle;
	}
	public bool HasError()
	{
		return request.error != null;
	}
	public string URL()
	{
		return request.url;
	}
	public string Error()
	{
		return request.error;
	}
	private WWW request;
	
	public HttpResult(WWW w)
	{
		request = w;
	}
}
/* this class should be used as a normal monoBeahaviour--based on Gameobject*/ 
public class HttpHelper : MonoBehaviour{

    static HttpHelper curHelper;

    void Awake()
    {
        curHelper = this;
    }

	public HttpHelper()
	{
		
	}

    static public void GETRequest(string url, Action<HttpResult> callback)
    {
        curHelper.GET(url, callback);
    }

    static public void POSTRequest(string url, Dictionary<string, string> post, Action<HttpResult> callback)
    {
        curHelper.Post(url, post, callback);
    }

	public void GET(string url,Action <HttpResult> callback)
	{
		// start a async mission
		StartCoroutine(TryGET(url,callback));
	}
	
	public void Post(string url,Dictionary<string,string> post,Action <HttpResult> callback)
	{
		// start a async mission
		StartCoroutine(TryPOST(url,post,callback));
	}
	
	IEnumerator TryGET(string url,Action <HttpResult> callback)
    {
        WWW  request = new WWW (url);
        yield return request;
		// called when request did finihsed
		HttpResult result = new HttpResult(request);
		callback.Invoke(result);
    }
	
	IEnumerator TryPOST(string url, Dictionary<string,string> post,Action <HttpResult> callback)
    {
		// create a web form for infomation posting 
        WWWForm form = new WWWForm();
		//set the key-value  
        foreach(KeyValuePair<string,string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW request = new WWW(url, form);
        yield return request;
		// called when request did finihsed
        HttpResult result = new HttpResult(request);
		callback.Invoke(result);
    }
}
