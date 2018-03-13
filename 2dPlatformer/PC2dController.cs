using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PC2dController : MonoBehaviour {
    private Vector2 _curVelocity;//计算出的当前速度
    
    //地面运动参数
    public float groundSpeed;//最大跑动速度
    public float timeToGroundSpeed;//达到最大速度时间
    public float groundStopDistance;//停止移动需要的距离
    private float groundAcceleration;//加速 减速是计算出的
    private float groundDeceleration;//加速 减速是计算出的

    private PC2dStateManager stateManager;

    // Use this for initialization
    void Start () {
        stateManager = new PC2dStateManager();
        stateManager.OnStateChange += statechanged;
        groundAcceleration = groundSpeed / timeToGroundSpeed;
        groundDeceleration = (groundSpeed * groundSpeed) / (2 * groundStopDistance);
    }
    
    // Update is called once per frame
    void Update () {
        //检测输入
        if (Input.GetKeyDown(KeyCode.Space)) {
            stateManager.ChangeStateTo(UserState.Rising);
        }
        //计算速度
        //输入与速度同方向加速，反方向减速
    }

    void FixedUpdate () {
        //测试碰撞
        //计算运动
    }

    //监听状态改变
    void statechanged(UserState fromState, UserState toState) {
        Debug.Log(fromState.ToString() + " => " + toState.ToString());
    }
}
