using UnityEngine;
using System.Collections;

public class ArenaSummaryUI : MonoBehaviour 
{
	public UISlider uiSlider;

	void Awake()
	{
		this.uiSlider.value = 0f;
	}
}
