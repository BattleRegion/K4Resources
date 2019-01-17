using UnityEngine;

public class PvpEliminateData
{
	/// <summary>
	/// X 位置
	/// </summary>
	public int x;

	/// <summary>
	/// Y 位置
	/// </summary>
	public int y;

	/// <summary>
	/// 颜色
	/// </summary>
	public int color;

	/// <summary>
	/// 原来颜色
	/// </summary>
	public int orgi;

	public PvpEliminateData(int x, int y, int color = -1, int orgi = -1)
	{
		this.x = x;
		this.y = y;
		this.color = color;
		this.orgi = orgi;
	}
}
