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
        lastState = currentState;
        currentState = nextState;
        lastStateDuration = Time.time - stateEnterTime;
        stateEnterTime = Time.time;
        OnStateChange?.Invoke(lastState, currentState);
    }

}
