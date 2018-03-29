using UnityEngine;
using System;
using System.Collections.Generic;


public enum UserState {
    Start, Grounded, Rising, Falling, Dashing
};

public class PC2dStateManager {

    public UserState lastState, currentState;
    public float stateEnterTime, lastStateDuration = 0;
    public event Action<UserState, UserState> OnStateChange;

    public bool isInAir {
        get {
            return currentState == UserState.Falling || currentState == UserState.Rising;
        }
    }
    public int airJumpMax = 1;//空中跳跃最大次数
    public int airJumpCnt = 0;//空中跳跃已使用次数

    private bool isJumpStart = false;//是否开始跳跃
    public bool IsJumpStart {
        get {
            //设置为true后，被访问一次重置
            if (isJumpStart) {
                isJumpStart = false;
                return true;
            }
            return false;
        }
    }
    public float extraJumpAllowTime = 0;//本次跳跃的额外跳跃截止时间 
  

    private bool isDashStart = false;//是否开始dash
    public bool IsDashStart {
        get {
            //设置为true后，被访问一次重置
            if (isDashStart) {
                isDashStart = false;
                return true;
            }
            return false;
        }
    }
    public float dashNextAvailiableTime = 0;//下次dash可以使用的时间
    public float dashStopTime = 0;//本次dash 停止时间

    public PC2dStateManager() {
        lastState = currentState = UserState.Start;
        stateEnterTime = Time.time;
    }

    // 修改当前状态
    public void ChangeStateTo(UserState nextState) {
        if (nextState == currentState) {
            return;
        }
        lastState = currentState;
        currentState = nextState;
        lastStateDuration = Time.time - stateEnterTime;
        stateEnterTime = Time.time;
        OnStateChange?.Invoke(lastState, currentState);
    }

    //判断当前状态是否允许跳跃
    public bool canJump() {
        //地面跳跃
        if(currentState == UserState.Grounded) {
            return true;
        }
        //空中跳跃
        if(isInAir && airJumpCnt < airJumpMax){
            return true;
        }
        return false;
    }

    //根据当前时间判断是否允许继续额外跳跃
    public bool canExtraJump() {
        return (extraJumpAllowTime != 0) && (Time.time <= extraJumpAllowTime);
    }

    //记录起跳，记录允许额外跳跃的截止时间
    public void recordJumpStart(float extraJumpTime) {
        isJumpStart = true;
        extraJumpAllowTime = extraJumpTime;
        if(isInAir){
            airJumpCnt++;
        }
        ChangeStateTo(UserState.Rising);
    }
    //重置跳跃次数等信息
    public void resetJump() {
        isJumpStart = false;
        airJumpCnt = 0; 
        extraJumpAllowTime = 0;
    }

    public bool isDashing {
        get {
            return currentState == UserState.Dashing;
        }
    }
    //从时间上判断dash是否该停止
    public bool shouldDashStop {
        get {
            return Time.time >= dashStopTime;
        }
    }
    //dash已经冷却，且当前不处于dash状态时，返回true
    public bool canDash(){
        return (!isDashing) &&　 (Time.time >= dashNextAvailiableTime);
    }
    //记录这次dash的结束时间，以及下次可用时间
    public void DashStart(float nextAvailiableTime, float stopTIme){
        isDashStart = true;
        dashNextAvailiableTime = nextAvailiableTime;
        dashStopTime = stopTIme;
        ChangeStateTo(UserState.Dashing);
    }
    
    public void DashStop(){
        ChangeStateTo(UserState.Falling);
        //changeState
    }
}
