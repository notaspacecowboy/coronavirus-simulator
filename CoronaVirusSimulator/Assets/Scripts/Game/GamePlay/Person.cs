//=============================
//Author: Zack Yang 
//Created Date: 09/24/2020 14:33
//=============================
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Person : MonoBehaviour
{
    //enums
    #region enum of person's health status
    /// <summary>
    /// all health status of person
    /// </summary>
    public enum E_Person_Status
    {
        Healthy,
        Incubation,
        Infected,
        Hospital,
        Null
    }
    #endregion


    //fields
    #region Fields of person's space, status and image
    /// <summary>
    /// the region for people to move within
    /// </summary>
    private RectTransform mSpace;
    /// <summary>
    /// the health status of person
    /// </summary>
    private E_Person_Status mStatus = E_Person_Status.Null;
    /// <summary>
    /// the image of person
    /// </summary>
    private Image mImg;
    /// <summary>
    /// the city object
    /// </summary>
    private City mCity;
    /// <summary>
    /// if the game has started
    /// </summary>
    private bool gameStart = false;
    #endregion

    #region fields of person's moving direction
    /// <summary>
    /// the number of frame before each direction change
    /// </summary>
    private int changeDirFrame = 100;

    /// <summary>
    /// current frame number
    /// </summary>
    private int currentFrame = 0;

    /// <summary>
    /// the moving direction of person
    /// </summary>
    private Vector3 direction = Vector3.zero;

    /// <summary>
    /// the moving speed of the person
    /// </summary>
    private float speed = 0;
    #endregion

    #region Fields of person's infection logic
    /// <summary>
    /// the number of frame before each infection check
    /// </summary>
    private int checkInfectionFrame = 15;

    /// <summary>
    /// current frame number
    /// </summary>
    private int currentInfectionFrame = 0;
    /// <summary>
    /// the number of frame before a infected person get into hospital
    /// </summary>
    private float waitTimeToHospital = 0;
    #endregion

    #region Fields of person's incubation period
    private float waitTimeToInfected = 0f;
    #endregion

    //methods
    #region methods of person's initialization
    /// <summary>
    /// Initialize the properties of person when it is created
    /// </summary>
    /// <param name="space"></param>
    public void Init(RectTransform space, City city = null)
    {
        gameObject.transform.parent = space.gameObject.transform;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localPosition = Vector3.zero;
        changeDirFrame = Random.Range(100, 200);
        currentFrame = changeDirFrame + 1;
        speed = Random.Range(10f, 15f);
        mCity = city;

        mSpace = space;
        mImg = GetComponent<Image>();

        if(space.name == "cityRange")
            ChangeStatus(E_Person_Status.Healthy);
        else
            ChangeStatus(E_Person_Status.Hospital);
        gameObject.transform.localPosition = new Vector3(mSpace.rect.center.x - mSpace.rect.width/2 + Random.Range(0, mSpace.rect.width),
                                                         mSpace.rect.center.y - mSpace.rect.height/2 + Random.Range(0, mSpace.rect.height),
                                                         0);

        EventCenter.GetInstance().AddEventListener("Game Start", () =>
        {
            gameStart = true;
        });

        EventCenter.GetInstance().AddEventListener("Game Stop", () =>
        {
            gameStart = false;
        });
    }
    #endregion

    #region methods to change person's health status

    /// <summary>
    /// change the health status of person
    /// </summary>
    /// <param name="status"></param>
    public void ChangeStatus(E_Person_Status status)
    {
        mStatus = status;
        switch (status)
        {
            case E_Person_Status.Healthy:
                mImg.color = Color.white;
                break;
            case E_Person_Status.Incubation:
                waitTimeToInfected = 0;
                mImg.color = Color.yellow;
                break;
            case E_Person_Status.Infected:
                waitTimeToHospital = 0;
                mImg.color = Color.red;
                break;
            case E_Person_Status.Hospital:
                mImg.color = Color.blue;
                break;
        }
    }
    #endregion

    #region methods of person's random move

    /// <summary>
    /// implement the random move behavior of each person
    /// </summary>
    private void RandomMove()
    {
        currentFrame++;
        if (currentFrame > changeDirFrame)
        {
            if (Random.Range(0f, 1f) < Virus.intentionToMove)
            {
                direction.Set(Random.Range(-50, 50), Random.Range(-50, 50), 0);
                direction.Normalize();
            }
            else
                direction = Vector3.zero;

            currentFrame = 0;
        }

        if (gameObject.transform.localPosition.x < mSpace.rect.center.x - mSpace.rect.width / 2)
            direction = Vector3.right;

        if (gameObject.transform.localPosition.x > mSpace.rect.center.x + mSpace.rect.width / 2)
            direction = Vector3.left;

        if (gameObject.transform.localPosition.y < mSpace.rect.center.y - mSpace.rect.height / 2)
            direction = Vector3.up;

        if (gameObject.transform.localPosition.y > mSpace.rect.center.y + mSpace.rect.height / 2)
            direction = Vector3.down;

        gameObject.transform.Translate(direction * Time.deltaTime * speed);
    }
    #endregion

    #region methods of how person infect others
    void CheckInfection()
    {
        if (mStatus == E_Person_Status.Infected || mStatus == E_Person_Status.Incubation)
        {
            currentInfectionFrame++;
            if (currentInfectionFrame > checkInfectionFrame && mCity != null)
            {
                StartCoroutine(CheckInfectionCoroutine());
            }
        }
    }

    IEnumerator CheckInfectionCoroutine()
    {
        float infectionRate = Virus.infectionRate * Virus.intentionToMove;
        for(int i = 0; mCity!= null && i < mCity.people.Count; i++)
        {
            if (mCity.people[i].mStatus == E_Person_Status.Healthy && Vector3.Distance(gameObject.transform.localPosition, mCity.people[i].gameObject.transform.localPosition) < 20f && Random.Range(0, 1) < infectionRate)
            {
                EventCenter.GetInstance().EventTrigger<E_Person_Status>("Minus Data", mCity.people[i].mStatus);
                if (Virus.incubationPeriod > 0)
                    mCity.people[i].ChangeStatus(E_Person_Status.Incubation);
               
                else
                    mCity.people[i].ChangeStatus(E_Person_Status.Infected);
        
                    
                EventCenter.GetInstance().EventTrigger<E_Person_Status>("Add Data", mCity.people[i].mStatus);
            }

            if (i % 100 == 0)
                yield return 0;
        }
    }
    #endregion

    #region methods of person's check in and check from to a hospital
    /// <summary>
    /// if infected and wait for more than the minimum time to be witnessed by hospital,
    /// publish an event
    /// </summary>
    void CheckHospital()
    {
        if (mStatus == E_Person_Status.Infected)
        {
            waitTimeToHospital += Time.deltaTime;

            if (waitTimeToHospital > Virus.responseTime)
                EventCenter.GetInstance().EventTrigger<Person>("Add Patient", this);
        }
    }
    #endregion

    #region methods of how person change from incubated to infected
    /// <summary>
    /// check if a person pass his incubation period
    /// </summary>
    void IncubationToInfected()
    {
        if (mStatus == E_Person_Status.Incubation)
        {
            waitTimeToInfected += Time.deltaTime;
            if (waitTimeToInfected > Virus.incubationPeriod)
            {
                EventCenter.GetInstance().EventTrigger<Person.E_Person_Status>("Minus Data", Person.E_Person_Status.Incubation);
                EventCenter.GetInstance().EventTrigger<Person.E_Person_Status>("Add Data", Person.E_Person_Status.Infected);
                ChangeStatus(E_Person_Status.Infected);
            }
        }
    }
    #endregion

    #region Methods of monobehaviors
    void Update()
    {
        if (gameStart)
        {
            RandomMove();
            CheckInfection();
            CheckHospital();
            IncubationToInfected();
        }
    }
    #endregion
}
