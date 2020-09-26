//=============================
//Author: Zack Yang 
//Created Date: 09/25/2020 16:03
//=============================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    //Fields
    #region Fields of UI inputField components for setting panel
    private InputField population;
    private InputField infectionRate;
    private InputField incubationPeriod;
    private InputField capacity;
    private InputField intention;
    private InputField originalInfected;
    private InputField responseTime;
    #endregion

    #region Fields of UI Button components for setting panel
    private Button     beginBtn;
    private Button     stopBtn;
    #endregion

    //Methods
    #region Methods of monobehaviors
    void Start()
    {
        //get all the fields
        population = GetComponent<InputField>("populationInputField");
        infectionRate = GetComponent<InputField>("infectionRateInputField");
        incubationPeriod = GetComponent<InputField>("incubationPeriodInputField");
        capacity = GetComponent<InputField>("capacityInputField");
        intention = GetComponent<InputField>("intentionInputField");
        originalInfected = GetComponent<InputField>("originalInfectedInputField");
        responseTime = GetComponent<InputField>("responseTimeInputField");
        beginBtn = GetComponent<Button>("startSimulationBtn");
        stopBtn = GetComponent<Button>("stopSimulationBtn");

        //initialize values
        stopBtn.gameObject.SetActive(false);
        population.text = "500";
        infectionRate.text = "5";
        incubationPeriod.text = "14";
        capacity.text = "0";
        intention.text = "100";
        originalInfected.text = "50";
        responseTime.text = "0";

        beginBtn.onClick.AddListener(beginBtnClick);
        stopBtn.onClick.AddListener(stopBtnClick);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PoolMgr.GetInstance().Clear();
            Application.Quit();
        }
    }
    #endregion

    #region Methods of btn click events
    void beginBtnClick()
    {
        //reset data
        ResetInfo();
        EventCenter.GetInstance().EventTrigger<int>("Add People", Convert.ToInt32(population.text));

        beginBtn.gameObject.SetActive(false);
        stopBtn.gameObject.SetActive(true);
        EventCenter.GetInstance().EventTrigger<int>("Init Data", Virus.capacity);
    }

    void stopBtnClick()
    {
        EventCenter.GetInstance().EventTrigger("Clear People");
        EventCenter.GetInstance().EventTrigger("Game Stop");
        stopBtn.gameObject.SetActive(false);
        beginBtn.gameObject.SetActive(true);
    }
    #endregion

    #region Methods of game setting initialization
    void ResetInfo()
    {
        if (population.text == "")
            population.text = "500";

        if (originalInfected.text == "")
            originalInfected.text = "50";

        if (incubationPeriod.text == "")
            incubationPeriod.text = "14";

        if (capacity.text == "")
            capacity.text = "0";

        if (intention.text == "")
            intention.text = "100";

        if (responseTime.text == "")
            responseTime.text = "0";
        Virus.population = int.Parse(population.text);
        Virus.originalInfected = int.Parse(originalInfected.text);
        Virus.incubationPeriod = int.Parse(incubationPeriod.text);
        Virus.infectionRate = int.Parse(infectionRate.text);
        Virus.capacity = int.Parse(capacity.text);
        Virus.intentionToMove = int.Parse(intention.text);
        Virus.responseTime = int.Parse(responseTime.text);
    }
    #endregion
}
