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
}
