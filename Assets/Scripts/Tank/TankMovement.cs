using UnityEditorInternal;
using UnityEngine;
using Photon.Pun;

public class TankMovement : MonoBehaviour, IPunObservable
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;
    public bool m_IsAI;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;
    private Transform m_Transform;
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;

    private PhotonView pv;

    // Smooth moving variable
    private Vector3 curPos = Vector3.zero;
    private Quaternion curRot = Quaternion.identity;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();

        curPos = m_Transform.position;
        curRot = m_Transform.rotation;
    }

    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false; //사용자 컨트롤 상태 위해 (조작)물리 적용받음
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true; //시체상태 (조작)물리 적용 안받음
    }


    private void Start()
    {
        m_MovementAxisName = "VerticalKey";
        m_TurnAxisName = "HorizontalKey";

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        if (!m_IsAI) //AI 가 아니라면 플레이어 조작
        {
            // Store the player's input and make sure the audio for the engine is playing.
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
        }

        if (!pv.IsMine)
        {
            transform.position = Vector3.Lerp(m_Transform.position, curPos, Time.deltaTime * 3.0f);
            transform.rotation = Quaternion.Slerp(m_Transform.rotation, curRot, Time.deltaTime * 3.0f);
        }

        //EngineAudio();
        pv.RPC("EngineAudio", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) // 정지상태
        {
            if (m_MovementAudio.clip == m_EngineDriving) //움직이는 엔진 사운드 출력중이면
            {
                m_MovementAudio.clip = m_EngineIdling; // 아이들엔진 사운드 출력으로 바꿈
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange); // ori =1 range 0.8 to 1.2
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling) //움직이는 엔진 사운드 출력중이면
            {
                m_MovementAudio.clip = m_EngineDriving; // 아이들엔진 사운드 출력으로 바꿈
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange); // ori =1 range 0.8 to 1.2
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate() //물리 엔진이 업데이트 될때마다 호출
    {
        if (pv.IsMine)
        {
            // Move and turn the tank.
            Move();
            Turn();
        }
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; //speed 12 unit per every sec

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement); //curr pos + mov pos
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f,turn,0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_Transform.position);
            stream.SendNext(m_Transform.rotation);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}