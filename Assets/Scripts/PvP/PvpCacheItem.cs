using UnityEngine;
using System.Collections;

public class PvpCacheItem
{
	public object objectItem;

	public int useCount;

	public bool useStatus;

	public PvpCacheItem(object objectItem)
	{
		this.objectItem = objectItem;
	}

	/// <summary>
	/// 是否存在
	/// </summary>
	/// <param name="objectItem">Object item.</param>
	public bool Exists(object objectItem)
	{
		return this.objectItem.GetHashCode() == objectItem.GetHashCode();
	}
}
