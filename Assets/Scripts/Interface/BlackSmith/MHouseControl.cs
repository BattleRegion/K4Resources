using UnityEngine;
using System.Collections;

public class MHouseControl : MonoBehaviour, materialBagInter
{
    public MaterialBagControl MaterialBag;

    void OnEnable()
    {
        MaterialBag.inter = this;
        MaterialBag.DelaySortMaterial();
    }

    void OnDisable()
    {
        MaterialBag.ClearMaterialBag();
    }

    public void _OnClickMaterialInter(int Id)
    {
        MaterialBag._OnLongPressMaterial(Id);
    }

    public void _OnLongPressMaterialInter(int Id)
    {
    }
}
