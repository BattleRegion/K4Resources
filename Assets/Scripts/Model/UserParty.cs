using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class UserParty
{
    public int partyIndex;

    public UserWare weapon
    {
        get
        {
            if (weaponUid == -1) return null;
            else return UserManager.CurUserInfo.FindUserWare(weaponUid);
        }
        set
        {
            weaponUid = value.UserWareId;
        }
    }

    int weaponUid = -1;

    public UserWare armor
    {
        get
        {
            if (armorUid == -1) return null;
            else return UserManager.CurUserInfo.FindUserWare(armorUid);
        }
        set
        {
            armorUid = value.UserWareId;
        }
    }

    int armorUid = -1;

    public UserWare helmet
    {
        get
        {
            if (helmetUid == -1) return null;
            else return UserManager.CurUserInfo.FindUserWare(helmetUid);
        }
        set
        {
            helmetUid = value.UserWareId;
        }
    }

    int helmetUid = -1;

    public List<UserPet> pets
    {
        get
        {
            List<UserPet> partyPets = new List<UserPet>();
            foreach(int i in petUids)
            {
                partyPets.Add(UserManager.CurUserInfo.FindPetById(i));
            }
            return partyPets;
        }
    }

    public List<int> petUids = new List<int>();

    public UserParty(int pIndex, JsonObject data, List<UserWare> userWares, List<UserPet> userPets)
    {
        partyIndex = pIndex;

        List<UserWare> wares = new List<UserWare>();
        JsonArray equips = (JsonArray)data["weapons"];
        for (int i = 0; i < equips.Count; i++)
        {
            foreach(UserWare uw in userWares)
            {
                if (uw.UserWareId == int.Parse(equips[i].ToString()))
                {
                    wares.Add(uw);
                    break;
                }
            }
        }
        foreach(UserWare u in wares)
        {
            if((int)u.CurHardWareData.Style < 5)
            {
                weaponUid = u.UserWareId;
            }
            else if(u.CurHardWareData.Style == HardWareData.HardWareType.Head)
            {
                helmetUid = u.UserWareId;
            }
            else if(u.CurHardWareData.Style == HardWareData.HardWareType.Cuirass)
            {
                armorUid = u.UserWareId;
            }
        }

        JsonArray ps = (JsonArray)data["pets"];
        for(int i = 0; i < ps.Count; i++)
        {
            foreach(UserPet up in userPets)
            {
                if (up.UserPetId == int.Parse(ps[i].ToString()))
                {
                    petUids.Add(up.UserPetId);
                    break;
                }
            }
        }
    }

    public bool Elementconfirm(DungeonEnum.ElementAttributes element)
    {
        switch (element)
        {
            case DungeonEnum.ElementAttributes.Earth: return ElementCounter(DungeonEnum.ElementAttributes.Fire);
            case DungeonEnum.ElementAttributes.Fire: return ElementCounter(DungeonEnum.ElementAttributes.Water);
            case DungeonEnum.ElementAttributes.None: return false;
            case DungeonEnum.ElementAttributes.Water: return ElementCounter(DungeonEnum.ElementAttributes.Wind);
            case DungeonEnum.ElementAttributes.Wind: return ElementCounter(DungeonEnum.ElementAttributes.Earth);
            default: return false;
        }
    }


    bool ElementCounter(DungeonEnum.ElementAttributes element)
    {
        if (((weapon == null) ? false : (weapon.CurHardWareData.Element != element)) || ((helmet == null) ? false : (helmet.CurHardWareData.Element != element)) || ((armor == null) ? false : (armor.CurHardWareData.Element != element)))
        {
            return true;
        }
        return false;
    }
}
