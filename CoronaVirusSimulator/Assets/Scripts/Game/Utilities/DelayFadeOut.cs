//=============================
//Author: Zack Yang 
//Created Date: 09/22/2020 21:46
//=============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayFadeOut : MonoBehaviour
{
    //Methods
    #region methods of the show time of every trump speech
    /// <summary>
    /// the show time of every trump speech
    /// </summary>
    public void DelayToFadeOut()
    {
        GetComponent<EasyTween>().Invoke("OpenCloseObjectAnimation", 3);
    }
    #endregion
}
