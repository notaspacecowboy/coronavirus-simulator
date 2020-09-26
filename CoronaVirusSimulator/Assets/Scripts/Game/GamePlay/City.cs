//=============================
//Author: Zack Yang 
//Created Date: 09/24/2020 14:32
//=============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    //Fileds
    #region Field of animation image
    /// <summary>
    /// the loading animation image when game start
    /// </summary>
    public Image loadingAnimation;
    #endregion

    #region Field of city's range, people and currentInfected
    /// <summary>
    /// the range of city
    /// </summary>
    public RectTransform cityRange;
    /// <summary>
    /// the list of all current alive people
    /// </summary>
    public List<Person> people = new List<Person>();
    /// <summary>
    /// amount of current infected people. Only use for initialzation.
    /// </summary>
    private int currentInfected = 0;
    #endregion


    //Methods
    #region Methods of initialization
    public void Init()
    {
        EventCenter.GetInstance().AddEventListener<int>("Add People", AddPerson);
        EventCenter.GetInstance().AddEventListener("Clear People", Clear);
    }
    #endregion

    #region Methods of add people to city when game start

    /// <summary>
    /// add all people to city
    /// </summary>
    /// <param name="num"></param>
    public void AddPerson(int num)
    {
        currentInfected = 0;
        StartCoroutine(BeginCreate(num));
    }

    /// <summary>
    /// add all people to city coroutine
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private IEnumerator BeginCreate(int num)
    {
        if(loadingAnimation != null)
            loadingAnimation.gameObject.SetActive(true);

        for (int i = 0; i < num; i++)
        {
            PoolMgr.GetInstance().GetObj("Prefabs/Person", (obj) =>
            {
                Person p = obj.GetComponent<Person>();
                people.Add(p);
                p.Init(cityRange, this);

                if (currentInfected < Virus.originalInfected)
                {
                    p.ChangeStatus(Person.E_Person_Status.Infected);
                    currentInfected++;
                }
            });

            if (i % 100 == 0)
                yield return 0;
        }

        yield return new WaitForSeconds(2);
        loadingAnimation.gameObject.SetActive(false);
        EventCenter.GetInstance().EventTrigger("Game Start");
    }
    #endregion

    #region Methods of delete all people from city when game ends
    /// <summary>
    /// clear people from city
    /// </summary>
    public void Clear()
    {
        StartCoroutine(beginClear());
    }

    /// <summary>
    /// clear people from city coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator beginClear()
    {
        for (int i = 0; i < people.Count; i++)
        {
            people[i].ChangeStatus(Person.E_Person_Status.Null);
            PoolMgr.GetInstance().PushObj("Prefabs/Person", people[i].gameObject);
            if (i % 100 == 0)
                yield return 0;
        }
        people.Clear();
    }
    #endregion
}
