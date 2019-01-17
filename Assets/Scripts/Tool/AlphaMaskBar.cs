using UnityEngine;
using System.Collections;

/// <summary>
/// 透明shader的进度条
/// </summary>
public class AlphaMaskBar : UITexture
{
    bool SetTag = false;

    public float value
    {
        get
        {
            return material.GetFloat("_Progress");
        }
        set
        {
            material.SetFloat("_Progress", value);
            SetTag = true;
        }
    }

    void Update()
    {
        if(SetTag)
        {
            panel.RebuildAllDrawCalls();
            SetTag = false;
        }
    }
}
