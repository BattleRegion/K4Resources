using UnityEngine;
using System.Collections;

public class PetHouseControl : MonoBehaviour, BagInterface
{
    public BagControl bagControl;

    public UIScrollView dragPanel;

    public GameObject DetailPanel;

    public GameObject SortButton;

    void OnEnable()
    {
        BagControl.NotInParty = true; //表示不是在组队界面中
        bagControl.bagInter = this;
        foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            ItemInterface item = bagControl.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurPetData.Hp, (int)pet.CurPetData.Attack, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
        }
        bagControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);

        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            bagControl.SwitchSorting();
        };
    }

    void OnDisable()
    {
        bagControl.ClearBag();
    }

    public void _OnClickItemInter(int UserMonsterID)
    {
        DetailPanel.SetActive(true);
        DetailPanel.GetComponent<SetMonsterDetail>().SetDetail(UserMonsterID);
    }

    public void _OnLongPressItemInter(int UserMonsterID)
    {
    }

    public void _OnClickRemoveInter(int UserMonsterID)
    {
    }
}
