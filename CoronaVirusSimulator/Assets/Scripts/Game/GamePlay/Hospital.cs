//=============================
//Author: Zack Yang 
//Created Date: 09/24/2020 19:27
//=============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : MonoBehaviour
{
    //Fields
    #region Fields of hospital capacity, hospital UI range
    /// <summary>
    /// hospital current capacity
    /// </summary>
    private int currentCapacity = 0;
    /// <summary>
    /// hospital UI range
    /// </summary>
    public RectTransform hospitalRange;
    #endregion

    //Methods
    #region Methods of Initialization
    public void Init()
    {
        currentCapacity = 0;
        EventCenter.GetInstance().AddEventListener<Person>("Add Patient", AddPatient);
    }
    #endregion

    #region Methods of add patient when there are still enough capacity
    public void AddPatient(Person p)
    {
        if (currentCapacity < Virus.capacity)
        {
            EventCenter.GetInstance().EventTrigger<Person.E_Person_Status>("Minus Data", Person.E_Person_Status.Infected);
            EventCenter.GetInstance().EventTrigger<Person.E_Person_Status>("Add Data", Person.E_Person_Status.Hospital);
            p.Init(hospitalRange);
            currentCapacity++;
        }
    }
    #endregion
}
