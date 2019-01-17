using UnityEngine;
using System.Collections;
/// <summary>
/// 战斗中所有对象的基类
/// </summary>
public class FightElement : MonoBehaviour
{

    #region 属性
    /// <summary>
    /// 皮肤ID
    /// </summary>
    public string SkinId;
    /// <summary>
    /// X 轴坐标
    /// </summary>
    public int XPosition;

    /// <summary>
    /// Y轴坐标
    /// </summary>
    public int YPosition;


    /// <summary>
    /// 0,0点元素坐标
    /// </summary>
    private Vector3 zeroPosition = new Vector3(-9999, -9999, -9999);

    /// <summary>
    /// 2d 精灵对象
    /// </summary>
    public SpriteRenderer elementSpriteRender;

    //战斗区域大小
    static readonly public float OriginAreaWidth = 606.4f;

    static readonly public float OriginAreaHeight = 774.4f;

    public Sprite[] Sprites;
    #endregion

    #region 重写MONO
    void Start()
    {
        SetName();
    }
    #endregion

    #region 计算坐标位置
    /// <summary>
    /// 设置元素在UNITY坐标系中的显示位置
    /// </summary>
    /// <param name="xPosition">x 坐标</param>
    /// <param name="yPosition">y 坐标</param>
    public void SetPosition(int xPosition, int yPosition)
    {
        XPosition = xPosition;
        YPosition = yPosition;
        CaculateFirstPosition();
        transform.localPosition = GetBasicPosition(xPosition, yPosition);
        SetZorder();
        ResertSpritePosition();
    }


    public virtual void ResertSpritePosition()
    {
        elementSpriteRender.transform.localPosition = new Vector3(0, (elementSpriteRender.sprite.rect.height - basicHeight)/2, 0);
    }

    public virtual void SetZorder()
    {
        if (typeof(FightTile) == this.GetType())
        {
            elementSpriteRender.sortingOrder = 0;
        }
        else if (typeof(RemoveBlock) == this.GetType())
        {
            elementSpriteRender.sortingOrder = 1;
        }
        else
        {
            if (YPosition == 0)
            {
                elementSpriteRender.sortingOrder = GetZeroZorder();
            }
            else
            {
                elementSpriteRender.sortingOrder = GetZeroZorder() - 4 * YPosition;
            }
        }
    }

    int GetZeroZorder()
    {
        return 100 + XPosition;
    }

    /// <summary>
    /// 获取游戏坐标对应的显示位置
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    public Vector3 GetPosition(int xPosition, int yPosition)
    {
        Vector3 v = new Vector3(zeroPosition.x + xPosition * (basicWidth + xOffset), zeroPosition.y + yPosition * (basicHeight + yOffset) + (elementSpriteRender.sprite.rect.height - basicHeight) / 2, zeroPosition.z);
        return v;
    }

    /// <summary>
    /// 获取基础位置
    /// </summary>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    public Vector3 GetBasicPosition(int xPosition, int yPosition)
    {
        Vector3 v = new Vector3(zeroPosition.x + xPosition * (basicWidth + xOffset), zeroPosition.y + yPosition * (basicHeight + yOffset), zeroPosition.z);
        return v;
    }
    /// <summary>
    /// 计算0,0坐标点在UNITY坐标系中的位置
    /// </summary>
    float yOffset = 2;
    float xOffset = 2;
    float xScale = 8.2f;
    float yScale = 8.2f;
    float basicWidth = 82;
    protected float basicHeight = 82;
    void CaculateFirstPosition()
    {
        if (zeroPosition == new Vector3(-9999, -9999, -9999))
        {
            zeroPosition = new Vector3(-1 * FightElement.OriginAreaWidth / 2 + basicWidth / 2 + xOffset + xScale, -1 * FightElement.OriginAreaHeight / 2 + basicHeight / 2 + yOffset + yScale, 0);
        }
    }

    /// <summary>
    /// 设置名字虚方法
    /// </summary>
    public virtual void SetName()
    {

    }

    /// <summary>
    /// 设置皮肤
    /// </summary>
    /// <param name="skinId"></param>
    public virtual void SetRender(string skinId, int xPosition, int yPosition)
    {
        SkinId = skinId;
        RenderSprite();
        SetPosition(xPosition, yPosition);
    }

    /// <summary>
    /// 渲染方法
    /// </summary>
    public virtual void RenderSprite()
    {
        elementSpriteRender.sprite = Sprites[int.Parse(SkinId)];
    }

    /// <summary>
    /// 元素事件处理
    /// </summary>
    public virtual void ElementEventDeal(FightPlayer player)
    {
        player.PlayerNextAction();
    }

    public virtual void ElementBeSkilled(SkillData skill)
    {
    }

    /// <summary>
    /// 计算目标方向
    /// </summary>
    /// <returns></returns>
    public RemoveBlock.LinkDirection GetTargetDirection(FightElement target)
    {
        //左右判断
        if (YPosition == target.YPosition)
        {
            if (XPosition < target.XPosition)
            {
                return RemoveBlock.LinkDirection.Right;
            }
            else
            {
                return RemoveBlock.LinkDirection.Left;
            }
        }
        else if (XPosition == target.XPosition)
        {
            if (YPosition < target.YPosition)
            {
                return RemoveBlock.LinkDirection.Up;
            }
            else
            {
                return RemoveBlock.LinkDirection.Down;
            }
        }
        else if (XPosition < target.XPosition && YPosition < target.YPosition)
        {
            return RemoveBlock.LinkDirection.UpRight;
        }
        else if (XPosition < target.XPosition && YPosition > target.YPosition)
        {
            return RemoveBlock.LinkDirection.RightDown;
        }
        else if (XPosition > target.XPosition && YPosition < target.YPosition)
        {
            return RemoveBlock.LinkDirection.LeftUp;
        }
        else if (XPosition > target.XPosition && YPosition > target.YPosition)
        {
            return RemoveBlock.LinkDirection.LeftDown;
        }
        return RemoveBlock.LinkDirection.None;
    }

    /// <summary>
    /// 判断某个消除块是否为该块周围8块之一
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool IsNeighbour(FightElement element)
    {
        if ((element.XPosition == XPosition - 1 && element.YPosition == YPosition) ||
            (element.XPosition == XPosition - 1 && element.YPosition == YPosition + 1) ||
            (element.XPosition == XPosition - 1 && element.YPosition == YPosition - 1) ||
            (element.XPosition == XPosition + 1 && element.YPosition == YPosition) ||
            (element.XPosition == XPosition + 1 && element.YPosition == YPosition + 1) ||
            (element.XPosition == XPosition + 1 && element.YPosition == YPosition - 1) ||
            (element.XPosition == XPosition && element.YPosition == YPosition - 1) ||
             (element.XPosition == XPosition && element.YPosition == YPosition + 1))
        {
            return true;
        }
        return false;
    }
    #endregion
}
