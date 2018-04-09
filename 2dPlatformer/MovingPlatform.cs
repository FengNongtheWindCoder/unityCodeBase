using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 这个脚本需要添加到一个platform上，在platform外层加一个parent，同时parent下加一些waypoint gameobject
/// </summary>
public class MovingPlatform : MonoBehaviour {
    protected Rigidbody2D rb2d;
    public Transform[] waypoints;//waypoint数组，platform会按顺序在这些点间移动
    public float Speed = 2;//移动速度
    public int curDestIndex = 0;
    public Vector2 curTargetPos;
    public float minMoveDistance = 0.05f;
    public float pauseTime = 1;//在到达某个点后停留几秒
    private int waypointCnt = 0;
    private bool inPauseState = false;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        waypointCnt = waypoints.Length;
        curDestIndex = 0;
        curTargetPos = waypoints[curDestIndex].position;
    }

    private void FixedUpdate() {
        if(inPauseState || waypointCnt < 2){return;}
        Vector2 newPos;
        if(minMoveDistance >= Vector2.Distance(rb2d.position, curTargetPos)){
            //到达某个点
            newPos = curTargetPos;
            curDestIndex = getNextDestIndex(curDestIndex);
            curTargetPos = waypoints[curDestIndex].position;
            inPauseState = true;
            StartCoroutine(waitsometime(pauseTime));
        } else {
            newPos = Vector2.MoveTowards(rb2d.position, curTargetPos, Speed * Time.deltaTime);
        }
        rb2d.MovePosition(newPos);
    }

    IEnumerator waitsometime(float time){
        yield return new WaitForSeconds(time);
        inPauseState = false;
    }

    private int getNextDestIndex(int curIndex) {
        return (curIndex + 1) % waypointCnt;
    }

    private int getPreviousDestIndex(int curIndex) {
        //负数取余不太确定
        if (curIndex == 0) { return waypointCnt - 1; }
        return (curIndex - 1) % waypointCnt;
    }
}
