using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//UI层级的枚举
public enum UILayer
{
    System,
    Top,
    Mid,
    Bot
};

/// <summary>
/// UI管理器
/// 1.管理所有当前正在显示的面板
/// 2.提供给外部 显示 和 隐藏面板的接口
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    public UIMgr()
    {
        //创建canvas和EventSystem
        GameObject canvasObj = ResMgr.GetInstance().Load<GameObject>("UI/Canvas");
        GameObject eventObj = ResMgr.GetInstance().Load<GameObject>("UI/EventSystem");
        //记录UI的画布位置,以方便外部使用
        canvas = canvasObj.transform as RectTransform;
        //使canvas和eventsystem在过场景是不被移除
        GameObject.DontDestroyOnLoad(canvasObj);
        GameObject.DontDestroyOnLoad(eventObj);

        //找到各个层级
        system = canvas.Find("System");
        top = canvas.Find("Top");
        mid = canvas.Find("Mid");
        bot = canvas.Find("Bot");
    }

    /// <summary>
    /// 在一个指定的UI层级上,显示一个指定的面板,并处理面板加载完成后的逻辑
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">面板所处的UI层级</param>
    /// <param name="callBackAction">完成面板加载后的回调函数</param>
    public void ShowPanel<T>(string panelName, UILayer layer = UILayer.Mid, UnityAction<T> callBackAction = null) where T: BasePanel
    {
        //避免面板的重复加载,如果面板已被加载,就直接显示面板->调用回调函数->返回
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].ShowMe();
            if(callBackAction != null)
                callBackAction(panels[panelName] as T);
            return;
        }

        ResMgr.GetInstance().LoadAsync<GameObject>("UI/Panels/" + panelName,
            (obj) =>
            {
                Transform father;
                switch (layer)
                {
                    case UILayer.System:
                        father = system;
                        break;
                    case UILayer.Top:
                        father = top;
                        break;
                    case UILayer.Mid:
                        father = mid;
                        break;
                    default:
                        father = bot;
                        break;
                }

                //设置面板的父对象,相对位置和大小
                obj.transform.SetParent(father);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                (obj.transform as RectTransform).offsetMax = Vector2.zero;
                (obj.transform as RectTransform).offsetMin = Vector2.zero;

                //得到预设体身上的面板脚本
                T panel = obj.GetComponent<T>();

                panel.ShowMe();

                //处理面板创建完成后的逻辑
                if (callBackAction != null)
                    callBackAction(panel);

                panels.Add(panelName, panel);
            });
    }


    /// <summary>
    /// 隐藏一个特定面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].HideMe();
            GameObject.Destroy(panels[panelName].gameObject);
            panels.Remove(panelName);
        }
    }


    /// <summary>
    /// 得到一个已经显示的面板,以供外部使用
    /// </summary>
    /// <param name="panelName"></param>
    public T GetPanel<T>(string panelName) where T:BasePanel
    {
        if (panels.ContainsKey(panelName))
            return panels[panelName] as T;
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 得到一个层级的父对象的transform
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetLayerTransform(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Bot:
                return bot;

            case UILayer.Mid:
                return mid;

            case UILayer.Top:
                return top;

            case UILayer.System:
                return system;

            default:
                return null;
        }
    }

    
    /// <summary>
    /// 在一个指定的UI控件上加上一个指定的事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBackAction">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBackAction)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBackAction);
        trigger.triggers.Add(entry);
    }

    //面板的层级属性
    public RectTransform canvas;
    private Transform system;
    private Transform top;
    private Transform mid;
    private Transform bot;

    private Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
}
