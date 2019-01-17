using UnityEngine;
using System;
using System.Collections;

public class PvpTimer
{
	/// <summary>
	/// 计时行为
	/// </summary>
	private PvpTimerBehaviour timerBehaviour;

	/// <summary>
	/// 自动销毁
	/// </summary>
	private bool autoDestory;

	/// <summary>
	/// 回调
	/// </summary>
	private Action endCallback;

	/// <summary>
	/// 启动计时
	/// </summary>
	/// <param name="time">Time.</param>
	/// <param name="delayCallback">Delay callback.</param>
	/// <param name="endCallback">End callback.</param>
	/// <param name="autoDestory">If set to <c>true</c> auto destory.</param>
	public void Run(int time, Action<int> delayCallback, Action endCallback, bool autoDestory = true)
	{
		this.Stop (null);

		this.endCallback = endCallback;
		if(this.timerBehaviour == null)
		{
			GameObject timerObject = new GameObject ();
			timerObject.name = "TimerObject";
			this.timerBehaviour = timerObject.AddComponent<PvpTimerBehaviour> ();
		}

		this.timerBehaviour.Run (time, delayCallback, () => 
		{
			if(autoDestory) GameObject.Destroy(this.timerBehaviour.gameObject);
			if(this.endCallback != null) this.endCallback();
		});
	}

	/// <summary>
	/// 停止计时
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void Stop(Action callback)
	{
		if(callback != null) callback();
		if(this.timerBehaviour != null) this.timerBehaviour.Stop(); 
	}
}

class PvpTimerBehaviour : MonoBehaviour
{
	/// <summary>
	/// 时间
	/// </summary>
	public int time;

	/// <summary>
	/// 结束回调函数
	/// </summary>
	public Action endCallback;

	/// <summary>
	/// 每次回调函数
	/// </summary>
	public Action<int> delayCallback;

	/// <summary>
	/// 启动计时
	/// </summary>
	public void Run(int time, Action<int> delayCallback, Action endCallback)
	{
		this.time = time;
		this.delayCallback = delayCallback;
		this.endCallback = endCallback;

		this.RunItem ();
	}

	/// <summary>
	/// 运行
	/// </summary>
	private void RunItem()
	{
		this.time --;
		
		if(this.delayCallback != null) this.delayCallback(this.time);
		
		if(this.time > 0)
		{
			this.Invoke("RunItem", 1f);
		}
		else
		{
			if(this.endCallback != null) this.endCallback();
		}
	}

	/// <summary>
	/// 停止计时
	/// </summary>
	public void Stop()
	{
		this.CancelInvoke ("RunItem");
	}
}
