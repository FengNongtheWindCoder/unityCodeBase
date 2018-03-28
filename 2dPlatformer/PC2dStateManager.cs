using UnityEngine;
using System;
using System.Collections.Generic;


public enum UserState {
    Start, Grounded, Rising, Falling
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

    public void resetJump() {
        isJumpStart = false;
        airJumpCnt = 0; 
        extraJumpAllowTime = 0;
    }

}
