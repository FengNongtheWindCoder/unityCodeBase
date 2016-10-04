/*********************************************************************************
 *Copyright(C) 2015 by FengNongStudio
 *All rights reserved.
 *FileName:     OnMosue_doubleEvent.cs
 *Author:       FengNongStudio
 *Version:      1.0
 *UnityVersion：5.4.1f1
 *Date:         2016-10-04
 *Description:   
 *History:  
**********************************************************************************/
using UnityEngine;
using System.Collections;

public class OnMosue_doubleEvent : MonoBehaviour {

    public float doubleClickDelay = 0.25f;
    public float lastClicktime = 0;
    /// <summary>
    /// 按照给定的时间，将上次点击的记录删除掉，一般是doubleClickDelay
    /// </summary>
    /// <param name="delaytime"></param>
    /// <returns></returns>
    IEnumerator clearLastClicktime(float delaytime)
    {
        yield return new WaitForSecondsRealtime(delaytime);
        lastClicktime = 0;
    }

    /// <summary>
    /// onmouse系列只能处理左键单击
    /// 在这里处理单击还是双击的逻辑
    /// </summary>
    public void OnMouseUpAsButton()
    {
        //判断单双击事件
        //基于两次点击的事件差
        //第一次点击后触发单击事件并记录时间
        //第二次点击如果小于阈值视为双击事件，否则视为另一次单击事件
        if (lastClicktime == 0)
        {
            lastClicktime = Time.unscaledTime;
            StartCoroutine(clearLastClicktime(doubleClickDelay));
            Debug.Log("this is a single click ");
        }
        else if (Time.unscaledTime < (lastClicktime + doubleClickDelay))
        {
            Debug.Log("this is a double click " + (Time.unscaledTime - lastClicktime));
            lastClicktime = 0;
        }
        else
        {
            //因为有clearLastClicktime，这里可能不会进来
            lastClicktime = 0;
            Debug.Log("this is a single click ");
        }
    }
}
