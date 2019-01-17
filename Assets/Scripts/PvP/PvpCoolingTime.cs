using UnityEngine;
using System.Collections;

public class PvpCoolingTime : MonoBehaviour
{
	private Vector3[] pathList = new Vector3[]
	{
		new Vector3(142.0124f, 1.256219f, 0f),
		new Vector3(127.5595f, -10.68284f, 0f),
		new Vector3(112.4788f, 1.256525f, 0f),
		new Vector3(94.2558f, -10.05445f, 0f),
		new Vector3(77.91827f, 1.256219f, 0f),
		new Vector3(60.95197f, -9.426068f, 0f),
		new Vector3(44.61415f, 1.256525f, 0f),
		new Vector3(28.27617f, -10.05415f, 0f),
		new Vector3(13.19537f, 1.884787f, 0f),
		new Vector3(-3.770584f, -8.797623f, 0f),
		new Vector3(-20.73682f, -6.103516e-05f, 0f),
		new Vector3(-37.07469f, -9.425763f, 0f),
		new Vector3(-53.41234f, 0.6280789f, 0f),
		new Vector3(-69.12151f, -10.05409f, 0f),
		new Vector3(-86.08744f, 0.6280179f, 0f),
		new Vector3(-103.0537f, -8.797379f, 0f),
		new Vector3(-119.3912f, 0.6283841f, 0f),
		new Vector3(-137.6143f, -9.425824f, 0f),
		new Vector3(-155.8371f, 7.540488f, 0f)
	};

	public CoolingTimeUI coolingTimeUI;
	public GameObject parentItem;

	private GameObject bombSelfItem;
	private GameObject bombEnemyItem;

	private GameObject bombItem;
	private GameObject bombEndItem;

	private float correctValue;

	private PvpTimer pvpTimer;

	private PvpGameControl gameControl;

	public void Stop()
	{
		if(this.bombSelfItem != null) this.bombSelfItem.SetActive (false);
		if(this.bombEnemyItem != null) this.bombEnemyItem.SetActive (false);

		if(this.coolingTimeUI != null) this.coolingTimeUI.ChangeProgress (1f);

		if(this.pvpTimer != null)
		{
			this.pvpTimer.Stop(null);
			if(this.gameControl != null) this.gameControl.ShowTimerData(false, -1);
		}
	}

	public void Run(float time, bool self, PvpGameControl gameControl)
	{
		this.gameControl = gameControl;
		time = time - 2;
		if(time < 0f) time = 0;

		if(this.bombItem != null)
		{
			iTween.StopByName (this.bombItem, "BobmMoveTween");
			iTween.StopByName (this.bombItem, "BobmValueTween");

			this.bombItem.SetActive(false);
		}

		if(self)
		{
			if(this.bombSelfItem == null)
			{
				this.bombSelfItem = GameObject.Instantiate(Resources.Load("PreFabs/FX/Bomb00")) as GameObject;
				this.bombSelfItem.transform.parent = this.parentItem.transform;
				this.bombSelfItem.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}else{
			if(this.bombEnemyItem == null)
			{
				this.bombEnemyItem = GameObject.Instantiate(Resources.Load("PreFabs/FX/Bomb0")) as GameObject;
				this.bombEnemyItem.transform.parent = this.parentItem.transform;
				this.bombEnemyItem.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}

		if(self)
		{
			this.bombItem = this.bombSelfItem;
		}else{
			this.bombItem = this.bombEnemyItem;
		}

		this.bombItem.SetActive (true);
		this.bombItem.transform.localPosition = new Vector3 (this.pathList [0].x, this.bombItem.transform.localPosition.y, this.bombItem.transform.localPosition.z);

		if(self)
		{
			if(this.pvpTimer != null) this.pvpTimer.Stop (null);
			if(this.pvpTimer == null) this.pvpTimer = new PvpTimer();
			this.pvpTimer.Run((int)time, (int vt)=>
			{
				if(vt <= 10)
				{
					this.gameControl.ShowTimerData(true, vt);
				}
			}, ()=>
			{
				this.gameControl.ShowTimerData(false, -1);
			});
		}else
		{
			if(this.pvpTimer != null) this.pvpTimer.Stop(null);
			this.gameControl.ShowTimerData(false, -1);
		}

		iTween.MoveTo(this.bombItem, iTween.Hash("name", "BobmMoveTween","path", PvpPathPointUnit.PointList(this.pathList, 30), "easetype", iTween.EaseType.linear, "onupdate", "OnUpdateCallback", "onupdatetarget", this.gameObject, "oncomplete", "OnCompleteCallback", "oncompletetarget", this.gameObject, "time", time, "islocal", true));
		iTween.ValueTo (this.bombItem, iTween.Hash ("name", "BobmValueTween", "from", -0.1f, "to", 0.1f, "easetype", iTween.EaseType.linear, "onupdate", "OnUpdateValueCallback", "onupdatetarget", this.gameObject, "time", time));
	}

	void OnUpdateCallback()
	{
		float totalX = Mathf.Abs(this.pathList [0].x - this.pathList [this.pathList.Length - 1].x);
		// 百分比长度
		float percentX = Mathf.Abs (this.bombItem.transform.localPosition.x - this.pathList [this.pathList.Length - 1].x);
		
		this.coolingTimeUI.ChangeProgress (percentX / totalX + this.correctValue);
	}

	void OnUpdateValueCallback(float value)
	{
		this.correctValue = value;
	}

	void OnCompleteCallback()
	{
		
	}

	public void CoolingTimeComplete()
	{
		this.bombItem.SetActive (false);
		
		if(this.bombEndItem == null)
		{
			this.bombEndItem = GameObject.Instantiate(Resources.Load("PreFabs/FX/Bomb1")) as GameObject;
			this.bombEndItem.transform.parent = this.parentItem.transform;
			this.bombEndItem.transform.localScale = new Vector3(1f, 1f, 1f);
			this.bombEndItem.transform.localPosition = this.bombItem.transform.localPosition;
		}
	}
}
