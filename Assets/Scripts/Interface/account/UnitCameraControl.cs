using UnityEngine;
using System.Collections;

public class UnitCameraControl : MonoBehaviour
{
    public GameObject UnitCamera;

    void OnEnable()
    {
        UnitCamera.SetActive(false);
    }

    void OnDisable()
    {
		if(UnitCamera != null) UnitCamera.SetActive(true);
    }
}
