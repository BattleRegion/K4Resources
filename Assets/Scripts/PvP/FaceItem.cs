using UnityEngine;
using System;
using System.Collections;

public class FaceItem : MonoBehaviour 
{
	private Action callback;

	public void Init(Action callback)
	{
		this.callback = callback;
	}

	public void EndCallback()
	{
		// 销毁数据
		GameObject.Destroy (this.gameObject);
		if(this.callback != null) this.callback();
	}
}
