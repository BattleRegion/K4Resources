using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 对象创建管理器，通过预设可以降低或降低短时间内创建多个对象产生的卡顿现在，推荐使用这个函数创建预设
/// </summary>
public class PvpGameObjectManager
{
	public static bool CACHE_STATUS = true;
	/// <summary>
	/// 池对象、避免重复创建对象
	/// </summary>
	private static List<PvpGameObjectPoolItem> poolItemList = new List<PvpGameObjectPoolItem>();

	public static float Second
	{
		get
		{
			return UnityEngine.Random.Range (1, 100) / 10000f;
		}
	}

	/// <summary>
	/// 添加池对象
	/// </summary>
	/// <param name="objectItem">Object item.</param>
	public static void Insert(GameObject objectItem)
	{
		if(objectItem == null) return;
		objectItem.name = "GameObjectCreate";

		PvpGameObjectPoolItem poolItem = objectItem.AddComponent<PvpGameObjectPoolItem>();
		poolItemList.Add(poolItem);
	}

	public static void SetLayer(GameObject objectItem, int layer)
	{
		Transform[] transformItemList = objectItem.GetComponentsInChildren<Transform> ();

		int transformLength = transformItemList.Length;
		for(int index = 0; index < transformLength; index ++)
		{
			transformItemList[index].gameObject.layer = layer;
		}
	}

	/// <summary>
	/// 根据路径创建对象
	/// </summary>
	/// <param name="path">Path.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	public static void Create(string path, Action<GameObject> callback, bool autoDestory = false)
	{
		PvpGameObjectPoolItem poolItem = Get ();
		if(poolItem == null)
		{
			GameObject objectItem = new GameObject();
			objectItem.name = "GameObjectCreate";
			
			poolItem = objectItem.AddComponent<PvpGameObjectPoolItem>();
			poolItemList.Add(poolItem);
		}

		poolItem.Create(path, callback, autoDestory);
	}

	/// <summary>
	/// 根据预设创建对象
	/// </summary>
	/// <param name="prefabObject">Prefab object.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	public static void Create(GameObject prefabObject, Action<GameObject> callback, bool autoDestory = false)
	{
		PvpGameObjectPoolItem poolItem = Get ();
		if(poolItem == null)
		{
			GameObject objectItem = new GameObject();
			objectItem.name = "GameObjectCreate";
			
			poolItem = objectItem.AddComponent<PvpGameObjectPoolItem>();
			poolItemList.Add(poolItem);
		}
		poolItem.Create(prefabObject, callback, autoDestory);
	}

	/// <summary>
	/// 获取空闲的池对象
	/// </summary>
	private static PvpGameObjectPoolItem Get()
	{
		foreach(PvpGameObjectPoolItem poolItem in poolItemList)
		{
			if(!poolItem.status) return poolItem;
		}
		return null;
	}

	// 销毁
	public static void Dispose()
	{
		int length = poolItemList.Count;
		for(int index = 0; index < length; index ++)
		{
			if(poolItemList[index] != null && poolItemList[index].gameObject != null)
			{
				GameObject.Destroy(poolItemList[index].gameObject);
			}
		}
		poolItemList.Clear ();
	}
}

/// <summary>
/// 创建对象
/// </summary>
class PvpGameObjectPoolItem : MonoBehaviour
{
	public bool status;

	/// <summary>
	/// 创建对象
	/// </summary>
	/// <param name="path">Path.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	public void Create(string path, Action<GameObject> callback, bool autoDestory)
	{
		this.status = true;
		this.StartCoroutine (this.CreateEnumerator(path, callback, autoDestory));
	}

	/// <summary>
	/// 创建对象
	/// </summary>
	/// <param name="prefabObject">Prefab object.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	public void Create(GameObject prefabObject, Action<GameObject> callback, bool autoDestory)
	{
		this.status = true;
		this.StartCoroutine (this.CreateEnumerator(prefabObject, callback, autoDestory));
	}

	/// <summary>
	/// 创建协程，根据路径创建预设
	/// </summary>
	/// <returns>The enumerator.</returns>
	/// <param name="path">Path.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	private IEnumerator CreateEnumerator(string path, Action<GameObject> callback, bool autoDestory)
	{
		yield return null;

		GameObject objectItem = GameObject.Instantiate(Resources.Load(path)) as GameObject;
		this.CreateEnd (objectItem, callback, autoDestory);
	}

	/// <summary>
	/// 创建协程，根据对象创建预设
	/// </summary>
	/// <returns>The enumerator.</returns>
	/// <param name="prefabObject">Prefab object.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	private IEnumerator CreateEnumerator(GameObject prefabObject, Action<GameObject> callback, bool autoDestory)
	{
		yield return null;
		
		GameObject objectItem = GameObject.Instantiate(prefabObject) as GameObject;
		this.CreateEnd (objectItem, callback, autoDestory);
	}

	/// <summary>
	/// 创建结束回调
	/// </summary>
	/// <param name="objectItem">Object item.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	private void CreateEnd(GameObject objectItem, Action<GameObject> callback, bool autoDestory)
	{
		this.status = false;

		if(callback != null) callback (objectItem);
		
		if(autoDestory)
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}
