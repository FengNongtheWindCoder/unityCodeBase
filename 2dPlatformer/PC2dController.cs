using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PC2dController : MonoBehaviour {

    public Vector2 _curVelocity = Vector2.zero;//计算出的当前速度

    //重力系数
    public float gravityModifier = 1;
    //地面运动参数
    public float groundSpeed = 7f;//最大跑动速度
    public float timeToGroundSpeed = 0.5f;//达到最大速度时间
    public float groundStopDistance = 0.5f;//停止移动需要的距离
    public float speedThreshold = 0.1f;//最小移动速度，小于此值速度归零
    private float groundAcceleration;//加速 减速是计算出的
    private float groundDeceleration;//加速 减速是计算出的

    private PC2dStateManager stateManager;
    private Rigidbody2D rb2d;
    // Use this for initialization
    void Start() {
        stateManager = new PC2dStateManager();
        stateManager.OnStateChange += Statechanged;
        groundAcceleration = groundSpeed / timeToGroundSpeed;
        groundDeceleration = (groundSpeed * groundSpeed) / (2 * groundStopDistance);
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        //检测输入
        float xInput = Input.GetAxisRaw("Horizontal");

        //计算速度
        float inputSign = Mathf.Sign(xInput);//0的sign是1
        float xVelocitySign = Mathf.Sign(_curVelocity.x);
        float xVelocity = _curVelocity.x;

        //静止状态，按照输入方向加速
        if (xVelocity == 0 && xInput != 0) {
            xVelocity += xInput * groundAcceleration * Time.deltaTime;
        } else if (xVelocity != 0) {
            //非静止状态
            //水平输入与速度同方向加速，反方向减速, 无输入减速
            if (xInput != 0 && inputSign == xVelocitySign) {
                xVelocity += xVelocitySign * groundAcceleration * Time.deltaTime;
            } else {
                xVelocity += -1 * xVelocitySign * groundDeceleration * Time.deltaTime;
                xVelocity = xVelocity < speedThreshold ? 0 : xVelocity;
            }
        }
        _curVelocity.x = Mathf.Clamp(xVelocity, -1 * groundSpeed, groundSpeed);
        //计算重力影响。使用的Physics2D重力设置
        _curVelocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
    }

    void FixedUpdate() {
        //测试碰撞
        //rb2d.Cast();
        //计算运动
        Vector2 targetPos = rb2d.position + _curVelocity * Time.deltaTime;
        rb2d.MovePosition(targetPos);
    }

    //监听状态改变
    void Statechanged(UserState fromState, UserState toState) {
        Debug.Log(fromState.ToString() + " => " + toState.ToString());
    }
}
