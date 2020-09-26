using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入控制模块
/// 统一管理输入相关的逻辑
/// 暂时只针对pc段按键
/// 将游戏中输入相关的操作独立出来,降低程序耦合性
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    public InputMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }

    //选择是否开启玩家输入检测
    public void setStatus(bool status)
    {
        isOpen = status;
    }
    private void CheckKeyCode(KeyCode key)
    {
        //分发按键按下或抬起事件至事件中心
        if (Input.GetKeyDown(key))
        {
            EventCenter.GetInstance().EventTrigger("keydown", key);
        }
        if (Input.GetKeyUp(key))
        {
            EventCenter.GetInstance().EventTrigger("keyup", key);
        }
    }
    private void Update()
    {
        if (!isOpen)
            return;
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.D);
    }

    private bool isOpen = false;
}
