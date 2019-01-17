using UnityEngine;
using System.Collections;

/// <summary>
/// 技能详细信息面板
/// 发动 和 取消
/// </summary>
public class Confirm : MonoBehaviour {

    public GameObject CancleObject;

    public GameObject OKObject;

    public UILabel Label;

	public int PetType;

	// Use this for initialization
	void Start () {
        UIEventListener.Get(CancleObject).onClick = (go) =>
        {
			if(PetType == 0)
			{
	            gameObject.SetActive(false);
	            PveGameControl ds = GameObject.Find("UI Root").GetComponent<PveGameControl>();
	            foreach (PveEliminate eb in ds.AllEliminates)
	            {
	                eb.ReturnToCommon();
	            }
	            ds.EnemyUnitEliminateEnable(false);
	            ds.UserInputLock = false;
	            ds.CurCharacter.ShowArrow(true);
			}else{
				gameObject.SetActive(false);
				PvpGameControl ds = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
				// 重置界面状态
				ds.ResetGameControlStatus();
			}
        };

        UIEventListener.Get(OKObject).onClick = (go) =>
        {
			if(PetType == 0)
			{
	            PveGameControl ds = GameObject.Find("UI Root").GetComponent<PveGameControl>();
	            gameObject.SetActive(false);
	            //ds.ShowSkillBack(true);

                // 如果按钮可以点击，并且是主动技能
				if (this.powerStatus && this.cdStatus && initiativeStatus)
                {

                    ds.UseSkillWithPetFly();
                }
                else
                {
                    ds.UseSkillEnd();
                }
			}else
			{
				PvpGameControl ds = GameObject.Find("UI Root").GetComponent<PvpGameControl>();
				gameObject.SetActive(false);

				// 查找技能配置文件
				BaseSkillItem baseSkillItem = SkillManager.GetSkillItem (ds.CurReadySkillUnit.MainSkill);
				// 如果数据不正确，或者不是法术技能，返回
				if(baseSkillItem == null || !baseSkillItem.IsMagic)
				{
					// 重置界面状态
					ds.ResetGameControlStatus();
					return;
				}

				ds.ShowSkillBack(true);

				// 如果按钮可以点击，并且是主动技能
				if(this.powerStatus && this.cdStatus && initiativeStatus)
				{
					ds.UseSkillWithPetFly();
                	//ds.UseSkill(ds.CurReadySkillUnit.MainSkill);
				}else{
					ds.UseSkillEnd();
				}
			}
        };
	}

	private bool powerStatus;
	private bool cdStatus;
	private bool initiativeStatus;
	private bool okPve;

	public void ChangeData(bool powerStatus, bool cdStatus, bool initiativeStatus, bool okPve = true)
	{
		UISprite uiSprite = this.OKObject.GetComponent<UISprite>();

		this.powerStatus = powerStatus;
		this.cdStatus = cdStatus;
		this.initiativeStatus = initiativeStatus;

		// 如果是被动技能
		if(!initiativeStatus)
		{
			this.OKObject.SetActive(false);
		}else
		{ 
			// 如果是主动技能
			if(okPve)
			{
				this.OKObject.SetActive(true);

				if(!cdStatus)
				{
					uiSprite.spriteName = "btn_cd";
				}
				if(!powerStatus)
				{
					uiSprite.spriteName = "btn_power";
				}

				if(cdStatus && powerStatus)
				{
					uiSprite.spriteName = "btn_initiative";
				}
			}else
			{
				this.OKObject.SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void PvpSet(SkillData s1=null,SkillData s2=null) {
        OKObject.SetActive(false);
        if (s1 != null)
        {
            if (s1.Type == 1)
            {
                //主动即时
                OKObject.SetActive(true);

            }
            else if (s1.Type == 2)
            {
                //主动延迟
                OKObject.SetActive(true);

            }
            else if (s1.Type == 3)
            {
                //被动
               

            }
        }
        if (s2 != null)
        {
            if (s1.Type == 3)
            {
                //被动

            }
        }
    
    }

    public void SetDes(string des)
    {
        Label.text = des;
    }
    public void OkButtonVisible(bool b)
    {
        OKObject.SetActive(b);
    }
}
