/*********************************************************************************
 *Copyright(C) 2015 by DefaultCompany
 *All rights reserved.
 *FileName:     GMLoader.cs
 *Author:       DefaultCompany
 *Version:      1.0
 *UnityVersion：5.4.1f1
 *Date:         2016-09-28
 *Description:   用于加载gamemanager脚本，保持场景内脚本唯一
 *              把这个脚本放到某个场景中预先存在的对象上，在检视视图关联GameManager脚本所在的prefab
 *History:  
**********************************************************************************/
using UnityEngine;
using System.Collections;

public class GMLoader : MonoBehaviour
{
    public GameObject gamemanager;
    public void Awake()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gamemanager);
        }
    }
}
