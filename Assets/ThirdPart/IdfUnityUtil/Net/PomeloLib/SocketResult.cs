using UnityEngine;
using System.Collections;
using SimpleJson;
public class SocketResult {

    public enum ResultCode
    {
        Success = 200,
        TimeOut = 999,
    }

    /// <summary>
    /// 编码
    /// </summary>
    public ResultCode Code;

    public int ErrorCode;

    /// <summary>
    /// 信息
    /// </summary>
    public string Message;

    /// <summary>
    /// 返回数据
    /// </summary>
    public JsonObject Data;

    /// <summary>
    /// 返回的日期
    /// </summary>
    public string Date;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="data"></param>
    public SocketResult(JsonObject data)
    {
        Debug.Log(data);
        Code = (ResultCode)(int.Parse(data["code"].ToString()));
        ErrorCode = int.Parse(data["code"].ToString());
        //Message = data["message"].ToString();
        Data = (JsonObject)data["data"];
        if (data["date"] != null)
        {
            Date = data["date"].ToString();
        }
    }
}
