using UnityEngine;
using System.Collections;

public class FileDownLoader {
    public static void FileDownLoad(string url, string filePath, string fileName, System.Action<HttpResult> callback,bool needMD5)
	{
		GameObject httpObject = new GameObject();
        HttpHelper hh = httpObject.AddComponent<HttpHelper>();
		if (hh == null)
		{
			httpObject.AddComponent<HttpHelper>();
			hh = httpObject.GetComponent<HttpHelper>();
		}
        hh.GET(url, (result) =>
        {
            if (result.HasError()==false)
            {
                FileHelper.StoreBytesToLocal(filePath, result.ResponseBytes());
            }

            callback.Invoke(result);

            //判断是否有ASSET ，有的话需要清除内存镜像
			/*
            if (result.ResponseAssetBundle() != null)
            {
                result.ResponseAssetBundle().Unload(true);
            }
            */

            //销毁HTTP下载对象
            MonoBehaviour.Destroy(httpObject);
        });
    }
}
