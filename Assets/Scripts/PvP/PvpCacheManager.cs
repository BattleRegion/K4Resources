using UnityEngine;
using System.Collections.Generic;

public class PvpCacheManager
{
	public static Dictionary<string, List<PvpCacheItem>> cacheList = new Dictionary<string, List<PvpCacheItem>>();
	private static GameObject cacheItem;

	public static void Init(GameObject cacheItem)
	{
		PvpCacheManager.cacheItem = cacheItem;

		if(cacheList == null) cacheList = new Dictionary<string, List<PvpCacheItem>>();
		cacheList.Clear ();
	}

	/// <summary>
	/// 添加缓存
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="item">Item.</param>
	public static void InsertObject(string key, object objectItem)
	{
		if(string.IsNullOrEmpty(key) || objectItem == null) return;

		if(!cacheList.ContainsKey(key))
		{
			cacheList[key] = new List<PvpCacheItem>();
		}

		int index = GetIndexByList(cacheList[key], objectItem);
		if(index != -1)
		{
			cacheList[key][index].useStatus = false;
		}else
		{
			cacheList[key].Add(new PvpCacheItem(objectItem));
		}

		if(objectItem.GetType() == typeof(GameObject))
		{
			GameObject gameObjectItem = objectItem as GameObject;
			// 如果父对象为空
			if(cacheItem != null) gameObjectItem.transform.parent = cacheItem.transform;
			gameObjectItem.SetActive(false);

		}else if(objectItem.GetType() == typeof(MonoBehaviour))
		{
			MonoBehaviour monoObjectItem = objectItem as MonoBehaviour;
		}
	}

	/// <summary>
	/// 获得元素
	/// </summary>
	/// <param name="key">Key.</param>
	public static PvpCacheItem Get(string key)
	{
		if(!cacheList.ContainsKey(key)) return null;

		List<PvpCacheItem> itemList = cacheList [key];
		if(itemList == null || itemList.Count == 0) return null;

		PvpCacheItem cacheItem = GetItemByList (itemList);
		if(cacheItem != null)
		{
			// 使用次数加 1
			cacheItem.useCount ++;
			// 使用状态设置为 true
			cacheItem.useStatus = true;
		}

		return cacheItem;
	}

	/// <summary>
	/// 查找 T 类型
	/// </summary>
	/// <param name="key">Key.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T GetBehaviour<T>(string key) where T : MonoBehaviour
	{
		PvpCacheItem cacheItem = Get (key);
		if(cacheItem == null) return null;

		if(cacheItem.objectItem != null && cacheItem.objectItem.GetType() == typeof(MonoBehaviour)) return cacheItem.objectItem as T;

		return null;
	}

	public static GameObject GetObject(string key)
	{
		PvpCacheItem cacheItem = Get (key);
		if(cacheItem == null) return null;

		if(cacheItem.objectItem != null && cacheItem.objectItem.GetType() == typeof(GameObject)) return cacheItem.objectItem as GameObject;
		
		return null;
	}

	/// <summary>
	/// 在列表中查找元素
	/// </summary>
	/// <returns>The item by list.</returns>
	/// <param name="itemList">Item list.</param>
	private static PvpCacheItem GetItemByList(List<PvpCacheItem> itemList)
	{
		foreach(PvpCacheItem cacheItem in itemList)
		{
			if(!cacheItem.useStatus) return cacheItem;
		}
		return null;
	}

	/// <summary>
	/// 在列表中查找索引
	/// </summary>
	/// <returns>The index by list.</returns>
	/// <param name="itemList">Item list.</param>
	/// <param name="objectItem">Object item.</param>
	private static int GetIndexByList(List<PvpCacheItem> itemList, object objectItem)
	{
		int index = 0;
		foreach(PvpCacheItem cacheItem in itemList)
		{
			if(cacheItem.Exists(objectItem)) return index;
			index ++;
		}
		return -1;
	}

	public static void Dispose()
	{

	}
}
