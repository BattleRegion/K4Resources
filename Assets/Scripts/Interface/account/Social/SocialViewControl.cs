using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocialViewControl : MonoBehaviour
{

    public List<GameObject> SocialViews = new List<GameObject>();


    public void SetBSViews()
    {
        foreach (GameObject g in SocialViews)
        {
            if (g.name == "Sprite_Main")
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
}
