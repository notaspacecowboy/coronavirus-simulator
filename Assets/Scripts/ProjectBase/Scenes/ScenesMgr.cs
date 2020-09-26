using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


/// <summary>
/// 场景切换管理器
/// 封装了同步和异步切换场景的两个方法
/// 协程，异步加载，委托
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    /// <summary>
    /// 同步加载场景
    /// 缺点：切换场景时会产生卡顿
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="doList"></param>
    public void LoadScene(string sceneName, UnityAction doList)
    {
        SceneManager.LoadScene(sceneName);
        doList();
    }
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="doList"></param>
    public void LoadSceneAsync(string sceneName, UnityAction doList)
    {
        MonoMgr.GetInstance().StartCoroutine(RealLoadSceneAsync(sceneName, doList));
    }

    /// <summary>
    /// 协程会一直执行到yield return，然后暂时挂起，等到下次gameobj的fixedupdate/update
    /// 函数被调用时，再判断yield return后的条件是否满足，如果满足就进入协程继续执行
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="doList"></param>
    /// <returns></returns>
    public IEnumerator RealLoadSceneAsync(string sceneName, UnityAction doList)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
        {
            //事件中心向外界分发进度条状态，外部想用就用
            EventCenter.GetInstance().EventTrigger("loading update", ao.progress);
            yield return ao.progress;
        }
    }
}
