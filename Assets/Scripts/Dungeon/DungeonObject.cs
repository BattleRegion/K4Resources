using UnityEngine;
using System.Collections;
public interface ObjectInterface
{
    /// <summary>
    /// 对象从副本中销毁
    /// </summary>
    /// <param name="dungeonObject"></param>
    void ObjectDestoryFromDungeon(DungeonObject dungeonObject);
}
public class DungeonObject : MonoBehaviour
{

    #region 属性
    /// <summary>
    /// 游戏坐标系X坐标
    /// </summary>
    public int XPosition;

    /// <summary>
    /// 游戏坐标系Y坐标
    /// </summary>
    public int YPosition;

    /// <summary>
    /// 对象精灵
    /// </summary>
    public SpriteRenderer ObjectSprite;

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

    /// <summary>
    /// 副本场景控制器指针
    /// </summary>
    private DungeonScene dungeonScene;
    public DungeonScene DungeonScene
    {
        get { return dungeonScene; }
        set 
        {
            dungeonScene = value;
            ObjectHandler = dungeonScene;
        }
    }

    /// <summary>
    /// 接口指针
    /// </summary>
    public ObjectInterface ObjectHandler;

    /// <summary>
    /// 基础的精灵位置
    /// </summary>
    public Vector3 OriginSpritePosition;
    #endregion

    #region 资源指针
    /// <summary>
    /// 敌方对象消失时候的效果指针
    /// </summary>
    public GameObject DisAppearFX;
    #endregion

    #region 虚方法
    /// <summary>
    /// 设置对象名字
    /// </summary>
    public virtual void SetObjectName()
    {
        name = "Object:" + XPosition + "," + YPosition;
    }

    /// <summary>
    /// 渲染对象精灵
    /// </summary>
    public virtual void RenderObjectSprite(int xPosition,int yPosition)
    {
        SetSprite();
        SetPosition(xPosition,yPosition);
        SetObjectName();
        SetZorder();
    }

    /// <summary>
    /// 根据游戏坐标系计算对象实际
    /// </summary>
    public virtual void SetPosition(int xPosition,int yPosition)
    {
        XPosition = xPosition;
        YPosition = yPosition;
        CaculateFirstPosition();
        transform.localPosition = CaculateRealPosition(XPosition, YPosition);
    }

    /// <summary>
    /// 设置精灵
    /// </summary>
    public virtual void SetSprite()
    {
        ObjectSprite.transform.localPosition = new Vector3(0, (ObjectSprite.sprite.rect.height - basicHeight) / 2 + yOffset, 0);
        OriginSpritePosition = ObjectSprite.transform.localPosition;
    }

    /// <summary>
    /// 消失
    /// </summary>
    public virtual void ObjectDisAppear()
    {
        if (DisAppearFX != null)
        {
            GameObject disObject = Instantiate(DisAppearFX) as GameObject;
            disObject.transform.position = transform.position;
        }
        Destroy(gameObject);
    }

    public virtual void SetZorder()
    {
        if (typeof(TileBlock) == this.GetType())
        {
            ObjectSprite.sortingOrder = 0;
        }
        else if (typeof(EliminateBlock) == this.GetType())
        {
            ObjectSprite.sortingOrder = 1;
        }
        else
        {
            if (YPosition == 0)
            {
                ObjectSprite.sortingOrder = GetZeroZorder();
            }
            else
            {
                ObjectSprite.sortingOrder = GetZeroZorder() - 4 * YPosition;
            }
        }
    }

    int GetZeroZorder()
    {
        return 100 + XPosition;
    }
    #endregion

    #region 私有方法
    void CaculateFirstPosition()
    {
        if (orignPosition == new Vector3(-1, -1, -1))
        {
            orignPosition = new Vector3(-1 * originAreaWidth / 2 + basicWidth / 2 + xOffset + xScale, -1 * originAreaHeight / 2 + basicHeight / 2 + yOffset + yScale, 0);
        }
    }
    #endregion

    #region 工具方法
    public Vector3 CaculateRealPosition(int xPosition, int yPosition)
    {
        return new Vector3(orignPosition.x + xPosition * (basicWidth + xOffset), orignPosition.y + yPosition * (basicHeight + yOffset), 0);
    }

    /// <summary>
    /// 判断对象是否为另一个对象周围8个中的一个
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool isNeighbour(DungeonObject obj)
    {
        if ((obj.XPosition == XPosition - 1 && obj.YPosition == YPosition) ||
            (obj.XPosition == XPosition - 1 && obj.YPosition == YPosition + 1) ||
            (obj.XPosition == XPosition - 1 && obj.YPosition == YPosition - 1) ||
            (obj.XPosition == XPosition + 1 && obj.YPosition == YPosition) ||
            (obj.XPosition == XPosition + 1 && obj.YPosition == YPosition + 1) ||
            (obj.XPosition == XPosition + 1 && obj.YPosition == YPosition - 1) ||
            (obj.XPosition == XPosition && obj.YPosition == YPosition - 1) ||
             (obj.XPosition == XPosition && obj.YPosition == YPosition + 1))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 计算目标方向
    /// </summary>
    /// <returns></returns>
    public DungeonEnum.FaceDirection GetTargetDirection(DungeonObject target)
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

    public float Distance(DungeonObject from, DungeonObject to)
    {
        Vector3 vf = new Vector3(from.XPosition, from.YPosition, 0);
        Vector3 vt = new Vector3(to.XPosition, to.YPosition, 0);
        return Vector3.Distance(vf, vt);
    }
    #endregion
}
