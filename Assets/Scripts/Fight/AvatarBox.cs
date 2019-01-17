using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class AvatarBox : MonoBehaviour {

    public FightLabel PlayerLvlLabel;
    public List<PetAvatar> PetAvatars = new List<PetAvatar>();
	// Use this for initialization
	void Start () {
        int lvl = 99;
        PlayerLvlLabel.SetNum(lvl.ToString());
        int index = 4;
        foreach (UserPet up in UserManager.CurUserInfo.UserPets)
        {
            PetAvatar pa = PetAvatars[index];
            pa.UserPetId = up.UserPetId;
            //pa.SetAvatar(up.CurPetData.PetPro, up.CurPetData.HeadId);
            pa.gameObject.SetActive(true);
            index--;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 宠物准备的动画
    /// </summary>
    public void ReadyPet(MonsterData.Properties per,int showNum)
    {
        List<PetAvatar> sameAvatar = new List<PetAvatar>();
        for(int i = PetAvatars.Count - 1;i>=0;i--)
        {
            PetAvatar pet = PetAvatars[i];
            if (pet.CurPro == per &&pet.gameObject.activeSelf == true)
            {
                sameAvatar.Add(pet);
            }
        }
        int totalCount = sameAvatar.Count;
        for (int i = 0; i < totalCount; i++)
        {
            if (i < showNum)
            {
                sameAvatar[i].AvatarShow(true);
            }
        }
    }

    /// <summary>
    /// 取消准备
    /// </summary>
    public void DisReadyPet(int curCount)
    {
        List<PetAvatar> curReady = new List<PetAvatar>();
        for (int i = PetAvatars.Count - 1; i >= 0; i--)
        {
            PetAvatar pet = PetAvatars[i];
            if (pet.hasReady == true)
            {
                curReady.Add(pet);
            }
        }
        if (curReady.Count > 0 && curCount <= curReady.Count)
        {
            curReady[curReady.Count - 1].AvatarShow(false);
        }
    }

    public void DisAllReadyPet()
    {
        for (int i = PetAvatars.Count - 1; i >= 0; i--)
        {
            PetAvatar pet = PetAvatars[i];
            pet.AvatarShow(false);
        }
    }


    public void CurPetUIFly(Vector3 moveTo,Vector3 localMoveTo,Action<PetAvatar> callback)
    {
        List<PetAvatar> curReady = new List<PetAvatar>();
        for (int i = PetAvatars.Count - 1; i >= 0; i--)
        {
            PetAvatar pet = PetAvatars[i];
            if (pet.hasReady == true)
            {
                curReady.Add(pet);
            }
        }
        if (curReady.Count > 0)
        {
            curReady[0].AppearPosition = localMoveTo;
            curReady[0].PetTryFlyIntoScene(moveTo,callback);
            curReady[0].hasReady = false;
        }
    }



    public void ActionAll(bool action)
    {
        foreach (PetAvatar pet in PetAvatars)
        {
            pet.ActionShine(action);
        }
    }

    public int ReadyCount()
    {
        int readyCount = 0;
        foreach (PetAvatar pet in PetAvatars)
        {
            if (pet.hasReady)
            {
                readyCount++;
            }
        }
        return readyCount;
    }

    public PetAvatar FindPetByUserPetId(int userPetId)
    {
        foreach (PetAvatar p in PetAvatars)
        {
            if (p.UserPetId == userPetId)
            {
                return p;
            }
        }
        return null;
    }
}
