using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSViewControl : MonoBehaviour 
{
    public List<GameObject> bsViews = new List<GameObject>();


    public void SetBSViews()
    {
        foreach (GameObject g in bsViews)
        {
            if (g.name == "Sprite_Main" || g.name == "Background")
            {
                if (g.activeSelf == false)
                {
                    g.SetActive(true);
                }
            }
            else if (g.activeSelf == true)
            {
                g.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        SetBSViews();
    }

    void OnDisable()
    {
        foreach (GameObject g in bsViews)
        {
            g.SetActive(false);
        }
    }
}
