//=============================
//Author: Zack Yang 
//Created Date: 09/24/2020 14:33
//=============================
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Virus
{
    //Fields
    #region Field and property of population
    /// <summary>
    /// original infected population
    /// </summary>
    private static int _population = 500;
    public static int population
    {
        get { return _population; }
        set { _population = Mathf.Clamp(value, 0, 2000); }
    }
    #endregion

    #region Field and property of original infected population
    private static int _originalInfected = 5;
    public static int originalInfected
    {
        get { return Mathf.Clamp(_originalInfected, 0, population); }
        set { _originalInfected = value; }
    }
    #endregion

    #region Field and property of infection rate
    private static float _infectionRate = 0.5f;
    public static float infectionRate
    {
        get { return Mathf.Clamp(_infectionRate, 0f, 1f); }
        set { _infectionRate = value / 100.0f; }
    }
    #endregion

    #region Field and property of incubation rate
    private static int _incubationPeriod;
    public static int incubationPeriod
    {
        get { return _incubationPeriod; }
        set { _incubationPeriod = value; }
    }
    #endregion

    #region Field and property of hospital capacity
    private static int _capacity = 0;
    public static int capacity
    {
        get { return _capacity; }
        set { _capacity = value; }
    }
    #endregion

    #region Field and property of hospital response time
    private static int _responseTime = 5;
    public static int responseTime
    {
        get { return _responseTime; }
        set { _responseTime = value; }
    }
    #endregion

    #region Field and property of people's intention to move
    private static float _intentionToMove = 0.1f;
    public static float intentionToMove
    {
        get { return Mathf.Clamp(_intentionToMove, 0f, 1f); }
        set { _intentionToMove = value / 100.0f; }
    }
    #endregion
}
