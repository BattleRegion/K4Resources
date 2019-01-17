using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//所有对象的基类用于定位
public class PvpGameObject : MonoBehaviour
{
    #region 属性
    public int XPosition;

    public int YPosition;

    public int XRange;

    public int YRange;

    public GameObject RenderObject;

    /// <summary>
    /// 战斗区域大小
    /// </summary>
    static readonly public float originAreaWidth = 606.4f;

    static readonly public float originAreaHeight = 774.4f;

    /// <summary>
    /// 元素间隔
    /// </summary>
    static readonly float yOffset = 2;

    static readonly float xOffset = 2;

    /// <summary>
    /// 基础长宽
    /// </summary>
    static readonly float basicWidth = 82;

    static readonly float basicHeight = 82;
    /// <summary>
    /// 缩放参数
    /// </summary>
    static readonly float xScale = 8.2f;

    static readonly float yScale = 8.2f;

    /// <summary>
    /// 起始位置
    /// </summary>
    Vector3 orignPosition = new Vector3(-1, -1, -1);

    public PvpGameControl GameControl;
    #endregion
    
    #region 重写MONO
    void Awake()
    {
        SetName();
        gameObject.layer = GetLayer();
        SetOrder();
    }
    #endregion

    #region 方法
    public virtual void SetPosition(int xPosition, int yPosition)
    {
        XPosition = xPosition;
        YPosition = yPosition; 
        SetName();
        transform.localPosition = CaculateRealPosition(XPosition, YPosition);
    }
    //pve control
    public PvpGameControl GameControlFun()
    {
        if (GameControl != null) return GameControl; 
        return null;
    }

    public virtual void RenderSprite()
    {

    }

    public virtual void SetName()
    {
        name = "Object:" + XPosition + "," + YPosition;
    }

    public virtual int GetLayer()
    {
        return LayerHelper.Basic;
    }

    public virtual void CleanZorder()
    {

    }

    public virtual void SetOrder()
    {
        SpriteRenderer sr = RenderObject.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 0;
    }

    public void CaculateFirstPosition()
    {
        if (orignPosition == new Vector3(-1, -1, -1))
        {
            orignPosition = new Vector3(-1 * originAreaWidth / 2 + basicWidth / 2 + xOffset + xScale, -1 * originAreaHeight / 2 + basicHeight / 2 + yOffset + yScale, 0);
        }
    }

    public virtual Vector3 CaculateRealPosition(int xPosition, int yPosition)
    {
        CaculateFirstPosition();
        return new Vector3(orignPosition.x + xPosition * (basicWidth + xOffset), orignPosition.y + yPosition * (basicHeight + yOffset), 0);
    }

    public DungeonEnum.FaceDirection GetTargetDirection(PvpGameObject target)
    {
        if (YPosition == target.YPosition && XPosition == target.XPosition)
        {
            return DungeonEnum.FaceDirection.Up;
        }
        //左右判断
        else if (YPosition == target.YPosition)
        {
            if (XPosition < target.XPosition)
            {
                return DungeonEnum.FaceDirection.Right;
            }
            else
            {
                return DungeonEnum.FaceDirection.Left;
            }
        }
        else if (XPosition == target.XPosition)
        {
            if (YPosition < target.YPosition)
            {
                return DungeonEnum.FaceDirection.Up;
            }
            else
            {
                return DungeonEnum.FaceDirection.Down;
            }
        }
        else if (XPosition < target.XPosition && YPosition < target.YPosition)
        {
            return DungeonEnum.FaceDirection.UpRight;
        }
        else if (XPosition < target.XPosition && YPosition > target.YPosition)
        {
            return DungeonEnum.FaceDirection.RightDown;
        }
        else if (XPosition > target.XPosition && YPosition < target.YPosition)
        {
            return DungeonEnum.FaceDirection.LeftUp;
        }
        else if (XPosition > target.XPosition && YPosition > target.YPosition)
        {
            return DungeonEnum.FaceDirection.LeftDown;
        }
        return DungeonEnum.FaceDirection.None;
    }

    public float Distance(PvpGameObject from, PvpGameObject to)
    {
        Vector3 vf = new Vector3(from.XPosition, from.YPosition, 0);
        Vector3 vt = new Vector3(to.XPosition, to.YPosition, 0);
        return Vector3.Distance(vf, vt);
    }

	public float Distance(int xPos, int yPos, PvpGameObject to)
	{
		Vector3 vf = new Vector3(xPos, yPos, 0);
		Vector3 vt = new Vector3(to.XPosition, to.YPosition, 0);
		return Vector3.Distance(vf, vt);
	}

    #endregion

}

