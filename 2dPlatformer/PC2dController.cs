using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PC2dController : MonoBehaviour {

    public Vector2 _curVelocity = Vector2.zero;//计算出的当前速度

    //重力系数
    public float gravityModifier = 4;
    public float fallGravityModifier = 8;//下落时重力加大，下落变快
    //地面运动参数
    public float groundSpeed = 10f;//最大跑动速度
    public float timeToGroundSpeed = 0.5f;//达到最大速度时间
    public float groundStopDistance = 0.5f;//停止移动需要的距离
    public float speedThreshold = 0.1f;//最小移动速度，小于此值速度归零
    private float groundAcceleration;//加速 减速是计算出的
    private float groundDeceleration;//加速 减速是计算出的
    //空中运动参数
    public float airSpeed = 10f;//空中水平最大速度
    public float timeToAirSpeed = 0.5f;//达到最大空中速度时间，水平方向
    public float airStopDistance = 10f;//空中停止移动需要的距离，水平方向
    private float airAcceleration;//空中加速 减速是计算出的
    private float airDeceleration;//空中加速 减速是计算出的
    //跳跃相关
    public float jumpHeight = 5f;//跳跃高度
    public float extraJumpHeight = 5f;//按住可以增加的最大跳跃高度
    public int airJumpMax = 1; //空中跳跃最大次数
    private float jumpStartSpeed;//起跳速度由重力和跳跃高度决定，
    private float extraJumpTime;//计算得出额外跳跃高度所需要的时间，
    

    //用户输入存储
    private struct stUserInput {
        public float xInput;     //水平输入
        public bool jumpPressed;    //跳跃按键
        public void Reset() {
            //重置所有输入
            xInput = 0;   
            jumpPressed = false;
        }
    }
    private stUserInput userInput;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    private const float shellRadius = 0.01f;
    private PC2dStateManager stateManager;
    private Rigidbody2D rb2d;
    // Use this for initialization
    void Start() {
        stateManager = new PC2dStateManager();
        stateManager.OnStateChange += Statechanged;
        rb2d = GetComponent<Rigidbody2D>();

        //地面加速减速计算
        groundAcceleration = groundSpeed / timeToGroundSpeed;
        groundDeceleration = (groundSpeed * groundSpeed) / (2 * groundStopDistance);
        //空中加速减速计算
        airAcceleration = airSpeed / timeToAirSpeed;
        airDeceleration = (airSpeed * airSpeed) / (2 * airStopDistance);
        //起跳速度计算 v=sqrt(2gh)
        jumpStartSpeed = Mathf.Sqrt(2 * -1 * gravityModifier * Physics2D.gravity.y * jumpHeight);
        //按住跳跃键获得额外高度的时长，忽略重力
        extraJumpTime = extraJumpHeight / jumpStartSpeed;
        //设置空中可跳跃次数
        stateManager.airJumpMax = airJumpMax;

        userInput.Reset();
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update() {
        //检测输入，event类输入按帧重置，只能在update里处理
        userInput.jumpPressed = Input.GetButtonDown("Jump");//ButtonDown只在下一帧重置
        
        //跳跃
        if (userInput.jumpPressed && stateManager.canJump()) {
            stateManager.recordJumpStart(Time.time + extraJumpTime);
        }
    }

    void FixedUpdate() {
        //检测输入，帧率无关的输入检测
        userInput.xInput = Input.GetAxisRaw("Horizontal");
        
        //计算当前速度
        ComputeVelocity();

        //计算运动
        Vector2 deltaMovement = _curVelocity * Time.deltaTime;
        
        //测试碰撞
        //水平方向
        float trymove = deltaMovement.x;
        CheckCollisions(ref trymove, true);
        deltaMovement.x = trymove;
        //垂直方向
        trymove =  deltaMovement.y ;
        CheckCollisions(ref trymove, false);
        deltaMovement.y = trymove;

        //移动位置
        Vector2 targetPos = rb2d.position + deltaMovement;
        rb2d.MovePosition(targetPos);

        //检查当前状态
        if (stateManager.currentState != UserState.Grounded) {
            UserState nextstate = _curVelocity.y > 0 ? UserState.Rising : UserState.Falling;
            stateManager.ChangeStateTo(nextstate);
        }
    }

    /// <summary>
    /// 在此方向和距离下检测是否有碰撞
    /// </summary>
    /// <param name="xOrYMove">移动的大小,有正负</param>
    /// <param name="isXMovement">是否x方向</param>
    void CheckCollisions(ref float xOrYMove, bool isXMovement) {
        Vector2 movement = isXMovement ? new Vector2(xOrYMove, 0) : new Vector2(0, xOrYMove);
        float distance = Mathf.Abs(xOrYMove);
        float sign = Mathf.Sign(xOrYMove);
        int count = rb2d.Cast(movement, contactFilter, hitBuffer, distance + shellRadius);
        for (int i = 0; i < count; i++) {
            float modifiedDistance = hitBuffer[i].distance - shellRadius;
            distance = modifiedDistance < distance ? modifiedDistance : distance;
            if (!isXMovement) {
                if (_curVelocity.y <= 0) {
                    //下落过程中，碰到地面
                    stateManager.ChangeStateTo(UserState.Grounded);
                    stateManager.resetJump();
                }
                //y方向的速度碰到东西后清零
                _curVelocity.y = 0;
            } else {
                //x方向的速度碰到东西后清零
                _curVelocity.x = 0;
            }
        }
        xOrYMove = distance * sign;
    }

    /// <summary>
    /// 基于当前的输入和上一帧的速度计算当前的速度
    /// </summary>
    void ComputeVelocity() {
        float xInput = userInput.xInput;
        float inputSign = Mathf.Sign(xInput);//0的sign是1
        float xVelocitySign = Mathf.Sign(_curVelocity.x);
        float xVelocity = _curVelocity.x;

        //x方向
        //区分是在地面还是在空中，x方向使用不同的加减速和最大速度
        float acceleration = stateManager.isInAir ? airAcceleration : groundAcceleration ;
        float deceleration = stateManager.isInAir ? airDeceleration : groundDeceleration;
        float maxSpeed = stateManager.isInAir ? airSpeed : groundSpeed;
        //静止状态，按照输入方向加速
        if (isStopped(xVelocity) && xInput != 0) {
            xVelocity = xInput * acceleration * Time.deltaTime;
        } else if (!isStopped(xVelocity)) {
            //非静止状态
            //水平输入与速度同方向加速，其他情况都是减速，包括反方向减速, 无输入减速
            if (xInput != 0 && inputSign == xVelocitySign) {
                //加速
                xVelocity += xVelocitySign * acceleration * Time.deltaTime;
            } else {
                //减速，但是不能把速度方向翻转了，减速的加速度太大
                xVelocity += -1 * xVelocitySign * deceleration * Time.deltaTime;
                xVelocity = (xVelocity * xVelocitySign < 0) ? 0 : xVelocity;
            }
        }
        //小于阈值直接停下
        xVelocity = isStopped(xVelocity) ? 0 : xVelocity;
        //不能超出最大速度
        _curVelocity.x = Mathf.Clamp(xVelocity, -1 * maxSpeed, maxSpeed);

        //y方向
        //跳跃检测在update中，因为buttondown检测是帧率相关
        //IsJumpStart为true时，读取一次后会重置
        if (stateManager.IsJumpStart) {
            _curVelocity.y = jumpStartSpeed;
        }
        bool jumpHold = Input.GetButton("Jump");
        float modifier;
        if (jumpHold && stateManager.canExtraJump()) {
            modifier = 0;
        } else {
            modifier = _curVelocity.y > 0 ? gravityModifier : fallGravityModifier;
        }
        //计算重力影响。使用的Physics2D重力设置
        _curVelocity += modifier * Physics2D.gravity * Time.deltaTime;
    }

    bool isStopped(float speed) {
        return Mathf.Abs(speed) <= speedThreshold;
    }

    //监听状态改变
    void Statechanged(UserState fromState, UserState toState) {
        Debug.Log(fromState.ToString() + " => " + toState.ToString());
    }

}
