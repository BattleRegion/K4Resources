using UnityEngine;
using System.Collections;
using System.IO;
public class FileHelper  {

    public static string DeviceDefaultAssetBundlePath = "";
    /// <summary>
    /// 获取设备根路径
    /// </summary>
    /// <returns>设备根路径</returns>
    public static string GetDeviceDefaultAssetBundlePath()
    {
        if (DeviceDefaultAssetBundlePath == "")
        {
            string assetBundlePath = Application.dataPath;

			/* 平台判断建议使用这种
			#if UNITY_IPHONE
				assetBundlePath = Application.persistentDataPath;
			#elif UNITY_ANDROID
				assetBundlePath = Application.persistentDataPath;
			#endif */

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                assetBundlePath = Application.persistentDataPath;
                //assetBundlePath = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
                //assetBundlePath = assetBundlePath.Substring(0, assetBundlePath.LastIndexOf('/')) + "/Documents";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                assetBundlePath = Application.persistentDataPath;
            }

            DeviceDefaultAssetBundlePath = assetBundlePath;
        }
        return DeviceDefaultAssetBundlePath;
    }

    /// <summary>
    /// 获取文件的真实路径 根路径+相对路径
    /// </summary>
    /// <param name="filePath">文件相对路径</param>
    /// <returns>文件真实路径</returns>
    public static string GetFileRealPath(string filePath)
    {
        string realPath = GetDeviceDefaultAssetBundlePath() +"/"+ filePath;
        return realPath;
    }

    public static bool FileExists(string filePath)
    {
        string realPath = GetFileRealPath(filePath);
        return File.Exists(realPath);
    }

    /// <summary>
    /// 读取文件2进制流
    /// </summary>
    /// <param name="filePath">文件相对路径</param>
    /// <returns>文件的二进制流</returns>
    private static byte[] LoadFileContent(string filePath)
    {
        try
        {
            string realPath = GetFileRealPath(filePath);
            FileInfo fi = new FileInfo(realPath);
            long fileLength = fi.Length;
             
            FileStream fs = new FileStream(realPath, FileMode.Open);
            byte[] buffer = new byte[fileLength];
            fs.Read(buffer, 0, (int)fileLength);
            fs.Close();
            fs.Dispose();
            return buffer;
        }
        catch (System.Exception e)
        {
            throw (e);
        }
    }


    public static string LoadStringFileContentFromResourceBundle(string filePath, System.Text.Encoding encode)
    {
        TextAsset t = Resources.Load(filePath) as TextAsset;
        return t.text;
    }

    /// <summary>
    /// 从本地读取一个文本文件的内容
    /// </summary>
    /// <param name="filePath">文件地址</param>
    /// <param name="encode">文件编码</param>
    /// <returns>文件String内容</returns>
    public static string LoadStringFileContent(string filePath, System.Text.Encoding encode)
    {
        try
        {
            string realPath = GetFileRealPath(filePath);
            StreamReader sr = new StreamReader(realPath, encode);
            string s =  sr.ReadToEnd();
            sr.Close();
            return s;
        }
        catch (System.Exception e)
        {
            throw (e);
        }
    }

    /// <summary>
    /// 从本地读取资源
    /// </summary>
    /// <param name="filePath">文件相对路径</param>
    /// <param name="callback">读取的回调</param>
    public static void LoadAsset(string filePath, System.Action<GameObject> callback)
    {
        string realPath = GetFileRealPath(filePath);
        realPath = "file://" + realPath;
        GameObject httpObject = MonoBehaviour.Instantiate(Resources.Load("PreFabs/Http/HttpPreFab")) as GameObject;
        HttpHelper hh = httpObject.GetComponent<HttpHelper>();
        hh.GET(realPath, (result) =>
        {
            GameObject o = Object.Instantiate(result.ResponseAssetBundle().mainAsset) as GameObject;
            callback(o);
            result.ResponseAssetBundle().Unload(true);
            MonoBehaviour.Destroy(httpObject);
        });
    }

    /// <summary>
    /// 从本地删除文件
    /// </summary>
    /// <param name="filePath">文件相对路径</param>
    public static void DeleteFromLocal(string filePath)
    {
        try
        {
            string realPath = GetFileRealPath(filePath);
            File.Delete(realPath);
        }
        catch (System.Exception e)
        {
            throw (e);
        }
    }

    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="directoryPath">文件夹相对路径</param>
    private static void CreateDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        catch (System.Exception e)
        {
            throw (e);
        }
    }

    /// <summary>
    /// 存储二进制文件流到本地
    /// </summary>
    /// <param name="filePath">文件相对路径</param>
    /// <param name="fileContent">文件二进制内容</param>
    public static void StoreBytesToLocal(string filePath, byte[] fileContent)
    {
        try
        {
            string directPath = Path.GetDirectoryName(filePath);
            string realDirectoryPath = GetFileRealPath(directPath);
            CreateDirectory(realDirectoryPath);
            File.WriteAllBytes(GetFileRealPath(filePath), fileContent);
        }
        catch (System.Exception e)
        {
            Debug.Log(filePath);
			Debug.Log("store error"+e);
            throw (e);
        }
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    /// <param name="orFilePath">文件的原始地址(相对路径)</param>
    /// <param name="newFilePath">文件需要被复制到的地址(相对路径)</param>
    public static void CopyFileToPath(string orFilePath, string newFilePath)
    {
        try
        {
            File.Copy(GetFileRealPath(orFilePath), GetFileRealPath(newFilePath));
        }
        catch (System.Exception e)
        {
            throw (e);
        }
    }
}
