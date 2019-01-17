using UnityEngine;
using System.Collections;

public interface FriendCellInter
{
    void OnClickFriendCell(FriendInfo f);
    void OnClickRequestCell(RequestInfo r);
}

public class FriendCell : MonoBehaviour, itemInterface
{
    public PlayerAvata FriendAvata;
    public ItemInterface FriendLeader;
    public UILabel NickName;
    public UILabel Level;
    public UILabel LastOnline;
    GameObject PetDetailView;

    public UserPet LeaderPet;

    FriendInfo Friend;

    RequestInfo Request;

    public int RequestId = -1;

    public FriendCellInter FriendInter = null;

    public FriendCellInter RequestInter = null;

    /// <summary>
    /// 初始化好友菜单条目
    /// </summary>
    /// <param name="f"></param>
    public void SetFriendCell(FriendInfo f)
    {
        FriendAvata.ClearAvata();
        if (f.FriendWeapon != null)
        {
            FriendAvata.AddAvataWare(f.FriendWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
        }
        if (f.FriendArmor != null)
        {
            FriendAvata.AddAvataWare(f.FriendArmor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        if (f.FriendHelmet != null)
        {
            FriendAvata.AddAvataWare(f.FriendHelmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        NickName.text = f.NickName;
        Level.text = "Lv." + f.FriendLevel.ToString();
        //LastOnline.text = "在线";
        if (f.FriendLeader == null)
        {
            FriendLeader.gameObject.SetActive(false);
        }
        else
        {
            LeaderPet = f.FriendLeader;
            FriendLeader.SetItem(LeaderPet);
        }
        Friend = f;
    }

    public void SetFriendCell(RequestInfo f)
    {
        if (f.FriendWeapon != null)
        {
            FriendAvata.AddAvataWare(f.FriendWeapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
        }
        if (f.FriendArmor != null)
        {
            FriendAvata.AddAvataWare(f.FriendArmor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        if (f.FriendHelmet != null)
        {
            FriendAvata.AddAvataWare(f.FriendHelmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        NickName.text = f.NickName;
        Level.text = "Lv." + f.Level.ToString();
        //LastOnline.text = "在线";
        if (f.Leader == null)
        {
            FriendLeader.gameObject.SetActive(false);
            LeaderPet = f.Leader;
        }
        else
        {
            FriendLeader.SetItem(f.Leader);
        }
        Request = f;
        RequestId = f.RequestId;
    }

    void OnEnable()
    {
        FriendLeader.itemInter = this;
    }

    void OnClick()
    {
        if (FriendInter != null)
        {
            FriendInter.OnClickFriendCell(Friend);
        }
        else
        {
            RequestInter.OnClickRequestCell(Request);
        }
    }

    public void _OnClickItem(int Uid)
    {
    }

    public void _OnLongPressItem(int Uid)
    {
        GameObject g = GameObject.Find("Detail");
        PetDetailView = g.transform.FindChild("Detail_pet").gameObject;
        if (PetDetailView != null)
        {
            PetDetailView.SetActive(true);
            PetDetailView.GetComponent<SetMonsterDetail>().SetDetail(Uid);
        }
    }
}
