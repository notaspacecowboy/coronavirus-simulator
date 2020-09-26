//=============================
//Author: Zack Yang 
//Created Date: 09/24/2020 15:19
//=============================
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(City), typeof(Hospital))]
public class GamePanel : BasePanel
{
    //Fields
    #region Fields of image, city, hospital
    /// <summary>
    /// image of loading animation
    /// </summary>
    private Image loadingAnimation;
    
    /// <summary>
    /// city component of the game panel
    /// </summary>
    private City city;

    /// <summary>
    /// hospital component of the game panel
    /// </summary>
    private Hospital hospital;
    // Start is called before the first frame update
    #endregion

    //Methods
    #region Methods of monobehaviors
    void Start()
    {
        city = GetComponent<City>();
        hospital = GetComponent<Hospital>();
        city.Init();
        hospital.Init();
    }
    #endregion
}
