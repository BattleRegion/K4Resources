using UnityEngine;
using System.Collections;

public class SelectEquipmentEvo : MonoBehaviour, EquipmentBagInterface
{
    public enum HardwareType
    {
        weapon = 0,
        armor = 1
    }


    /// <summary>
    /// 选择是哪种类型装备的List
    /// </summary>
    public HardwareType type;

    public EquipmentBagControl evolutionControl;
    public EquipmentEvolution evolutionDetail;
    public ButtonEquipmentEvolution evolutionButton;

    void OnEnable()
    {
        evolutionControl.bagInter = this;
        RefreshEvolutionControl();
        this.GetComponent<UIScrollView>().SetDragAmount(0, 0, false);
        this.GetComponent<UIScrollView>().UpdateScrollbars();
    }

    void RefreshEvolutionControl()
    {
        evolutionControl.SetNum(UserManager.CurUserInfo.UserWares.Count, UserManager.CurUserInfo.WareLimit);
        evolutionControl.ClearBag();
        switch ((int)type)
        {
            case 0:
                {
                    foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
                    {
                        if ((int)uw.CurHardWareData.Style < 5)
                        {
                            equipmentItemInterface ei = evolutionControl.AddEquipmentItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, uw.CurHardWareData.Rank, uw.UserWareId);
                            if(string.IsNullOrEmpty(uw.CurHardWareData.Evo))
                            {
                                ei.IsCover(true);
                            }
                        }
                    }
                    break;
                }
            case 1:
                {
                    foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
                    {
                        if ((int)uw.CurHardWareData.Style > 5)
                        {
                            equipmentItemInterface ei = evolutionControl.AddEquipmentItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, uw.CurHardWareData.Rank, uw.UserWareId);
                            if (string.IsNullOrEmpty(uw.CurHardWareData.Evo))
                            {
                                ei.IsCover(true);
                            }
                        }
                    }
                    break;
                }
        }
    }

    public UserWare evolutionWare;

    public void _OnClickEquipmentItemInter(int UserEquipmentID)
    {
        evolutionWare = UserManager.CurUserInfo.FindUserWare(UserEquipmentID);

        if (string.IsNullOrEmpty(evolutionWare.CurHardWareData.Evo))
        {
            return;
        }
        if ((int)evolutionWare.CurHardWareData.Style < 5)
        {
            evolutionDetail.SetEvolutionInfo(UserEquipmentID); 
        }
        else if ((int)evolutionWare.CurHardWareData.Style > 5)
        {
            evolutionDetail.SetEvolutionInfo(UserEquipmentID);
        }
        evolutionButton.userWareID = UserEquipmentID;
        SwitchEvolution2();
    }

    public void _OnLongPressEquipmentItemInter(int UserEquipmentId)
    {
    }

    public void _OnEquipmentClickRemoveInter(int UserEquipmentId)
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
        Bubble.SetActive(false);
        Avatar.SetActive(false);
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
