/*********************************************************************************
 *Copyright(C) 2015 by FengNongStudio
 *All rights reserved.
 *FileName:     OnGUI_doubleclick.cs
 *Author:       FengNongStudio
 *Version:      1.0
 *UnityVersion：5.4.1f1
 *Date:         2016-10-04
 *Description:   使用ongui函数处理鼠标双击事件
 *History:  
**********************************************************************************/
using UnityEngine;
using System.Collections;

public class OnGUI_doubleclick : MonoBehaviour
{
    public void OnMouseUpAsButton()
    {

    }

    public bool ismouseover = false;
    //检测当前鼠标是否在上方
    public void OnMouseEnter()
    {
        ismouseover = true;
    }
    public void OnMouseExit()
    {
        ismouseover = false;
    }

    /// <summary>
    /// 处理鼠标事件
    /// </summary>
    public void OnGUI()
    {
        Event e = Event.current;

        if (!ismouseover|| e.type != EventType.MouseDown)
        {
            return;
        }
        if (e.button == 0)
        {
            Debug.Log(e.clickCount);
            if (e.clickCount == 1)
            {
                Debug.Log(" leftclick");
            }
            else if (e.clickCount == 2)
            {
                Debug.Log(" doubleleftclick");
            }
        }
        else if (e.button == 1)
        {
            Debug.Log(" rightclick");
        }
    }
}
