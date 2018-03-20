using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PC2dController : MonoBehaviour {

    public Vector2 _curVelocity = Vector2.zero;//计算出的当前速度

    //重力系数
    public float gravityModifier = 3;
    public float fallGravityModifier = 5;//下落时重力加大，下落变快
    //地面运动参数
    public float groundSpeed = 10f;//最大跑动速度
    public float timeToGroundSpeed = 0.5f;//达到最大速度时间
    public float groundStopDistance = 0.5f;//停止移动需要的距离
    public float speedThreshold = 0.1f;//最小移动速度，小于此值速度归零
    private float groundAcceleration;//加速 减速是计算出的
    private float groundDeceleration;//加速 减速是计算出的

    //跳跃相关
    public float jumpHeight = 5f;//跳跃高度
    private float jumpSpeed;//起跳速度由重力和跳跃高度决定，

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
        //起跳速度计算 v=sqrt(2gh)
        jumpSpeed = Mathf.Sqrt(2 * -1 * gravityModifier * Physics2D.gravity.y * jumpHeight);
        
        userInput.Reset();
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;

    }

    // Update is called once per frame
    void Update() {
        //检测输入
        userInput.xInput = Input.GetAxisRaw("Horizontal");
        userInput.jumpPressed = Input.GetButtonDown("Jump");//ButtonDown只在下一帧重置
        //计算当前速度
        ComputeVelocity();
    }

    void FixedUpdate() {
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
        //不能超出最大速度
        _curVelocity.x = Mathf.Clamp(xVelocity, -1 * groundSpeed, groundSpeed);

        //y方向
        //跳跃
        if (userInput.jumpPressed && stateManager.canJump()) {
            _curVelocity.y = jumpSpeed;
            stateManager.ChangeStateTo(UserState.Rising);
        }
        //计算重力影响。使用的Physics2D重力设置
        float modifier = _curVelocity.y > 0 ? gravityModifier : fallGravityModifier;
        _curVelocity += modifier * Physics2D.gravity * Time.deltaTime;
    }

    //监听状态改变
    void Statechanged(UserState fromState, UserState toState) {
        Debug.Log(fromState.ToString() + " => " + toState.ToString());
    }

}
