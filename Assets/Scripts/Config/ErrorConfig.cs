using UnityEngine;
using System.Collections;
using SimpleJson;

public class ErrorConfig : GameConfig
{
    public ErrorConfig()
    {
        this.ConfigName = "ErrorCode";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            ErrorData e = new ErrorData(data);
            Configs.Add(e);
        }
    }

    public ErrorData GetErrorDataByCode(int code)
    {
        foreach (ErrorData e in Configs)
        {
            if (code == e.Code)
            {
                return e;
            }
        }
        return null;
    }
}

public class ErrorData : ConfigData
{
    public int Code;

    public string Description;

    public ErrorData(JsonObject data)
    {
        Code = int.Parse(data["Code"].ToString());
        Description = data["Message"].ToString();
    }
}
