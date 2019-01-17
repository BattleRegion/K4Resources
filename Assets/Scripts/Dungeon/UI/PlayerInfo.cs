using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;
public class PlayerInfo : MonoBehaviour
{
    #region 属性
    /// <summary>
    /// 怪物头像列表
    /// </summary>
    public List<PetAvata> UserPetAvatas = new List<PetAvata>();

    public InfoLabel FloorLabel;

	public InfoLabel GridLabel;

    public InfoLabel GoldLabel;

    public InfoLabel BoutLab;

    public InfoLabel BoxInfo;
    public InfoLabel EggInfo;

    public GameObject Exit_btn;

    public HpUI HpUI;

    public GameObject PowerValue;
    #endregion

    #region 重写MONO
    void Awake()
    {
        cgold = 0;
        RefreshPlayerInfo();
    }
    void Start()
    {
        //刷新金币信息
        GoldLabel.BeginX = -80;
        setResNum();
        //ShowCharCard();

        //UIEventListener.Get(Exit_btn).onClick = (g) =>
        //{
            
            //JsonObject args = new JsonObject();
            //args.Add("dungeon_id",PveGameControl.CurDungeonId  );
            //SocketCenter.Request(GameRouteConfig.leaveScene, args, (r) =>
            //{
            //    if (r.Code == SocketResult.ResultCode.Success)
            //    {
            //        //离开副本
            //        Loom.QueueOnMainThread(() =>
            //        {
            //            UserManager.CurView = ViewControl.Views.Monster;
            //            UserManager.CurUserInfo.UserPets.Remove(PveGameControl.CurFriend.FriendLeader);
            //            Application.LoadLevel("MainScene");                   
            //            ApplicationControl.CurApp.StopLoading();
            //            int guideid = UserManager.CurUserInfo.GetCurGuideID();

            //        });

            //    }
            //}, null, true, true);
        //};        
    }
    #endregion

    #region 公有方法
    /// <summary>
    /// 刷新用户信息
    /// </summary>
    void RefreshPlayerInfo()
    {
        //刷新怪物信息
        //排序怪物
        for (int i = 0; i < UserManager.CurUserInfo.UserPets.Count; i++)
        {
            for (int j = 0; j < UserManager.CurUserInfo.UserPets.Count - i - 1; j++)
            {
                UserPet p1 = UserManager.CurUserInfo.UserPets[j];
                UserPet p2 = UserManager.CurUserInfo.UserPets[j + 1];
                //if (p1.MapIndex > p2.MapIndex)
                //{
                //    UserPet tempp = p1;
                //    UserManager.CurUserInfo.UserPets[j] = UserManager.CurUserInfo.UserPets[j + 1];
                //    UserManager.CurUserInfo.UserPets[j + 1] = tempp;
                //}
            }
        }

        int index = 0;
        foreach (UserPet pet in UserManager.CurUserInfo.CurPets)
        {
            //if (pet.MapIndex != -1)
            //{
                UserPetAvatas[index].SetUserPet(pet);
                index++;
            //}
        }           
    }

	private int gridCount;
	public void SetGridValue(int count)
	{
		this.gridCount += count;
		if(this.GridLabel != null) this.GridLabel.SetNum(this.gridCount.ToString(), LayerHelper.UnitFX);
	}

    public InfoLabel powerInfo;
    public void SetPowerValue(float _pro, float currentPower=1f)
    {
        string hpString = ((int)(currentPower / 10f)).ToString();
        powerInfo.SetNum(hpString);
        powerInfo.transform.localPosition = new Vector3(210 - powerInfo.TotalWidth / 2, 1, 0);

        PowerValue.renderer.material.SetFloat("_Progress", _pro);
    }
    public void SetPowerData()
    {
        //Debug.Log("设置宠物数据 ： xx");
        foreach (PetAvata petAvata in this.UserPetAvatas)
        {
            petAvata.SetPowerData();
        }
    }
    public void RefreshCd(PveFightUnit pveFightUnit)
    {
        foreach (PetAvata petAvata in this.UserPetAvatas)
        {
            petAvata.PveRefreshCd(pveFightUnit);
        }
    }
    public void setResNum()
    {
        GoldLabel.SetNum(PveGameControl.CurTotalGode.ToString());
        setEggInfoNum(PveGameControl.CurEggNum);
        setBoxInfoNum(PveGameControl.CurBoxNum);
    }
    int cgold;
    public void setGoldLabelNum()
    {
        //金币
        GoldLabel.BeginX = -80;
        cun_n = (PveGameControl.CurTotalGode - cgold) / 8;
        //InvokeRepeating("RunNum", 0, 0.2f);
        AnimationHelper.AnimationValueTo(gameObject, cgold, PveGameControl.CurTotalGode, 1f, "onValueToUpdate", gameObject, null, null, null);
    }
    private void onValueToUpdate(float value)
    {
        int v = (int)value;
        cgold = v;
        GoldLabel.SetNum(v.ToString());
    }
    private void OnValueToCallback(float value)
    {
        
    }

    int cbox;
    int cun_n = 1;
    public void setBoxInfoNum(int n)
    {
        //宝箱
        BoxInfo.BeginX =190;                
        BoxInfo.SetNum(PveGameControl.CurBoxNum.ToString());
    }
    int cegg;
    public void setEggInfoNum(int n)
    {
        //蛋
        EggInfo.BeginX = 60;
        EggInfo.SetNum(PveGameControl.CurEggNum.ToString());
    }
    void RunNum()
    {
        //Debug.Log(cgold + "    " + PveGameControl.CurTotalGode);
        int cun = PveGameControl.CurTotalGode - cgold;   
        if (cun>0)
        {
            cgold=cgold+ cun_n ;
        }
        else
        {
            cgold = PveGameControl.CurTotalGode;
            CancelInvoke();
        }
        GoldLabel.SetNum(cgold.ToString());       
    }


    public void setBoutLabNum(int n)
    {
        //回合数设置
        
        BoutLab.BeginX = 272;
		if(n>9)BoutLab.BeginX=263;
        BoutLab.SetNum(n.ToString());
    }

    public void SetFloorShow(int curFloor,int totalFloor)
    {
        FloorLabel.BeginX = -230;
        FloorLabel.SetNum(curFloor.ToString()+"/"+totalFloor.ToString());
    }
    #endregion

    #region 头像
    /// <summary>
    /// 焦点某个属性的宠物头像
    /// </summary>
    /// <param name="UpAttribute"></param>
    public void FocusPetAvata(DungeonEnum.ElementAttributes attribute, int count)
    {
        List<PetAvata> attributePa = FindSameAttributePetAvata(attribute);



        int totalCount = count;
        if (count > attributePa.Count)
        {
            totalCount = attributePa.Count;
        }
        for (int i = 0; i < totalCount; i++)
        {
            PetAvata pa = attributePa[i];
            pa.AvatarFocus(true);
        }
    }

    public GameObject PartyBoard;
    void ShowCharCard()
    {
         
        GameObject playerAnime = Resources.Load("PreFabs/Characters/S10060") as GameObject;         
        GameObject temp = NGUITools.AddChild(PartyBoard, playerAnime);
        PlayerAvata CurPlayerAvata = temp.GetComponent<PlayerAvata>();

        PetUIController.SetLayer(temp.transform, LayerHelper.Unit);
        temp.transform.localScale = new Vector3(90, 90, 1);
        temp.transform.localPosition = new Vector3(-8f, -80f, -1);


        if (UserManager.CurUserInfo.CurWeapon != null)
        {
            string curWeaponSkinId = UserManager.CurUserInfo.CurWeapon.CurHardWareData.SkinId;
            CurPlayerAvata.AddAvataWare(curWeaponSkinId, DungeonEnum.FaceDirection.None);
        }
        if (UserManager.CurUserInfo.CurHelmet != null)
        {
            string curHelmetSkinId = UserManager.CurUserInfo.CurHelmet.CurHardWareData.SkinId;
            CurPlayerAvata.AddAvataWare(curHelmetSkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        if (UserManager.CurUserInfo.CurArmor != null)
        {
            string curArmorSkinId = UserManager.CurUserInfo.CurArmor.CurHardWareData.SkinId;
            CurPlayerAvata.AddAvataWare(curArmorSkinId, DungeonEnum.FaceDirection.LeftDown);
        }
        CurPlayerAvata.WeaponEffectShow();
        Animator ac = temp.GetComponent<Animator>();
        //ac.enabled = false;       
        ac.speed=0;
        

        //if (CurPlayerAnime != null)
        //{
        //    Destroy(CurPlayerAnime);
        //}
        //CurPlayerAnime = temp;
    }

    List<PetAvata> FindSameAttributePetAvata(DungeonEnum.ElementAttributes attribute)
    {
        List<PetAvata> attributePa = new List<PetAvata>();
        foreach (PetAvata pa in UserPetAvatas)
        {
            
            if (pa.CurPet != null)
            {
                if (pa.CurPet.CurPetData.PetPro == attribute || pa.CurPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.None)
                {
                    attributePa.Add(pa);
                }
            }
        }
        return attributePa;
    }
    //根据技能 改变属性
    public void SkillChangePetPro(List<DungeonEnum.ElementAttributes> proList,DungeonEnum.ElementAttributes curp=0)
    {
        foreach (PetAvata pa in UserPetAvatas)
        {
            if (pa.CurPet != null)
            {
                foreach (DungeonEnum.ElementAttributes a in proList)
                {
                    if (pa.CurPet.CurPetData.PetPro == a)
                    {
                        pa.CurPet.CurPetData.PetPro = DungeonEnum.ElementAttributes.None;
                    }
                }
            }
        }
    }
    //恢复属性
    public void ChangePetPro()
    {
        foreach (PetAvata pa in UserPetAvatas)
        {
            if (pa.CurPet != null)
            {
                pa.CurPet.CurPetData.PetPro = pa.CurPet.CurPetData.TempPetPro;                                   
            }
        }
    }

    public void DisFocusPetAvata(DungeonEnum.ElementAttributes attribute, int count)
    {
        List<PetAvata> attributePa = FindSameAttributePetAvata(attribute);
        //取消全部
        if (count == -1)
        {
            foreach (PetAvata pa in attributePa)
            {
                pa.AvatarFocus(false);
            }
        }
        else
        {
            List<PetAvata> curFocusPetAvatas = new List<PetAvata>();
            int focusCount = 0;
            foreach (PetAvata pa in attributePa)
            {
                if (pa.hasFocus == true)
                {
                    curFocusPetAvatas.Add(pa);
                    focusCount++;
                }
            }
            if (count < focusCount)
            {
                curFocusPetAvatas[curFocusPetAvatas.Count - 1].AvatarFocus(false);
            }
        }
    }

    /// <summary>
    /// 获取已经焦点的宠物头像
    /// </summary>
    /// <returns></returns>
    public List<PetAvata> GetFocusPetAvata()
    {
        List<PetAvata> focusPa = new List<PetAvata>();
        foreach (PetAvata pa in UserPetAvatas)
        {
            if (pa.hasFocus == true)
            {
                focusPa.Add(pa);
            }
        }
        return focusPa;
    }


    public PetAvata FindPetAvata(UserPet userPet)
    {
        foreach (PetAvata pa in UserPetAvatas)
        {
            if (pa.CurPet == userPet)
            {
                return pa;
            }
        }
        return null;
    }

    public void AvataEffectRender(bool render, UserPet userPet)
    {
        FindPetAvata(userPet).AvataEffectiveRender(render);
    }
    public void AvataProgress(float _pro, UserPet userPet)
    {
        FindPetAvata(userPet).AvataProgress(_pro);
    }
    #endregion
}
