//=============================
//Author: Zack Yang 
//Created Date: 09/24/2020 13:27
//=============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataPanel : BasePanel
{
    //Fields
    #region Fields of UI text components for data panel
    private Text healthy;
    private Text incubation;
    private Text infected;
    private Text capacity;
    #endregion

    #region Fields of hospital capacity data that will be shown on the panel
    private int capacityNum;
    private int currentCapacity;
    #endregion

    //Methods
    #region Methods of nomobehaviours
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<int>("Init Data", Init);
    }
    #endregion

    #region Methods of initialization when game start
    void Init(int num)
    {
        healthy = GetComponent<Text>("healthyTxt");
        incubation = GetComponent<Text>("inbationTxt");
        infected = GetComponent<Text>("infectedTxt");
        capacity = GetComponent<Text>("capacityTxt");
        capacityNum = num;
        currentCapacity = num;

        healthy.text = (Virus.population - Virus.originalInfected).ToString();
        incubation.text = "0";
        infected.text = Virus.originalInfected.ToString();
        capacity.text = currentCapacity.ToString() + "/" + capacityNum.ToString();

        EventCenter.GetInstance().AddEventListener("Game Stop", Clear);

        EventCenter.GetInstance().AddEventListener<Person.E_Person_Status>("Minus Data", MinusData);

        EventCenter.GetInstance().AddEventListener<Person.E_Person_Status>("Add Data", AddData);
    }
    #endregion

    #region Methods to call when receive manipulate data event
    void MinusData(Person.E_Person_Status status)
    {
        switch (status)
        {
            case Person.E_Person_Status.Healthy:
                healthy.text = (int.Parse(healthy.text) - 1).ToString();
                break;

            case Person.E_Person_Status.Incubation:
                incubation.text = (int.Parse(incubation.text) - 1).ToString();
                break;

            case Person.E_Person_Status.Infected:
                infected.text = (int.Parse(infected.text) - 1).ToString();
                break;

            case Person.E_Person_Status.Hospital:
                currentCapacity++;
                capacity.text = currentCapacity.ToString() + "/" + capacityNum.ToString();
                break;
        }
    }

    void AddData(Person.E_Person_Status status)
    {
        switch (status)
        {
            case Person.E_Person_Status.Healthy:
                healthy.text = (int.Parse(healthy.text) + 1).ToString();
                break;

            case Person.E_Person_Status.Incubation:
                incubation.text = (int.Parse(incubation.text) + 1).ToString();
                break;

            case Person.E_Person_Status.Infected:
                infected.text = (int.Parse(infected.text) + 1).ToString();
                break;

            case Person.E_Person_Status.Hospital:
                currentCapacity--;
                capacity.text = currentCapacity.ToString() + "/" + capacityNum.ToString();
                break;
        }
    }
    #endregion

    #region Methods to call to clear data when game over
    void Clear()
    {
        EventCenter.GetInstance().RemoveEventListener<Person.E_Person_Status>("Minus Data", MinusData);
        EventCenter.GetInstance().RemoveEventListener<Person.E_Person_Status>("Add Data", AddData);
        EventCenter.GetInstance().RemoveEventListener("Game Stop", Clear);
        healthy.text = "0";
        incubation.text = "0";
        infected.text = "0";
        capacity.text = "0";
    }
    #endregion
}
