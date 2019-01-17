using UnityEngine;
using System.Collections.Generic;

public class AStarNode
{
	/// <summary>
	/// 坐标 x
	/// </summary>
	public int nodeX;
	
	/// <summary>
	/// 坐标 y
	/// </summary>
	public int nodeY;
	
	/// <summary>
	/// 父节点
	/// </summary>
	public AStarNode parentNode;
	
	/// <summary>
	/// 二叉堆节点
	/// </summary>
	public BinaryHeapNode binaryHeapNode;
	
	/// <summary>
	/// 与此节点相邻的可通过的邻节点
	/// </summary>
	public IList<AStarLinkNode> links;
	
	/// <summary>
	/// 搜索路径的检查编号(确定是否被检查过)
	/// </summary>
	public int searchPathCheckNum;
	
	/// <summary>
	/// 可移动范围的检查编号(确定是否被检查过)
	/// </summary>
	public int walkableRangeCheckNum;
	
	/// <summary>
	/// 是否能被穿越
	/// </summary>
	private bool _walkable;
	
	/// <summary>
	/// 从此节点到目标节点的代价(A星算法使用)
	/// </summary>
	public int f;
	
	/// <summary>
	/// 从起点到此节点的代价
	/// </summary>
	public int g;

	/// <summary>
	/// 通过回调函数
	/// </summary>
	private AStarCallback aStarCallback = new AStarCallback ();

	/// <summary>
	/// 回调函数参数
	/// </summary>
	private AStarNode aStarNodeParam;

	/// <summary>
	/// 设置是否可以行走
	/// </summary>
	/// <value><c>true</c> if walkable; otherwise, <c>false</c>.</value>
	public bool walkable
	{
		get{ return this._walkable;}
		set{
			this._walkable = value;
			if(aStarCallback != null) this.aStarCallback.InvokeHeuristic(this.aStarNodeParam);
		}
	}

	/// <summary>
	/// 初始化 walkable 属性
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void InitWalkable(bool value)
	{
		this._walkable = value;
	}
	
	/// <summary>
	/// 添加穿越代价被修改后的回调函数
	/// </summary>
	/// <param name="callback">Callback.</param>
	/// <param name="aStarNodeParam">A star node parameter.</param>
	public void AddHeuristic(AStarCallback.HeuristicCallback callback, AStarNode aStarNodeParam)
	{
		this.aStarNodeParam = aStarNodeParam;
		this.aStarCallback.OnHeuristic += callback;
	}
	
	/// <summary>
	/// 移除穿越代价被修改后的回调函数
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void RemoveHeuristic(AStarCallback.HeuristicCallback callback)
	{
		this.aStarCallback.OnHeuristic -= callback;
	}
	
	/// <summary>
	/// 地图节点
	/// </summary>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeY">Node y.</param>
	public AStarNode(int nodeX, int nodeY)
	{
		this.nodeX = nodeX;
		this.nodeY = nodeY;
		
		this.walkable = true;
	}
}
