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
    private RectTransform _space;
    /// <summary>
    /// the health status of person
    /// </summary>
    private E_Person_Status _status = E_Person_Status.Null;
    /// <summary>
    /// the image of person
    /// </summary>
    private Image _img;
    /// <summary>
    /// the city object
    /// </summary>
    private City _city;
    /// <summary>
    /// if the game has started
    /// </summary>
    private bool _gameStart = false;
    #endregion

    #region fields of person's moving direction
    /// <summary>
    /// the number of frame before each direction change
    /// </summary>
    private int _changeDirFrame = 100;

    /// <summary>
    /// current frame number
    /// </summary>
    private float _currentFrame = 0;

    /// <summary>
    /// the moving direction of person
    /// </summary>
    private Vector3 _direction = Vector3.zero;

    /// <summary>
    /// the moving speed of the person
    /// </summary>
    private float _speed = 0;
    #endregion

    #region Fields of person's infection logic
    /// <summary>
    /// the number of frame before each infection check
    /// </summary>
    private int _checkInfectionFrame = 30;

    /// <summary>
    /// current frame number
    /// </summary>
    private int _currentInfectionFrame = 0;
    /// <summary>
    /// the number of frame before a infected person get into hospital
    /// </summary>
    private float _waitTimeToHospital = 0;
    #endregion

    #region Fields of person's incubation period
    private float _waitTimeToInfected = 0f;
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
        _changeDirFrame = Random.Range(100, 200);
        _currentFrame = _changeDirFrame + 1;
        _speed = Random.Range(20f, 30f);
        _city = city;

        _space = space;
        _img = GetComponent<Image>();

        if(space.name == "cityRange")
            ChangeStatus(E_Person_Status.Healthy);
        else
            ChangeStatus(E_Person_Status.Hospital);
        gameObject.transform.localPosition = new Vector3(_space.rect.center.x - _space.rect.width/2 + Random.Range(0, _space.rect.width),
                                                         _space.rect.center.y - _space.rect.height/2 + Random.Range(0, _space.rect.height),
                                                         0);

        EventCenter.GetInstance().AddEventListener("Game Start", () =>
        {
            _gameStart = true;
        });

        EventCenter.GetInstance().AddEventListener("Game Stop", () =>
        {
            _gameStart = false;
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
        _status = status;
        switch (status)
        {
            case E_Person_Status.Healthy:
                _img.color = Color.white;
                break;
            case E_Person_Status.Incubation:
                _waitTimeToInfected = 0;
                _img.color = Color.yellow;
                break;
            case E_Person_Status.Infected:
                _waitTimeToHospital = 0;
                _img.color = Color.red;
                break;
            case E_Person_Status.Hospital:
                _img.color = Color.blue;
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
        _currentFrame++;
        if (_currentFrame > _changeDirFrame)
        {
            if (Random.Range(0f, 1f) < Virus.intentionToMove)
            {
                _direction.Set(Random.Range(-50, 50), Random.Range(-50, 50), 0);
                _direction.Normalize();
            }
            else
                _direction = Vector3.zero;

            _currentFrame = 0;
        }

        if (gameObject.transform.localPosition.x < _space.rect.center.x - _space.rect.width / 2)
            _direction = Vector3.right;

        if (gameObject.transform.localPosition.x > _space.rect.center.x + _space.rect.width / 2)
            _direction = Vector3.left;

        if (gameObject.transform.localPosition.y < _space.rect.center.y - _space.rect.height / 2)
            _direction = Vector3.up;

        if (gameObject.transform.localPosition.y > _space.rect.center.y + _space.rect.height / 2)
            _direction = Vector3.down;

        gameObject.transform.Translate(_direction * Time.deltaTime * _speed);
    }
    #endregion

    #region methods of how person infect others
    void CheckInfection()
    {
        if (_status == E_Person_Status.Infected || _status == E_Person_Status.Incubation)
        {
            _currentInfectionFrame ++;
            if (_currentInfectionFrame > _checkInfectionFrame && _city != null)
            {
                StartCoroutine(CheckInfectionCoroutine());
                _currentInfectionFrame = 0;
            }
        }
    }

    IEnumerator CheckInfectionCoroutine()
    {
        float infectionRate = Virus.infectionRate * Virus.intentionToMove;
        Debug.Log(infectionRate);
        for(int i = 0; _city != null && i < _city.people.Count; i++)
        {
            if (_city.people[i]._status == E_Person_Status.Healthy && Vector3.Distance(gameObject.transform.localPosition, _city.people[i].gameObject.transform.localPosition) < 20f && Random.Range(0f, 1f) < infectionRate)
            {
                EventCenter.GetInstance().EventTrigger<E_Person_Status>("Minus Data", _city.people[i]._status);
                if (Virus.incubationPeriod > 0)
                    _city.people[i].ChangeStatus(E_Person_Status.Incubation);
               
                else
                    _city.people[i].ChangeStatus(E_Person_Status.Infected);
        
                    
                EventCenter.GetInstance().EventTrigger<E_Person_Status>("Add Data", _city.people[i]._status);
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
        if (_status == E_Person_Status.Infected)
        {
            _waitTimeToHospital += Time.deltaTime;

            if (_waitTimeToHospital > Virus.responseTime)
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
        if (_status == E_Person_Status.Incubation)
        {
            _waitTimeToInfected += Time.deltaTime;
            if (_waitTimeToInfected > Virus.incubationPeriod)
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
        if (_gameStart)
        {
            RandomMove();
            CheckInfection();
            CheckHospital();
            IncubationToInfected();
            //Cure();
        }
    }
    #endregion
}
