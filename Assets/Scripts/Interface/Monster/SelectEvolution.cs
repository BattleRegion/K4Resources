using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SelectEvolution : MonoBehaviour, BagInterface
{

    public BagControl evolutionBagControl;
    public Evolution evolutionDetail;
    public ButtonEvolution evolutionButton;
    public GameObject SortButton;

    void OnEnable()
    {
        evolutionBagControl.ClearBag();
        BagControl.NotInParty = true;
        evolutionBagControl.bagInter = this;
        RefreshEvolutionControl();

        UIEventListener.Get(SortButton).onClick = (g) =>
        {
            evolutionBagControl.SwitchSorting(BagControl.SortView.Evolution);
            evolutionBagControl.SetNoEvoCover();
        };
    }

    void OnDisable()
    {
        evolutionBagControl.ClearBag();
    }

    void RefreshEvolutionControl()
    {
        evolutionBagControl.SetNum(UserManager.CurUserInfo.UserPets.Count, UserManager.CurUserInfo.PetHouseLimit);
        foreach (UserPet pet in UserManager.CurUserInfo.UserPets)
        {
            if(!string.IsNullOrEmpty(pet.CurPetData.Evo))
            {
                ItemInterface i = evolutionBagControl.CreateSetItem(pet.Level, pet.CurPetData.PCost, (int)pet.CurPetData.Hp, (int)pet.CurPetData.Attack, pet.CurPetData.PetPro, pet.CurPetData.Id, pet.CurPetData.Rank, pet.UserPetId, false);
                if (string.IsNullOrEmpty(pet.CurPetData.Evo))
                {
                    i.IsCover(true);
                }
            }
        }

    }

    public UserPet evolutionPet;


    public void _OnClickItemInter(int UserMonsterID)
    {
        evolutionPet = UserManager.CurUserInfo.FindPetById(UserMonsterID);
        if (string.IsNullOrEmpty(evolutionPet.CurPetData.Evo)) return;

        evolutionDetail.SetEvolutionInfo(evolutionPet);

        evolutionButton.userPetID = UserMonsterID;
        SwitchEvolution2();
    }

    public void _OnLongPressItemInter(int UserMonsterID)
    {
    }

    public void _OnClickRemoveInter(int UserMonsterID)
    {
        return;
    }

    /// <summary>
    /// 当前场景
    /// </summary>
    public GameObject CurView;

    /// <summary>
    /// 目标场景
    /// </summary>
    public GameObject TargetView;

    /// <summary>
    /// 返回条（用于动画退出）
    /// </summary>
    public GameObject BackBar;

    /// <summary>
    /// 主要界面（动画退出）
    /// </summary>
    public GameObject MainBoard;

    public GameObject Bubble;

    public GameObject Avatar;

    public GameObject InfoText;

    void SwitchEvolution2()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
        //Bubble.SetActive(false);
        //Avatar.SetActive(false);
    }

    void SceneSwitch()
    {
        CurView.SetActive(false);
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
    }
}
