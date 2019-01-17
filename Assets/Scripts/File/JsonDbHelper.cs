using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Text;
public class JsonDbHelper  {
    public const string dbBasicPath = "JsonDb";
    //"DbFile"
    /// <summary>
    /// 获取JsonDB文件相对路径
    /// </summary>
    /// <param name="dbPath">基于DBBASICPATH 的 相对路径</param>
    /// <returns></returns>
    static public string GetRealJsonDbPath(string dbPath)
    {
        string s = dbBasicPath + "/" + dbPath;
        return s;
    }

    public static bool JsonTableExist(string tablePath)
    {
        string dbPath = GetRealJsonDbPath(tablePath);
        return FileHelper.FileExists(dbPath);
    }
    public static JsonObject TotalTables;
    public static JsonArray OpenJsonTable(string tablePath)
    {
        if (tablePath == "Seed")
        {
            try
            {
                string dbPath = GetRealJsonDbPath("Seed");
                string jsonString = FileHelper.LoadStringFileContent(dbPath, Encoding.UTF8);
                return (JsonArray)SimpleJson.SimpleJson.DeserializeObject(jsonString);
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }


        if (TotalTables == null)
        {
            try
            {
                string dbPath = GetRealJsonDbPath("JsonDb");
                string jsonString = FileHelper.LoadStringFileContent(dbPath, Encoding.UTF8);
                //JsonArray TotalTables = (JsonArray)SimpleJson.SimpleJson.DeserializeObject(jsonString);
                TotalTables = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(jsonString);
                return (JsonArray)TotalTables[tablePath];
            }
            catch (System.Exception e)
            {
                throw (e);
            }
        }
        else
        {
            return (JsonArray)TotalTables[tablePath];
        }       
    }

    /// <summary>
    /// 将对象序列化成JSON存储到JSONDB目录下
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="tableObject">需要存储的对象</param>
    /// <param name="tablePath">JSON文件的相对路径</param>
    public static void SaveJsonTable(JsonObject tableObject,string tablePath)
    {
        try
        {
            string jsonString = SimpleJson.SimpleJson.SerializeObject(tableObject);
            string realPath = GetRealJsonDbPath(tablePath);
            FileHelper.StoreBytesToLocal(realPath, System.Text.Encoding.Default.GetBytes(jsonString));
        }
        catch(System.Exception e)
        {
            throw (e);
        }
    }

    /// <summary>
    /// 从JSON DB中删除一张表（JSON文件）
    /// </summary>
    /// <param name="tablePath">JSON文件相对路径</param>
    public static void DeleteJasonTable(string tablePath)
    {
        string realPath = GetRealJsonDbPath(tablePath);
        FileHelper.DeleteFromLocal(realPath);
    }
}
