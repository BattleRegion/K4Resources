using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class mViewControl : MonoBehaviour, _OnClickInter, BagInterface, EquipmentBagInterface
{
    public List<GameObject> monViews = new List<GameObject>();

    public GameObject GuiderView;

    public void SetMonViews()
    {
        foreach (GameObject g in monViews)
        {
            if (g.name == "Sprite_Main" || g.name == "Background")
            {
                if (g.activeSelf == false)
                {
                    g.SetActive(true);
                }
            }
            else if (g.activeSelf == true)
            {
                g.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        
        SetMonViews();
        foreach(SetParty s in PartyControl.setPartys)
        {
            s.partyInter = this;
        }
        EquipBagControl.bagInter = this;
        RefreshParty();
    }

    #region  队伍
    public PartyInfo PartyControl;
    public SetStrengthen StrengthenControl;
    public Sell SellControl;
    public EquipmentBagControl EquipBagControl;

    /// <summary>
    /// 每次刷新显示的Hp的增减量
    /// </summary>
    int HpBuff = 0;

    public void RefreshParty(int index = -1)
    {
        refreshFlag = false;
        if(index == -1)
        {
            foreach (SetParty s in PartyControl.setPartys)
            {
                s.bag.bagInter = this;
                s.ClearPets();
            }

            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(PartyControl.refreshParty(i, false));
            }
        }
        else
        {
            PartyControl.setPartys[index].bag.bagInter = this;
            PartyControl.setPartys[index].ClearPets();
            StartCoroutine(PartyControl.refreshParty(index, true));
        }
    }


    #region interfaces
    public void _OnClickWeaponInter(int partyIndex)
    {
        int count = 0;
        PartyControl.setPartys[partyIndex].equipmentBag.ClearBag();
        foreach (UserWare ware in UserManager.CurUserInfo.UserWares)
        {
            if (ware.IsWeapon())
            {
                equipmentItemInterface equip = PartyControl.setPartys[partyIndex].equipmentBag.AddEquipmentItem(ware.Level, ware.CurAtk, ware.CurHardWareData.Element, ware.CurHardWareData.SkinId, ware.CurHardWareData.Rank, ware.UserWareId);
                equip.IsEquip(ware.warePartys.Contains(PartyControl.curPartyIndex));
                count++;
            }
        }
        PartyControl.setPartys[partyIndex].equipmentBag.CurType = EquipmentBagControl.EquipmentBagType.weapon;
        PartyControl.setPartys[partyIndex].equipmentBag.SetNum(count, UserManager.CurUserInfo.WareLimit);
    }

    public void _OnLongPressWeaponInter(int partyIndex)
    {
    }

    public void _OnClickHelmetInter(int partyIndex)
    {
        int count = 0;
        PartyControl.setPartys[partyIndex].equipmentBag.ClearBag();
        foreach (UserWare ware in UserManager.CurUserInfo.UserWares)
        {
            if (ware.IsHelmet())
            {
                equipmentItemInterface equip = PartyControl.setPartys[partyIndex].equipmentBag.AddEquipmentItem(ware.Level, ware.CurAtk, ware.CurHardWareData.Element, ware.CurHardWareData.SkinId, ware.CurHardWareData.Rank, ware.UserWareId);
                equip.IsEquip(ware.warePartys.Contains(PartyControl.curPartyIndex));
                count++;
            }
        }
        PartyControl.setPartys[partyIndex].equipmentBag.CurType = EquipmentBagControl.EquipmentBagType.Helmet;
        PartyControl.setPartys[partyIndex].equipmentBag.SetNum(count, UserManager.CurUserInfo.WareLimit);
    }

    public void _OnLongPressHelmetInter(int partyIndex)
    {       
    }

    public void _OnClickArmorInter(int partyIndex)
    {
        int count = 0;
        PartyControl.setPartys[partyIndex].equipmentBag.ClearBag();
        foreach (UserWare ware in UserManager.CurUserInfo.UserWares)
        {
            if (ware.IsArmor())
            {
                equipmentItemInterface equip = PartyControl.setPartys[partyIndex].equipmentBag.AddEquipmentItem(ware.Level, ware.CurAtk, ware.CurHardWareData.Element, ware.CurHardWareData.SkinId, ware.CurHardWareData.Rank, ware.UserWareId);
                equip.IsEquip(ware.warePartys.Contains(PartyControl.curPartyIndex));
                count++;
            }
        }
        PartyControl.setPartys[partyIndex].equipmentBag.CurType = EquipmentBagControl.EquipmentBagType.Armor;
        PartyControl.setPartys[partyIndex].equipmentBag.SetNum(count, UserManager.CurUserInfo.WareLimit);
    }

    public void _OnLongPressArmorInter(int partyIndex)
    {

    }

    public void _OnClickPetInter(int position)
    {

    }

    public void _OnLongPressPetInter(int position,int id)
    {

    }

    public int curClickPosition;
    public int curClickPetId;
    public void _OnClickBlankInter(int partyIndex, int position, int UserPetID)
    {
        curClickPosition = position;
        curClickPetId = UserPetID;
        PartyControl.setPartys[partyIndex].bag.ClearBag();
        foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = PartyControl.setPartys[partyIndex].bag.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurHp, (int)pet.CurAtk, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
        }
        PartyControl.setPartys[partyIndex].bag.SetCost();
    }

    public void _OnLongPressBlankInter(int partyIndex, int position, int UserPetID)
    {

    }

    public void _OnClickItemInter(int UserMonsterID)
    {
        int curClickMonsterId = PartyControl.setPartys[PartyControl.curPartyIndex].returnMonsterID(curClickPosition);
        int makeMapIndex;
        int preHp;
        int laterHp;
        if (curClickMonsterId == -1)
        {
            makeMapIndex = PartyControl.setPartys[PartyControl.curPartyIndex].CurPosition;
            preHp = 0;
        }
        else
        {
            makeMapIndex = curClickPosition;
            preHp = (int)UserManager.CurUserInfo.FindPetById(curClickMonsterId).CurHp;
        }
        laterHp = (int)UserManager.CurUserInfo.FindPetById(UserMonsterID).CurHp;
        HpBuff = laterHp - preHp;
        PartyControl.setPartys[PartyControl.curPartyIndex].ClearHpBuff = false;

        UserManager.CurUserInfo.MakeTeam(() =>
        {
            Loom.QueueOnMainThread(() =>
            {
                RefreshParty(PartyControl.curPartyIndex);
                PartyControl.setPartys[PartyControl.curPartyIndex].bag.SwitchPartyPanel();
            });
        }, UserMonsterID, makeMapIndex - 1, curClickPetId, PartyControl.curPartyIndex);
    }

    public void _OnLongPressItemInter(int UserMonsterID)
    {

    }

    public void _OnClickRemoveInter(int UserMonsterID)
    {
        HpBuff = -((int)UserManager.CurUserInfo.FindPetById(UserMonsterID).CurHp);
        PartyControl.setPartys[PartyControl.curPartyIndex].ClearHpBuff = false;
        UserManager.CurUserInfo.MakeTeam(() =>
        {
            Loom.QueueOnMainThread(() =>
            {

                RefreshParty(PartyControl.curPartyIndex);
                PartyControl.setPartys[PartyControl.curPartyIndex].bag.SwitchPartyPanel();
            });
        }, UserMonsterID, -1, UserMonsterID, PartyControl.curPartyIndex);
    }

    public void _OnClickEquipmentItemInter(int UserEquipmentID)
    {
        JsonObject args = new JsonObject();
        UserWare uw = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
        if (uw.warePartys.Contains(PartyControl.curPartyIndex)) return;
        args.Add("house_id", UserEquipmentID);
        args.Add("weapon_id", uw.CurHardWareData.Id);
        args.Add("is_wear", 1);
        args.Add("queue_id", PartyControl.curPartyIndex);
        SocketCenter.Request(GameRouteConfig.WearWeapon, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (uw.IsWeapon())
                    {
                        UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].weapon = uw;
                    }
                    if (uw.IsHelmet())
                    {
                        UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].helmet = uw;
                    }
                    if (uw.IsArmor())
                    {
                        UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].armor = uw;
                    }
                    RefreshParty(PartyControl.curPartyIndex);
                    EquipBagControl.SwitchPartyPanel();
                });
            }
        }, null, true,true);
    }

    public void _OnLongPressEquipmentItemInter(int UserEquipmentID)
    {
    }

    public void _OnEquipmentClickRemoveInter(int UserEquipmentID)
    {
        UserWare uw = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);
        JsonObject args = new JsonObject();
        args.Add("house_id", UserEquipmentID);
        args.Add("weapon_id", uw.CurHardWareData.Id);
        args.Add("is_wear", 0);
        args.Add("queue_id", PartyControl.curPartyIndex);
        SocketCenter.Request(GameRouteConfig.WearWeapon, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (uw.IsWeapon())
                    {
                        UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].weapon = null;
                    }
                    if (uw.IsHelmet())
                    {
                        PartyControl.setPartys[PartyControl.curPartyIndex].helmet.RmHelmet();
                        PartyControl.setPartys[PartyControl.curPartyIndex].helmet.userEquipmentID = -1;
                        UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].helmet = null;
                    }
                    if (uw.IsArmor())
                    {
                        PartyControl.setPartys[PartyControl.curPartyIndex].armor.RmArmor();
                        PartyControl.setPartys[PartyControl.curPartyIndex].armor.userEquipmentID = -1;
                        UserManager.CurUserInfo.UserPartys[PartyControl.curPartyIndex].armor = null;
                    }
                    RefreshParty(PartyControl.curPartyIndex);
                    EquipBagControl.SwitchPartyPanel();
                });
            }
        }, null, true,true);
    }
    #endregion
    #endregion


    public static bool refreshFlag = false;
    void Update()
    {
        if(refreshFlag)
        {
            RefreshParty();
        }
    }
}
