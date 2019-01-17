using UnityEngine;
using System;
using System.Collections.Generic;

public class PvpMoveManager 
{
	public static float MOVE_SPEED = 500;

	public static void Move(List<PvpEliminate> eliminateList, PvpFightUnit fightUnit, Action<PvpEliminate> callback, Action endCallback)
	{
		if(fightUnit.GetType() == typeof(PvpCharacter))
		{
			Debug.Log("Pvp Move Manager ->" + fightUnit.XPosition + ":" + fightUnit.YPosition + ":" + eliminateList.Count);
		}

		GameObject objectItem = new GameObject ();
		objectItem.name = "PvpMoveItem";
		PvpMoveItem pvpMoveItem = objectItem.AddComponent<PvpMoveItem>();
		pvpMoveItem.ChangeData (eliminateList, fightUnit, callback, endCallback, true);
	}
}

class PvpMoveItem : MonoBehaviour
{
	static readonly float originAreaWidth = 606.4f;
	static readonly float originAreaHeight = 774.4f;
	static readonly float basicWidth = 82f;
	static readonly float basicHeight = 82f;
	static readonly float xScale = 8.2f;
	static readonly float yScale = 8.2f;
	static readonly float yOffset = 2;
	static readonly float xOffset = 2;

	//private float speed = 500f; //300f

	private List<PvpEliminate> eliminateList;
	private List<Vector3> pointList;
	private PvpFightUnit fightUnit;
	private Action<PvpEliminate> callback;
	private Action endCallback;
	
	private Vector3 pathPoint;
	private Vector3 direction;

	private Vector3 orignPosition = new Vector3(-1, -1, -1);

	private int moveIndex;
	private bool moveStatus;
	private bool initStatus;

	private bool autoDestory;

	public void ChangeData(List<PvpEliminate> eliminateList, PvpFightUnit fightUnit, Action<PvpEliminate> callback, Action endCallback, bool autoDestory = false)
	{
		this.autoDestory = autoDestory;

		this.eliminateList = eliminateList;
		this.fightUnit = fightUnit;
		this.callback = callback;
		this.endCallback = endCallback;

		if(this.pointList == null) this.pointList = new List<Vector3>();
		this.pointList.Clear ();

		foreach(PvpEliminate eliminateItem in eliminateList)
		{
			pointList.Add(this.PositionToVector(eliminateItem.XPosition, eliminateItem.YPosition));
		}

		if(this.pointList.Count <= 1)
		{
			if(this.callback != null) this.callback(this.eliminateList[0]);
			if(this.endCallback != null) this.endCallback();
			if(this.autoDestory) GameObject.Destroy(this.gameObject);
			return;
		}

		this.moveIndex = 1;
		this.moveStatus = false;

		this.fightUnit.transform.localPosition = this.pointList [0];
		if(this.callback != null) this.callback(this.eliminateList[0]);

		this.initStatus = true;
	}

	void Update()
	{
		if(!this.initStatus) return;

		this.SetMoveDirection ();
		this.SetMovePosition ();
	}

	private void SetMoveDirection()
	{
		if (this.pointList == null || this.pointList.Count == 0) return;
		if (this.moveIndex < this.pointList.Count) 
		{
			if(!this.moveStatus)
			{
				this.pathPoint = this.pointList[this.moveIndex];
				this.direction = (this.pathPoint - this.fightUnit.transform.localPosition).normalized * PvpMoveManager.MOVE_SPEED;
				// 检查方向
				DungeonEnum.FaceDirection direction = this.fightUnit.GetTargetDirection(this.eliminateList[this.moveIndex]);
				// 设置移动
				this.fightUnit.UnitMoveDirection(direction);

				this.moveStatus = true;
			}
		}else
		{
			this.initStatus = false;
			if(this.endCallback != null) this.endCallback();
			if(this.autoDestory) GameObject.Destroy(this.gameObject);
		}
	}

	private void SetMovePosition ()
	{
		if (this.pointList == null || this.pointList.Count == 0) return;
		if (!this.IsArrivePosition ()) 
		{
			this.fightUnit.transform.localPosition += this.direction * Time.deltaTime;
			//this.fightUnit.transform.Translate(this.direction * Time.deltaTime);
		} else 
		{
			this.transform.transform.localPosition = this.pathPoint;
    
			if(this.moveStatus)
			{
				if(this.callback != null) this.callback(this.eliminateList[this.moveIndex]);
			}
			this.moveStatus = false;
			this.moveIndex ++;
		}
	}

	private Vector3 moveDirection = new Vector3 (-1f, -1f, -1f);
	private bool IsArrivePosition()
	{
		Vector3 currentDirection = (this.pathPoint - (this.fightUnit.transform.localPosition + this.direction * Time.deltaTime)).normalized;
		if (this.CalculateNormalized (currentDirection) == this.CalculateNormalized (this.direction) * -1) 
		{
			return true;
		}
		return false;
	}

	private Vector3 CalculateNormalized(Vector3 data)
	{
		Vector3 position = Vector3.zero;
		if(data.x != 0) position.x = (data.x > 0 ? 1 : -1);
		if(data.y != 0) position.y = (data.y > 0 ? 1 : -1);
		return position;
	}

	private Vector3 PositionToVector(int xPosition, int yPosition)
	{
		Vector3 v = this.CaculateRealPosition(xPosition, yPosition);
		return new Vector3(v.x, v.y - 26, 0);
	}

	private void CaculateFirstPosition()
	{
		if (this.orignPosition == new Vector3(-1, -1, -1))
		{
			this.orignPosition = new Vector3(-1 * originAreaWidth / 2 + basicWidth / 2 + xOffset + xScale, -1 * originAreaHeight / 2 + basicHeight / 2 + yOffset + yScale, 0);
		}
	}
	
	private Vector3 CaculateRealPosition(int xPosition, int yPosition)
	{
		this.CaculateFirstPosition();
		return new Vector3(this.orignPosition.x + xPosition * (basicWidth + xOffset), this.orignPosition.y + yPosition * (basicHeight + yOffset), 0);
	}
}
