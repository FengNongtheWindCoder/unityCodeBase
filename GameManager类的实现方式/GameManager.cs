/*********************************************************************************
 *Copyright(C) 2015 by DefaultCompany
 *All rights reserved.
 *FileName:     GameManager.cs
 *Author:       DefaultCompany
 *Version:      1.0
 *UnityVersion：5.4.1f1
 *Date:         2016-09-27
 *Description:   负责处理游戏开始结束的逻辑，提供游戏结束开始的事件，场景唯一
 *History:  
**********************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameFinishType { Success,Failed}

    public static GameManager instance;
    public event Action GameStartEvent;
    public event Action<GameFinishType> GameFinishEvent;
    public bool isgamefinish = false;
    void Awake()
    {
        //保持此脚本场景中唯一，也是保持脚本所在的GameManager对象场景唯一的手段
        //但主要还是靠loader，否则这个脚本只能在update之前才删除，可能对场景内有影响的
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //保持场景切换不删除
        DontDestroyOnLoad(gameObject);
        InitGame();
    }
    /// <summary>
    /// reset data
    /// </summary>
    void OnLevelWasLoaded()
    {
        InitGame();
    }

    // Use this for initialization
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// Initializes the game.
    /// </summary>
    void InitGame()
    {
    }
    /// <summary>
    /// 特殊处理点击之后游戏才开始的情况 
    /// </summary>
    public void startGame( )
    {
        if (GameStartEvent != null)
        {
            GameStartEvent();
        }
    }
    public bool checkGameSuccess()
    {
        //if (*some success condition*)
        //{
        //    Debug.Log("game success");
        //    isgamefinish = true;
        //    if (GameFinishEvent != null)
        //    {
        //        GameFinishEvent(GameFinishType.Success);
        //    }
        //    return true;
        //}
        return false;
    }
    public void gameOver()
    {
        isgamefinish = true;
        if (GameFinishEvent != null)
        {
            GameFinishEvent(GameFinishType.Failed);
        }
        Debug.Log("game over");
    }
}
