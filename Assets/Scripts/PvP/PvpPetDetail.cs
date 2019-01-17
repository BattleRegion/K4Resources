using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpPetDetail : MonoBehaviour 
{

	/// <summary>
	/// 详情时关闭unit层摄像机
	/// </summary>
	public GameObject UnitCamera;
	
	public UIGrid stars;
	
	public GameObject StarPrefab;
	public GameObject StarOutline;
	
	public UISprite elementType;
	public UILabel monsterName;
	//public UILabel type;
	public UILabel level;
	public AlphaMaskBar expBar;
	public UILabel hp;
	public UILabel atk;
	public UILabel count;
	public UILabel cost;
	
	GameObject ClearTarget = null;
	
	public GameObject Skill_1;
	
	public UILabel Skill_Name;
	public UILabel Skill_Description;
	public UISprite Skill_Map;
	public UILabel Skill_Chain;
	public UILabel Skill_Cd;
	
	public GameObject Skill_2;
	
	public UILabel Skill_Name_2;
	public UILabel Skill_Description_2;
	public UISprite Skill_Map_2;
	public UILabel Skill_Chain_2;
	public UILabel Skill_Cd_2;
	
	public ShowNewObject NewObjectController;
	public GameObject ShowNewButton;
	
	UserPet CurPet;
	
	List<GameObject> TempStars = new List<GameObject>();
	List<Transform> StarOutlineTransforms = new List<Transform>();
	
	private PvpGameControl gameControl;
	
	public void SetDetail(UserPet u, PvpUserInfo userInfo, PvpGameControl gameControl)
	{
		this.gameControl = gameControl;

		this.gameObject.SetActive (true);
		
		CurPet = u;
		
		GameObject g = ClearTarget;
		if (g != null)
		{
			DestroyImmediate(g);
		}
		
		GameObject PetAnime = Resources.Load<GameObject>("PreFabs/Characters/" + u.CurPetData.SkinId + "60");
		if (PetAnime == null)
			PetAnime = Resources.Load<GameObject>("PreFabs/Charaters/S160");
		GameObject Anime = NGUITools.AddChild(gameObject, PetAnime);
		Anime.transform.localPosition = new Vector3(0f, 20f, 0f);
		Anime.transform.localScale = new Vector3(100f, 100f, 1f);
		PetUIController.SetLayer(Anime.transform, LayerHelper.UI);
		ClearTarget = Anime;
		
		int starCount = u.CurPetData.MaxRank;
		while (starCount > 0)
		{
			GameObject go = NGUITools.AddChild(stars.gameObject, StarOutline);
			StarOutlineTransforms.Add(go.transform);
			starCount--;
		}
		stars.Reposition();
		StartCoroutine(AddStars(0.2f, u.CurPetData.Rank));
		
		
		switch (u.CurPetData.PetPro)
		{
		case DungeonEnum.ElementAttributes.Earth: elementType.spriteName = "element_type_earth"; break;
		case DungeonEnum.ElementAttributes.Fire: elementType.spriteName = "element_type_fire"; break;
		case DungeonEnum.ElementAttributes.Water: elementType.spriteName = "element_type_water"; break;
		case DungeonEnum.ElementAttributes.Wind: elementType.spriteName = "element_type_wind"; break;
		default: elementType.spriteName = ""; break;
		}
		monsterName.text = u.CurPetData.Name;
		level.text = "Lv.[4FFE27]" + u.Level + "[FFFFFF]/" + u.CurPetData.MaxLevel;
		if(u.Level == u.CurPetData.MaxLevel)
		{
			expBar.value = 1f;
		}
		else
		{
			expBar.value = (float)u.CurrentExp / (float)u.CurLvlExp;
		}
		hp.text = u.CurHp.ToString();
		atk.text = u.CurAtk.ToString();
		
		int petcount = 0;
		foreach (UserPet up in userInfo.UserPets)
		{
			if (u.CurPetData.Id == up.CurPetData.Id)
			{
				petcount++;
			}
		}
		count.text = petcount.ToString();
		
		//cost.text = u.CurPetData.PCost.ToString();
		
		if (u.CurPetData.PetSkillData != null)
		{
			Skill_1.SetActive(true);
			SkillData skill = u.CurPetData.PetSkillData;
			Skill_Name.text = skill.Name;
			Skill_Description.text = skill.Description;
			Skill_Map.spriteName = skill.SkillIcon;
			Skill_Chain.text = (skill.SkillPower / 10).ToString();
			if (skill.SkillCd > 0) Skill_Cd.text = skill.SkillCd.ToString();
			else Skill_Cd.text = "--";
		}
		else Skill_1.SetActive(false);
		
		if (u.CurPetData.PetSkillData2 != null)
		{
			Skill_2.SetActive(true);
			SkillData skill = u.CurPetData.PetSkillData2;
			Skill_Name_2.text = skill.Name;
			Skill_Description_2.text = skill.Description;
			Skill_Map_2.spriteName = skill.SkillIcon;
			Skill_Chain_2.text = (skill.SkillPower / 10).ToString();
			if (skill.SkillCd > 0) Skill_Cd_2.text = skill.SkillCd.ToString();
			else Skill_Cd_2.text = "--";
		}
		else Skill_2.SetActive(false);
	}
	
	bool newStart = false;
	IEnumerator AddStars(float time, int rank)
	{
		yield return new WaitForSeconds(time);
		for (int i = 0; i < rank; i++)
		{
			StartCoroutine(AddStarAnimation(i));
		}
		Invoke("SetNewStart", rank * 0.2f);
	}
	
	IEnumerator AddStarAnimation(int index)
	{
		yield return new WaitForSeconds(index * 0.2f);
		GameObject g = NGUITools.AddChild(StarOutlineTransforms[index].gameObject, StarPrefab);
		TempStars.Add(g);
		g.transform.localPosition = new Vector3(-0.5f, 0.5f, 0f);
		g.transform.localScale *= 3f;
		AnimationHelper.AnimationScaleTo(Vector3.one, g, iTween.EaseType.linear, null, null, 0.2f);
	}
	
	void SetNewStart()
	{
		newStart = true;
	}
	
	
	void OnEnable()
	{
		if(this.gameControl != null && this.gameControl.PvpPlayerInfo != null)
		{
			this.gameControl.RoundAction.gameObject.SetActive(false);
			this.gameControl.PvpPlayerInfo.SurrenderBtn.gameObject.SetActive(false);
		}

		UIEventListener.Get(ShowNewButton).onClick = (g) =>
		{
			NewObjectController.ShowNew(1, CurPet.CurPetData.Id);
		};
	}
	
	void OnDisable()
	{
		if(this.gameControl != null && this.gameControl.PvpPlayerInfo != null)
		{
			this.gameControl.RoundAction.gameObject.SetActive(true);
			this.gameControl.PvpPlayerInfo.SurrenderBtn.gameObject.SetActive(true);
		}

		foreach (Transform t in stars.transform)
		{
			Destroy(t.gameObject);
		}
		foreach (GameObject g in TempStars)
		{
			Destroy(g);
		}
		TempStars.Clear();
		StarOutlineTransforms.Clear();
	}
}
