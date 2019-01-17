using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class PlayerAvata : MonoBehaviour {

    public SpriteRenderer Equip_hip;

    public SpriteRenderer Equip_body;

    public SpriteRenderer Equip_cape;

    public SpriteRenderer Equip_helmet0;

    public SpriteRenderer Equip_helmet1;

    public SpriteRenderer Equip_chest;

    public SpriteRenderer Equip_shoulder0;

    public SpriteRenderer Equip_shoulder00;

    public SpriteRenderer Equip_shoulder1;

    public SpriteRenderer Equip_shoulder11;

    public SpriteRenderer Equip_arm0;

    public SpriteRenderer Equip_forearm0;

    public SpriteRenderer Equip_shield;

    public SpriteRenderer Arrow;

    public SpriteRenderer FX_arrow;

    public SpriteRenderer Equip_arm1;

    public SpriteRenderer Equip_forearm1;

    public SpriteRenderer Weapon_0;

    public SpriteRenderer FX_sword;

    public SpriteRenderer FX_staff;

    public SpriteRenderer Bowstring_1;

    public SpriteRenderer Bowstring_2;

    public SpriteRenderer Equip_thigh0;

    public SpriteRenderer Equip_leg0;

    public SpriteRenderer Equip_thigh1;

    public SpriteRenderer Equip_leg1;

    public SpriteRenderer Weapon_1;

    public SpriteRenderer Weapon_2;

    public SpriteRenderer Equip_cloak;

    public SpriteRenderer Equip_wing0;

    public SpriteRenderer Equip_wing1;

    public SpriteRenderer FXW_0;

    public SpriteRenderer FXA;

	public GameObject Weakness;

	/// <summary>
	/// 初始化状态
	/// </summary>
	public bool InitStatus;

	// Use this for initialization
	void Start () 
    {

	}

    public void ClearAvata()
    {
        if (Equip_hip)
        {
           // Equip_hip.enabled=false;
            Equip_hip.sprite = null;
        }
        if (Equip_body)
        {
           // Equip_body.enabled = false;
            Equip_body.sprite = null;
        }
        if(Equip_cape)
        {
            //Equip_cape.enabled = false;
            Equip_cape.sprite = null;
        }
        if (Equip_helmet0)
        {
           // Equip_helmet0.enabled = false;
            Equip_helmet0.sprite = null;
        }
        if (Equip_helmet1)
        {
            //Equip_helmet1.enabled = false;
            Equip_helmet1.sprite = null;
        }
        if (Equip_chest)
        {
           // Equip_chest.enabled = false;
            Equip_chest.sprite = null;
        }
        if (Equip_shoulder0)
        {
            //Equip_shoulder0.enabled = false;
            Equip_shoulder0.sprite = null;
        }
        if (Equip_shoulder00)
        {
            //Equip_shoulder00.enabled = false;
            Equip_shoulder00.sprite = null;
        }
        if (Equip_shoulder1)
        {
            //Equip_shoulder1.enabled = false;
            Equip_shoulder1.sprite = null;
        }
        if (Equip_shoulder11)
        {
            //Equip_shoulder11.enabled = false;
            Equip_shoulder11.sprite = null;
        }
        if (Equip_arm0)
        {
           // Equip_arm0.enabled = false;
            Equip_arm0.sprite = null;
        }
        if (Equip_forearm0)
        {
           // Equip_forearm0.enabled = false;
            Equip_forearm0.sprite = null;
        }
        if (Equip_shield)
        {
            //Equip_shield.enabled = false;
            Equip_shield.sprite = null;
        }
        if (Arrow)
        {
            //Arrow.enabled = false;
            Arrow.sprite = null;
        }
        if (FX_arrow)
        {
            //FX_arrow.enabled = false;
            FX_arrow.sprite = null;
        }
        if (Equip_arm1)
        {
            //Equip_arm1.enabled = false;
            Equip_arm1.sprite = null;
        }
        if (Equip_forearm1)
        {
           // Equip_forearm1.enabled = false;
            Equip_forearm1.sprite = null;
        }
        if (Weapon_0)
        {
            //Weapon_0.enabled = false;
            Weapon_0.sprite = null;
        }
        if (FX_sword)
        {
            //FX_sword.enabled = false;
            FX_sword.sprite = null;
        }
        if (FX_staff)
        {
            //FX_staff.enabled = false;
            FX_staff.sprite = null;
        }
        if (Bowstring_1)
        {
           // Bowstring_1.enabled = false;
            Bowstring_1.sprite = null;
        }
        if (Bowstring_2)
        {
            //Bowstring_2.enabled = false;
            Bowstring_2.sprite = null;
        }
        if (Equip_thigh0)
        {
            //Equip_thigh0.enabled = false;
            Equip_thigh0.sprite = null;
        }
        if (Equip_leg0)
        {
            //Equip_leg0.enabled = false;
            Equip_leg0.sprite = null;
        }
        if (Equip_thigh1)
        {
            //Equip_thigh1.enabled = false;
            Equip_thigh1.sprite = null;
        }
        if (Equip_leg1)
        {
            //Equip_leg1.enabled = false;
            Equip_leg1.sprite = null;
        }
        if (Weapon_1)
        {
            //Weapon_1.enabled = false;
            Weapon_1.sprite = null;
        }
        if (Weapon_2)
        {
            //Weapon_2.enabled = false;
            Weapon_2.sprite = null;
        }
        if (Equip_cloak)
        {
            //Equip_cloak.enabled = false;
            Equip_cloak.sprite = null;
        }
        if (Equip_wing0)
        {
            //Equip_wing0.enabled = false;
            Equip_wing0.sprite = null;
        }
        if (Equip_wing1)
        {
            //Equip_wing1.enabled = false;
            Equip_wing1.sprite = null;
        }
        if (FXW_0)
        {
            //FXW_0.enabled = false;
            FXW_0.sprite = null;
        }
        if (FXA)
        {
            //FXA.enabled = false;
            FXA.sprite = null;
        }
    }

    List<Sprite> curWeaponSprites = new List<Sprite>();
    public void AddAvataWare(string wearId,DungeonEnum.FaceDirection direction)
    {
        if (direction == DungeonEnum.FaceDirection.Right || direction == DungeonEnum.FaceDirection.RightDown || direction == DungeonEnum.FaceDirection.Left)
        {
            direction = DungeonEnum.FaceDirection.LeftDown;
        }
        if (direction == DungeonEnum.FaceDirection.LeftUp)
        {
            direction = DungeonEnum.FaceDirection.UpRight;
        }
        string path = wearId;
        if (direction != DungeonEnum.FaceDirection.None)
        {
            path = path + "_" + ((int)direction).ToString();
        }
        Sprite[] wareSprites = Resources.LoadAll<Sprite>("Sprites/_Props/" + path);
        List<Sprite> fxWeaponSprites = new List<Sprite>();
        foreach (Sprite s in wareSprites)
        {
            SpriteRenderer sprite = GetRenderByName(s.name);
            
            if (direction == DungeonEnum.FaceDirection.None)
            {
                string[] nameSp = s.name.Split('_');
                if (nameSp.Length >= 2 && nameSp[0]=="FXA")
                {
                    fxWeaponSprites.Add(s);
                }
            }
            if (sprite)
            {
                sprite.enabled = true;
                sprite.sprite = s;
            }
        }
        if (direction == DungeonEnum.FaceDirection.None)
        {
            curWeaponSprites = fxWeaponSprites;
        }
    }

    void OnEnable()
    {
        WeaponEffectShow();
    }

    int curIndex;
    public void WeaponEffectShow()
    {
        StopCoroutine("ChangeEffectFrame");
        if (curWeaponSprites.Count > 0 && FXA != null)
        {
            curIndex = 0;
            StartCoroutine(ChangeEffectFrame());
        }
    }

    IEnumerator ChangeEffectFrame()
    {
        yield return new WaitForSeconds(0.1f);
        FXA.sprite = curWeaponSprites[curIndex];
        curIndex++;
        if (curIndex >= curWeaponSprites.Count)
        {
            curIndex = 0;
        }
        StartCoroutine(ChangeEffectFrame());
    }

    /// <summary>
    /// 穿戴装备剪影
    /// </summary>
    public void AddAvatarWareOutline(string wearId, DungeonEnum.FaceDirection direction, bool Tag)
    {
        if (direction == DungeonEnum.FaceDirection.Right || direction == DungeonEnum.FaceDirection.RightDown || direction == DungeonEnum.FaceDirection.Left)
        {
            direction = DungeonEnum.FaceDirection.LeftDown;
        }
        if (direction == DungeonEnum.FaceDirection.LeftUp)
        {
            direction = DungeonEnum.FaceDirection.UpRight;
        }
        string path = wearId;
        if (direction != DungeonEnum.FaceDirection.None)
        {
            path = path + "_" + ((int)direction).ToString();
        }
        Sprite[] wareSprites = Resources.LoadAll<Sprite>("Sprites/_Props/" + path);
        foreach (Sprite s in wareSprites)
        {
            SpriteRenderer sprite = GetRenderByName(s.name);
            
            if (sprite)
            {
                sprite.enabled = true;
                if (Tag)
                {
                    string regexModel = "^FX.*$";
                    Regex r = new Regex(regexModel);
                    Match m = r.Match(s.name);
                    sprite.sprite = s;
                    if (m.Success)
                    {
                        sprite.color = new Color(0, 0, 0);
                    }
                    else
                    {
                        sprite.color = new Color(0, 0, 0, 0.7f);
                    }
                }
                else
                {
                    string regexModel = "^FX.*$";
                    Regex r = new Regex(regexModel);
                    Match m = r.Match(s.name);
                    sprite.sprite = s;
                    if (m.Success)
                    {
                        sprite.color = new Color(255f, 255f, 255f);
                    }
                    else
                    {
                        sprite.color = new Color(255f, 255f, 255f, 1f);
                    }
                }
            }
        }
    }

    SpriteRenderer GetRenderByName(string name)
    {
        if (name == "Equip_hip")
        {
            return Equip_hip;
        }
        else if (name == "Equip_body")
        {
            return Equip_body;
        }
        else if(name == "Equip_cape")
        {
            return Equip_cape;
        }
        else if (name == "Equip_helmet0")
        {
            return Equip_helmet0;
        }
        else if (name == "Equip_helmet1")
        {
            return Equip_helmet1;
        }
        else if (name == "Equip_chest")
        {
            return Equip_chest;
        }
        else if (name == "Equip_shoulder0")
        {
            return Equip_shoulder0;
        }
        else if (name == "Equip_shoulder00")
        {
            return Equip_shoulder00;
        }
        else if (name == "Equip_shoulder1")
        {
            return Equip_shoulder1;
        }
        else if (name == "Equip_shoulder11")
        {
            return Equip_shoulder11;
        }
        else if (name == "Equip_arm0")
        {
            return Equip_arm0;
        }
        else if (name == "Equip_forearm0")
        {
            return Equip_forearm0;
        }
        else if (name == "Equip_shield")
        {
            return Equip_shield;
        }
        else if (name == "Arrow")
        {
            return Arrow;
        }
        else if (name == "FX_arrow")
        {
            return FX_arrow;
        }
        else if (name == "Equip_arm1")
        {
            return Equip_arm1;
        }
        else if (name == "Equip_forearm1")
        {
            return Equip_forearm1;
        }
        else if (name == "Weapon_0")
        {
            return Weapon_0;
        }
        else if (name == "FX_sword")
        {
            return FX_sword;
        }
        else if (name == "FX_staff")
        {
            return FX_staff;
        }
        else if (name == "Bowstring_1")
        {
            return Bowstring_1;
        }
        else if (name == "Bowstring_2")
        {
            return Bowstring_2;
        }
        else if (name == "Equip_thigh0")
        {
            return Equip_thigh0;
        }
        else if (name == "Equip_leg0")
        {
            return Equip_leg0;
        }
        else if (name == "Equip_thigh1")
        {
            return Equip_thigh1;
        }
        else if (name == "Equip_leg1")
        {
            return Equip_leg1;
        }
        else if (name == "Weapon_1")
        {
            return Weapon_1;
        }
        else if (name == "Weapon_2")
        {
            return Weapon_2;
        }
        else if (name == "Equip_cloak")
        {
            return Equip_cloak;
        }
        else if (name == "Equip_wing0")
        {
            return Equip_wing0;
        }
        else if (name == "Equip_wing1")
        {
            return Equip_wing1;
        }
        else if (name == "FXW_0")
        {
            return FXW_0;
        }
        else if (name == "FXA")
        {
            return FXA;
        }
        return null;
    }
}
