//=============================
//Author: Zack Yang 
//Created Date: 09/25/2020 16:03
//=============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TrumpPanel : BasePanel
{
    //Fields
    #region Fields of text component
    [Header("First of the trump speech")]
    public Text trumptxt1;
    #endregion

    //Methods
    #region Methods of monobehaviors
    void Start()
    {
       trumptxt1.GetComponent<EasyTween>().OpenCloseObjectAnimation();
    }
    #endregion
}
