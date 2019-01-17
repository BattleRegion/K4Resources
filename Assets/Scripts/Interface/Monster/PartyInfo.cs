using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;

public class PartyInfo : MonoBehaviour
{

    #region 控制队伍面板移动
    public GameObject PartyController;
    public K4Input partyInput;
    public List<GameObject> partys;
    public List<SetParty> setPartys;

    public GameObject button_left;
    public GameObject button_right;

    public int curPartyIndex = UserManager.CurUserInfo.CurPartyIndex;
    public bool partyLock = false;

    int shiftParameter
    {
        get
        {
            return curPartyIndex * 640;
        }
        set
        {
            shiftParameter = value;
        }
    }

    Vector3 ResetIndexPosition()
    {
        float x = PartyController.transform.localPosition.x;
        if (x > 320)
        {
            curPartyIndex = 0;
        }
        else if (x < -2560)
        {
            curPartyIndex = 4;
        }
        else
        {
            curPartyIndex = (int)(x - 320) / -640;
        }

        return GetPositionByIndex(curPartyIndex);
    }

    Vector3 GetPositionByIndex(int pIndex)
    {
        return new Vector3(-640 * pIndex, 0, 0);
    }

    //检测并关闭面板
    void SetPartyState()
    {
        float x = PartyController.transform.localPosition.x;
        if (x >= 0)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x < 0 && x > -640)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 0 || i == 1)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x == -640)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 1)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x < -640 && x > -1280)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 1 || i == 2)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x == -1280)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 2)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x < -1280 && x > -1920)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 2 || i == 3)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x == -1920)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 3)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else if (x < -1920 && x > -2560)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 3 || i == 4)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 4)
                {
                    partys[i].SetActive(true);
                }
                else
                {
                    partys[i].SetActive(false);
                }
            }
        }
    }


    bool cast; //点击按钮判断

    void UpdatePartyBoard()
    {
        if(!partyLock)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                cast = true;
            }

            if(!cast)
            {
                if (partyInput.StartPosition.y >= -450 && partyInput.StartPosition.y <= 310)
                {
                    switch (partyInput.CurInput)
                    {
                        case InputState.None:
                            {
                                break;
                            }
                        case InputState.Start:
                            {
                                break;
                            }
                        case InputState.End:
                            {

                                if (partyInput.inputTime < 0.3f)
                                {
                                    if (partyInput.direction.x == 0) return;
                                    if (partyInput.direction.x < 0)
                                    {
                                        if (curPartyIndex < 4)
                                        {
                                            ++curPartyIndex;
                                        }
                                    }
                                    else
                                    {
                                        if (curPartyIndex > 0)
                                        {
                                            --curPartyIndex;
                                        }
                                    }
                                    K4MoveHelper.MoveTo(PartyController, GetPositionByIndex(curPartyIndex), 0.2f, PartyAnime.moveEnd);
                                }
                                else
                                {
                                    Vector3 tarPosition = ResetIndexPosition();
                                    K4MoveHelper.MoveTo(PartyController, tarPosition, 0.2f, PartyAnime.moveEnd);
                                }
                                UserManager.CurUserInfo.CurPartyIndex = curPartyIndex;
                                break;
                            }
                        case InputState.Hold:
                            {
                                break;
                            }
                        case InputState.Move:
                            {
                                PartyController.transform.localPosition = new Vector3(partyInput.CurrentPosition.x - partyInput.StartPosition.x - shiftParameter, PartyController.transform.localPosition.y, PartyController.transform.localPosition.z);
                                break;
                            }
                        default: break;
                    }
                }
            }
            else
            {
                if(partyInput.CurInput == InputState.End)
                {
                    if(cast)
                    {
                        cast = false;
                    }
                }
            }
            SetPartyState();
        }
    }

    #endregion

    #region 队伍操作
    public IEnumerator refreshParty(int index, bool now)
    {
        if(now)
        {
            yield return new WaitForSeconds(0f);
        }
        else
        {
            yield return new WaitForSeconds(0.2f * (float)index);
        }

        SetParty partyInstance = setPartys[index];
        UserParty userParty = UserManager.CurUserInfo.UserPartys[index];

        int petHpAddition = 0;
        int cost = 0;

        partyInstance.ClearPets();

        foreach (UserPet pet in userParty.pets)
        {
            petHpAddition = petHpAddition + (int)(pet.CurHp);
            cost = cost + pet.CurPetData.PCost;

            partyInstance.AddPet(
                pet.Level,
                //(int)UserManager.pveUserInfo.GetPetHpUnderBuffs(pet),
                //(int)UserManager.pveUserInfo.GetPetAtkUnderBuffs(pet),
                (int)pet.CurHp,
                (int)pet.CurAtk,
                pet.CurPetData.PCost,
                pet.CurPetData.SkinId,
                pet.CurPetData.PetPro,
                pet.UserPetId);
        }

        //PveSkillManager.SkillInit(UserManager.pveUserInfo, UserManager.pveUserInfo.UserSkillList);
        //PveSkillManager.BuffListInit(UserManager.pveUserInfo);
        //UserManager.pveUserInfo.ResetSkillList();

        bool isSuit = false;
        string SuitBuff = "";

        if (userParty.helmet == null || userParty.armor == null)
        {
            isSuit = false;
        }
        else if (ConfigManager.SkillConfig.GetSkillById(userParty.helmet.CurHardWareData.SkillAffix1) != null && ConfigManager.SkillConfig.GetSkillById(userParty.armor.CurHardWareData.SkillAffix1) != null)
        {
            SkillData skill_helmet = ConfigManager.SkillConfig.GetSkillById(userParty.helmet.CurHardWareData.SkillAffix1);
            SkillData skill_armor = ConfigManager.SkillConfig.GetSkillById(userParty.armor.CurHardWareData.SkillAffix1);
            if (skill_armor.SuitSkillHardwareIds.Contains(userParty.helmet.CurHardWareData.Id) && skill_helmet.SuitSkillHardwareIds.Contains(userParty.armor.CurHardWareData.Id))
            {
                isSuit = true;
                SuitBuff = skill_armor.Name;
            }
            else
            {
                isSuit = false;
            }
        }
        else
        {
            isSuit = false;
        }

        partyInstance.SetPlayerInfo(
            "S10060",
            //(int)UserManager.pveUserInfo.CurHp,
            //(int)UserManager.pveUserInfo.CurAtk,
            //(int)UserManager.pveUserInfo.CurDef,
            (int)UserManager.CurUserInfo.GetPartyHp(index),
            (int)UserManager.CurUserInfo.GetPartyAtk(index),
            (int)UserManager.CurUserInfo.GetPartyDef(index),
            string.IsNullOrEmpty(userParty.weapon.CurHardWareData.SkillAffix1) ? "--" : ConfigManager.SkillConfig.GetSkillById(userParty.weapon.CurHardWareData.SkillAffix1).Name,
            string.IsNullOrEmpty(userParty.weapon.CurHardWareData.SkillAffix2) ? "--" : ConfigManager.SkillConfig.GetSkillById(userParty.weapon.CurHardWareData.SkillAffix2).Name,
            isSuit,
            SuitBuff,
            UserManager.CurUserInfo.CurWarfare,
            cost,
            UserManager.CurUserInfo.CurHero.Hcost);

        partyInstance.CurPlayerAvata.ClearAvata();
        if (userParty.weapon != null)
        {
            partyInstance.weapon.PartyWeaponInfo(userParty.weapon.CurHardWareData.SkinId, userParty.weapon.Level, userParty.weapon.CurHardWareData.Element, userParty.weapon.UserWareId);
            partyInstance.CurPlayerAvata.AddAvataWare(userParty.weapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
        }

        if (userParty.helmet != null)
        {
            partyInstance.helmet.PartyHelmetInfo(userParty.helmet.CurHardWareData.SkinId, userParty.helmet.Level, userParty.helmet.CurHardWareData.Element, userParty.helmet.UserWareId);
            partyInstance.CurPlayerAvata.AddAvataWare(userParty.helmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }

        if (userParty.armor != null)
        {
            partyInstance.armor.PartyArmorInfo(userParty.armor.CurHardWareData.SkinId, userParty.armor.Level, userParty.armor.CurHardWareData.Element, userParty.armor.UserWareId);
            partyInstance.CurPlayerAvata.AddAvataWare(userParty.armor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
        }
    }
    #endregion


    void OnEnable()
    {
        PartyController.transform.localPosition = GetPositionByIndex(UserManager.CurUserInfo.CurPartyIndex);
        curPartyIndex = UserManager.CurUserInfo.CurPartyIndex;


        UIEventListener.Get(button_left).onClick = (g) => 
        {
            if (curPartyIndex > 0)
            {
                --curPartyIndex;
                UserManager.CurUserInfo.CurPartyIndex = curPartyIndex;
            }
            K4MoveHelper.MoveTo(PartyController, GetPositionByIndex(curPartyIndex), 0.2f, PartyAnime.moveEnd);
        };

        UIEventListener.Get(button_right).onClick = (g) => 
        {
            if (curPartyIndex < 4)
            {
                ++curPartyIndex;
                UserManager.CurUserInfo.CurPartyIndex = curPartyIndex;
            }
            K4MoveHelper.MoveTo(PartyController, GetPositionByIndex(curPartyIndex), 0.2f, PartyAnime.moveEnd);
        };

    }

    void Update()
    {
        if (curPartyIndex == 0) button_left.SetActive(false);
        else button_left.SetActive(true);
        if (curPartyIndex == 4) button_right.SetActive(false);
        else button_right.SetActive(true);
        UpdatePartyBoard();
    }

    void OnDisable()
    {
        partyLock = false;
        if(name == "Sprite_Party")
        {
            UserManager.CurUserInfo.ChangeTeam(null, curPartyIndex);
        }
    }
}
