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
    private bool isJumpStart = false;
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
    public float extraJumpAllowTime = 0;

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
        if(currentState == UserState.Grounded) {
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
        ChangeStateTo(UserState.Rising);
    }
    public void resetJump() {
        isJumpStart = false;
        extraJumpAllowTime = 0;
    }

}
