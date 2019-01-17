using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillEffectItem : MonoBehaviour 
{
	private int effectIndex;
	private List<int> effectTypeList;
	private List<string> effectPrefabList;
	private Action<SkillEffectParam> endCallback;
	private Action<SkillEffectParam> attackCallback;

	/// <summary>
	/// 技能数据
	/// </summary>
	private BaseSkillItem skillItem;

	/// <summary>
	/// 已方
	/// </summary>
	private PvpFightUnit sourceItem;

	/// <summary>
	/// 敌人
	/// </summary>
	private PvpFightUnit targetItem;

	/// <summary>
	/// 敌人列表
	/// </summary>
	private List<PvpFightUnit> targetItemList;

	/// <summary>
	/// 粒子预设
	/// </summary>
	private GameObject prefabItem;

	/// <summary>
	/// 效果表现目标
	/// </summary>
	private PvpFightUnit prefabUnit;

	/// <summary>
	/// 运行状态
	/// </summary>
	public bool runStatus;

	/// <summary>
	/// 攻击回调状态
	/// </summary>
	private bool attackCallbackStatus;

	private SkillEffectParam effectParam;

	/// <summary>
	/// 效果表现
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="targetItemList">Target item list.</param>
	/// <param name="callback">Callback.</param>
	public void Run(SkillEffectParam effectParam, BaseSkillItem skillItem, PvpFightUnit sourceItem, PvpFightUnit targetItem, List<PvpFightUnit> targetItemList = null, Action<SkillEffectParam> endCallback = null, Action<SkillEffectParam> attackCallback = null)
	{
		//Debug.Log ("调用技能表现！！！！！！！！");

		// 如果是范围型的攻击，添加屏幕震动
		if(skillItem.skillData.skillTarget == SkillTargetTypeEnum.Range)
		{
			// 屏幕震动
			CameraControl.ShakeCamera();
		}

		this.effectParam = effectParam;
		this.runStatus = true;
		this.attackCallbackStatus = false;

		this.skillItem = skillItem;
		this.sourceItem = sourceItem;
		this.targetItem = targetItem;
		this.targetItemList = targetItemList;
		this.endCallback = endCallback;
		this.attackCallback = attackCallback;

		this.effectIndex = 0;

		this.effectTypeList = new List<int> ();
		this.effectPrefabList = new List<string> ();

		// 如果第一个技能符合要求
		if(this.EffectTypeCheck(skillItem.configData.FXType1))
		{
			if(!string.IsNullOrEmpty(skillItem.configData.FXPrefab1))
			{
				this.effectTypeList.Add(skillItem.configData.FXType1);
				this.effectPrefabList.Add(skillItem.configData.FXPrefab1);
			}
		}
		// 如果第二个技能符合要求
		if(this.EffectTypeCheck(skillItem.configData.FXType2))
		{
			if(!string.IsNullOrEmpty(skillItem.configData.FXPrefab2))
			{
				this.effectTypeList.Add(skillItem.configData.FXType2);
				this.effectPrefabList.Add(skillItem.configData.FXPrefab2);
			}
		}
		// 如果第三个技能符合要求
		if(this.EffectTypeCheck(skillItem.configData.FXType3))
		{
			if(!string.IsNullOrEmpty(skillItem.configData.FXPrefab3))
			{
				this.effectTypeList.Add(skillItem.configData.FXType3);
				this.effectPrefabList.Add(skillItem.configData.FXPrefab3);
			}
		}

		this.EffectCallback ();
	}

	/// <summary>
	/// 召唤的效果表现
	/// </summary>
	/// <param name="effectParam">Effect parameter.</param>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="position">Position.</param>
	/// <param name="endCallback">End callback.</param>
	public void Run(SkillEffectParam effectParam, PvpFightUnit pvpFightUnit, BaseSkillItem skillItem, Vector3 position, Action<SkillEffectParam> endCallback)
	{
		this.effectParam = effectParam;
		this.runStatus = true;
		this.endCallback = endCallback;

		string prefabName = "";

		if(skillItem.configData.FXType1 == SkillEffectTypeEnum.SUMMON) prefabName = skillItem.configData.FXPrefab1;
		if(skillItem.configData.FXType2 == SkillEffectTypeEnum.SUMMON) prefabName = skillItem.configData.FXPrefab2;
		if(skillItem.configData.FXType3 == SkillEffectTypeEnum.SUMMON) prefabName = skillItem.configData.FXPrefab3;

		if(string.IsNullOrEmpty(prefabName))
		{
			this.runStatus = false;
			if(this.endCallback != null) this.endCallback(this.effectParam);
			return;
		}

		Debug.Log("SkillEffectItem 预设名称：" + prefabName);

		PvpGameObjectManager.Create (DungeonSpritePathManager.SkillBumpFX (prefabName), (GameObject objectItem)=>
		{
			Debug.Log("PvpGameObjectManager 生成预设回调");
			this.prefabItem = objectItem;
			//this.prefabItem = SkillEffectManager.GetPrefabItem(prefabName);
			
			// 设置父对象
			this.prefabItem.transform.parent = pvpFightUnit.GameControl.FightUnitEffect.transform;
			// 设置本地位置
			this.prefabItem.transform.localPosition = new Vector3 (position.x, position.y, 0f);
			
			this.StartCoroutine (this.StarEffectItemSummon (SkillEffectManager.GetPrefabDestoryTimeItem(this.prefabItem)));
		});
	}

	IEnumerator StarEffectItemSummon(float destoryTime)
	{
		yield return new WaitForSeconds(destoryTime);

		Debug.Log ("SkillEffectItem 的 StarEffectItemSummon");

		this.runStatus = false;
		if(this.endCallback != null) this.endCallback(this.effectParam);
	}

	/// <summary>
	/// 效果类别检查
	/// </summary>
	/// <returns><c>true</c>, if type check was effected, <c>false</c> otherwise.</returns>
	/// <param name="fxType">Fx type.</param>
	private bool EffectTypeCheck(int fxType)
	{
		if(fxType == SkillEffectTypeEnum.NONE || fxType == SkillEffectTypeEnum.FIXED || fxType == SkillEffectTypeEnum.SUMMON) return false;
		return true;
	}

	/// <summary>
	/// 协程处理
	/// </summary>
	/// <returns>The effect item.</returns>
	/// <param name="callback">Callback.</param>
	IEnumerator StarEffectItem(Action callback)
	{
		yield return null;

		int effectType = this.effectTypeList [this.effectIndex];

		if(effectType == SkillEffectTypeEnum.ENEMY_BUFF || effectType == SkillEffectTypeEnum.SELF_BUFF)
		{
			// 确保 attackCallback 只调用了一次
			if(!this.attackCallbackStatus)
			{
				this.attackCallbackStatus = true;
				if(this.attackCallback != null) this.attackCallback(this.effectParam);
			}
		}
		// 如果不是逐格
		if(effectType != SkillEffectTypeEnum.ITEM)
		{
			PvpGameObjectManager.Create (DungeonSpritePathManager.SkillBumpFX (this.effectPrefabList[this.effectIndex]), (GameObject objectItem)=>
			{
				// 粒子预设
				this.prefabItem = objectItem;
				//this.prefabItem = SkillEffectManager.GetPrefabItem(this.effectPrefabList[this.effectIndex]);
				if(this.prefabItem != null)
				{
					// 设置层
					this.prefabItem.layer = LayerHelper.UnitFX;

					// 销毁时间
					float destoryTime = SkillEffectManager.GetPrefabDestoryTimeItem(this.prefabItem);
					switch(effectType)
					{
						case SkillEffectTypeEnum.ENEMY_SINGLE : // 敌人单体
								this.prefabUnit = this.targetItem;
							; break;
						case SkillEffectTypeEnum.SELF_SINGLE : // 已方单体
								this.prefabUnit = this.sourceItem;
							; break;
						case SkillEffectTypeEnum.ENEMY_BUFF : // 敌人效果
								this.prefabUnit = this.targetItem;
							; break;
						case SkillEffectTypeEnum.SELF_BUFF : // 已方效果
								this.prefabUnit = this.sourceItem;
							; break;
					}
					// 如果不是飞向目标
					if(effectType != SkillEffectTypeEnum.FLY_TO_ENEMY)
					{
						this.prefabItem.transform.parent = this.prefabUnit.transform;
						this.prefabItem.transform.localPosition = new Vector3(0f, 0.04f, 0f);
						// 销毁预设
						this.StartCoroutine(this.PrefabDesotry(destoryTime, callback));
					}
					else
					{ // 如果是飞向目标，另外处理
						this.SourceFlyToEnemy();
					}
				}else
				{
					// 销毁预设
					this.StartCoroutine(this.PrefabDesotry(0f, callback));
					/*yield return null;
					
					this.effectIndex ++;
					if(callback != null) callback();
					*/
				}
			});
		}else
		{
			this.sourceItem.GameControl.PowerSkillRenderBumpAttack(effectType, this.effectPrefabList[this.effectIndex], this.sourceItem as PvpCharacter, false);

			this.effectIndex ++;
			if(callback != null) callback();
		}
	}

	private IEnumerator PrefabDesotry(float destoryTime, Action callback)
	{
		yield return new WaitForSeconds(destoryTime);
		
		this.effectIndex ++;
		if(callback != null) callback();
	}

	/// <summary>
	/// 飞向目标
	/// </summary>
	private void SourceFlyToEnemy()
	{
		if(this.prefabItem != null)
		{
			// 设置位置 
			this.prefabItem.transform.parent = this.sourceItem.GameControl.FightUnitEffect.transform;
			this.prefabItem.transform.localPosition = this.sourceItem.transform.localPosition;

			AnimationHelper.AnimationMoveTo(this.targetItem.transform.localPosition, this.prefabItem.gameObject, iTween.EaseType.linear, this.gameObject, "SourceFlyToEnemyCallback", 1f);
		}else{
			this.effectIndex ++;
			this.EffectCallback();
		}
	}

	/// <summary>
	/// 飞行结束回调函数
	/// </summary>
	private void SourceFlyToEnemyCallback()
	{
		GameObject.Destroy (this.prefabItem.gameObject);
		this.effectIndex ++;

		this.EffectCallback ();
	}

	/// <summary>
	/// 递归调用效果
	/// </summary>
	private void EffectCallback()
	{
		if(this.effectIndex >= this.effectTypeList.Count)
		{
			this.runStatus = false;

			//Debug.Log("技能效果表现结果状态为：：：" + this.runStatus);

			if(!this.attackCallbackStatus)
			{
				this.attackCallbackStatus = true;
				if(this.attackCallback != null) this.attackCallback(this.effectParam);
			}
			if(this.endCallback != null) this.endCallback(this.effectParam);
			return;
		}

		this.StartCoroutine(this.StarEffectItem (this.EffectCallback));
	}
}
