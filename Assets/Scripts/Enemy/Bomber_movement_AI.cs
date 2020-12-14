using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;


public class Bomber_movement_AI : FSM
{
    public GameObject Bomb;
    public BombExplosion TankExplosion;
    public TankHealth tankHealth;
    public GameObject Patrol_icon;
    public GameObject Attack_icon;
    public GameObject Chase_icon;
    //NPC 사망판정
    private bool isDead = false;
    private float elapsedTime = 0.0f;

    private GameObject player = null;
    private NavMeshAgent navMeshAgent;

    private Animator BombAnimator;

    private Renderer iconColorP; // 패트롤 아이콘 컬러 렌더러
    private Renderer iconColorA; // 어택 아이콘 컬러 렌더러
    private Renderer iconColorC; // 추적 아이콘 컬러 렌더러

    private int count = 0;

    public enum FSMState
    {
        None,
        Chase,
        Attack,
        Dead,
    }

    //현재 NPC state
    public FSMState m_CurState;



    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        BombAnimator = Bomb.GetComponent<Animator>();

        iconColorP = Patrol_icon.GetComponent<Renderer>(); // 눈모양 아이콘의 렌더러 연결
        iconColorA = Attack_icon.GetComponent<Renderer>(); // 눈모양 아이콘의 렌더러 연결
        iconColorC = Chase_icon.GetComponent<Renderer>(); // 눈모양 아이콘의 렌더러 연결

    }
    //Update each frame
    protected override void FSMUpdate()
    {
        switch (m_CurState)
        {
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //시간 업데이트
        elapsedTime += Time.deltaTime;

        //체력이 없으면 dead 상태로 
        if (this.tankHealth.getCurHealth() <= 0)
            m_CurState = FSMState.Dead;
    }


    private void UpdateDeadState()
    {
        if (!isDead)
        {
            Debug.Log("Dead");
        }
    }

    private void UpdateAttackState()
    {

        iconColorP.material.color = new Color(1, 1, 1, 0);
        iconColorA.material.color = new Color(1, 1, 1, 1);
        iconColorC.material.color = new Color(1, 1, 1, 0);

        BombAnimator.SetBool("isDetonate", true);
        


        Collider[] players = Physics.OverlapSphere(transform.position, 5.0f, LayerMask.GetMask("Players"));
        if(players.Length > 0)
            player = players[0].gameObject;
        else
        {
            navMeshAgent.enabled = true;
            m_CurState = FSMState.Chase;
            return;
        }

        Vector3 _direction = (player.transform.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 3);
        if (count == 0)
        {
            this.TankExplosion.inite();
            count++;
        }
            
           
        
    }

    private void UpdateChaseState()
    {
        iconColorP.material.color = new Color(1, 1, 1, 0); // 알파값 0 
        iconColorA.material.color = new Color(1, 1, 1, 0); // 알파값 0 
        iconColorC.material.color = new Color(1, 1, 1, 1);

        Collider[] players = Physics.OverlapSphere(transform.position, 3.0f, LayerMask.GetMask("Players"));
        
        if (players.Length > 0)
        {
            m_CurState = FSMState.Attack;
            player = players[0].gameObject;
            navMeshAgent.enabled = false;
            return;
        }
        else 
        {
            m_CurState = FSMState.Chase;
        }
       

        player = GameObject.FindGameObjectWithTag("usr");

        if(player)
            navMeshAgent.destination = player.GetComponent<Transform>().position;
    }

    protected bool IsInCurrentRange(Vector3 pos) //범위 내에 있는지 계산
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 5 && zPos <= 5)
            return true;

        return false;
    }


}
