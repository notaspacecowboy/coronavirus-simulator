using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>      
public class ResMgr : BaseManager<ResMgr>
{
    //同步加在资源
    public T Load<T>(string name) where T:Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是一个GameObject,我们把它实例化后再返回出去，外部可以直接使用
        if (res is GameObject)
            return GameObject.Instantiate(res);
        //对于无需实例化的对象(audioclip,textasset...)直接返回出去
        else
            return res;
    }

    //异步加载资源:减少程序卡顿感
    //通过异步加载的资源无法马上使用，通常需要等待n帧(根据资源大小决定)才能加载完成
    //因此,我们的封装函数返回值为void
    public void LoadAsync<T>(string name, UnityAction<T> callBack) where T:Object
    {
        //开启异步加载的协程
        MonoMgr.GetInstance().StartCoroutine(RealLoadAsync<T>(name,callBack));
    }
    
    //真正的协程函数，用于开启异步加载的资源
    private IEnumerator RealLoadAsync<T>(string name, UnityAction<T> callBack) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callBack(GameObject.Instantiate(r.asset) as T);
        else
            callBack(r.asset as T);

    }
}
