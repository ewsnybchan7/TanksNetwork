using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : TankMovement
{
    // 플레이어 transform
    protected Transform m_PlayerTransform;

    // NPC tank 다음 목적지
    protected Vector3 destPos;

    // Patrol 할 point 목적지 리스트 
    protected GameObject[] pointList;


    protected virtual void Initialize()
    {
    }

    protected virtual void FSMUpdate()
    {

    }

    protected virtual void FSMFixedUpdate()
    {

    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}
