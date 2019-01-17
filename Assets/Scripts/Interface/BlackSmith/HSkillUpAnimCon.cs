using UnityEngine;
using System.Collections;

public class HSkillUpAnimCon : MonoBehaviour, EquipmentiIemInterface
{
    #region Animation
    public GameObject Animation_1;
    public GameObject Animation_2;
    public GameObject Animation_3;
    public GameObject Animation_4;
    public GameObject Animation_5;

    Vector3 BasePosition = new Vector3(0f, -22f, 0f);
    Vector3 Position_1 = new Vector3(-230f, -60f, 0f);
    Vector3 Position_2 = new Vector3(-145f, 75f, 0f);
    Vector3 Position_3 = new Vector3(145f, 75f, 0f);
    Vector3 Position_4 = new Vector3(230f, -60f, 0f);
    Vector3 Position_5 = new Vector3(0f, -150f, 0f);

    public SpriteRenderer HBaseTexture;
    public SpriteRenderer HMTexture_1;
    public SpriteRenderer HMTexture_2;
    public SpriteRenderer HMTexture_3;
    public SpriteRenderer HMTexture_4;
    public SpriteRenderer HMTexture_5;

    public GameObject BaseBoard;

    bool isOver = false;

    public void SetAnimation(string Id_base, string Id_m1, string Id_m2, string Id_m3, string Id_m4, string Id_m5)
    {
        HBaseTexture.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + Id_base);
        if (Id_m1 != null)
        {
            Animation_1.SetActive(true);

            if (ConfigManager.HardWareConfig.GetHardWareBySkin(Id_m1) != null)
                HMTexture_1.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + Id_m1);  //装备
            if (HMTexture_1.sprite == null)
            {
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m1);  //素材
                HMTexture_1.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
                HMTexture_1.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                HMTexture_1.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            }
        }
        else
        {
            Animation_1.SetActive(false);
        }
        if (Id_m2 != null)
        {
            Animation_2.SetActive(true);

            if (ConfigManager.HardWareConfig.GetHardWareBySkin(Id_m2) != null)
                HMTexture_2.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + Id_m2);
            if (HMTexture_2.sprite == null)
            {
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m2);
                HMTexture_2.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
                HMTexture_2.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                HMTexture_2.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            }
        }
        else
        {
            Animation_2.SetActive(false);
        }
        if (Id_m3 != null)
        {
            Animation_3.SetActive(true);

            if (ConfigManager.HardWareConfig.GetHardWareBySkin(Id_m3) != null)
                HMTexture_3.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + Id_m3);
            if (HMTexture_3.sprite == null)
            {
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m3);
                HMTexture_3.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
                HMTexture_3.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                HMTexture_3.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            }
        }
        else
        {
            Animation_3.SetActive(false);
        }
        if (Id_m4 != null)
        {
            Animation_4.SetActive(true);

            if (ConfigManager.HardWareConfig.GetHardWareBySkin(Id_m4) != null)
                HMTexture_4.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + Id_m4);
            if (HMTexture_4.sprite == null)
            {
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m4);
                HMTexture_4.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
                HMTexture_4.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                HMTexture_4.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            }
        }
        else
        {
            Animation_4.SetActive(false);
        }
        if (Id_m5 != null)
        {
            Animation_5.SetActive(true);

            if (ConfigManager.HardWareConfig.GetHardWareBySkin(Id_m5) != null)
                HMTexture_5.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + Id_m5);
            if (HMTexture_5.sprite == null)
            {
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m5);
                HMTexture_5.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
                HMTexture_5.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                HMTexture_5.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
            }
        }
        else
        {
            Animation_5.SetActive(false);
        }
        Invoke("PlayAudio", 3f);
        Invoke("SetOver", 4f);
        StartCoroutine(MaterialDelayDisappear());
    }

    void PlayAudio()
    {
        AudioClip Audio = Resources.Load<AudioClip>("Audio/UIAudio/Synthesis");
        audio.clip = Audio;
        audio.Play();
    }

    void SetOver()
    {
        isOver = true;
    }

    IEnumerator MaterialDelayDisappear()
    {
        yield return new WaitForSeconds(1.5f);
        HMTexture_1.sprite = null;
        HMTexture_2.sprite = null;
        HMTexture_3.sprite = null;
        HMTexture_4.sprite = null;
        HMTexture_5.sprite = null;
    }

    void OnEnable()
    {
        isOver = false;
        SetPowUpInfo();
    }

    void OnDisable()
    {
        HBaseTexture.sprite = null;
    }

    public GameObject CurView;
    public GameObject TargetView;
    public HardwareSkillEvolution HEvoView;

    public void SetPowUpInfo()
    {
        SetSkillInfo();
        UIEventListener.Get(BaseBoard).onClick = (g) =>
        {
            if (isOver)
            {
                gameObject.SetActive(false);
                HEvoView.SetSkillEvolution(Hbase.UserWareId);
                //if (CurView != null && TargetView != null)
                //{
                //    CurView.SetActive(false);
                //    TargetView.SetActive(true);
                //}
            }
        };
    }
    #endregion

    #region NGUI
    public UILabel Name;
    public UILabel SkillName;
    public UILabel PreSkillLv;
    public UILabel LaterSkillLv;
    public equipmentItemInterface Icon;

    public UserWare Hbase;

    void SetSkillInfo()
    {
        //SkillData skill = ConfigManager.SkillConfig.GetSkillByIdLv(Hbase.CurHardWareData.SkillAffix1, Hbase.CurSkillLv);
        //Name.text = Hbase.CurHardWareData.Name;
        //SkillName.text = skill.Name;
        //PreSkillLv.text = (skill.SkillLv - 1).ToString();
        //LaterSkillLv.text = skill.SkillLv.ToString();
        //Icon.SetItem(Hbase.Level, (int)Hbase.CurAtk, Hbase.CurHardWareData.Element, Hbase.CurHardWareData.SkinId, false, Hbase.CurHardWareData.Rank, Hbase.UserWareId);
        //Icon.equipmentItemInter = this;
    }

    public void _OnClickEquipmentItem(int UserEquipmentID)
    {

    }
    public void _OnLongPressEquipmentItem(int UserEquipmentID)
    {

    }
    #endregion
}
