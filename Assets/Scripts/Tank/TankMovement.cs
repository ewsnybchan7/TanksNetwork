using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         

    // Awake
    // 해당 스크립트가 등록된 오브젝트(스크립트)가 최초로 활성화될 때 불리는 함수(한번만)
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // OnEable
    // 활성화될 때마다 호출되는 함수
    // isKinematic: 움직임 여부
    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    // OnDisable
    // 비활성화될 때마다 호출되는 함수
    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }

    // Start
    // Awake()와 마찬가지로 최초로 활성화될 때 한번만 불리는 함수(Awake보다 늦게 호출됨)
    private void Start()
    {
        m_MovementAxisName = "VerticalKey";
        m_TurnAxisName = "HorizontalKey";

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    
    // Update
    // 활성화 상태일 때 한 프레임에 한번씩 호출되는 함수
    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    // FixedUpdate
    // Update()와 마찬가지로 활성화 상태일 때 지속적으로 호출되지만, 1초에 고정된 횟수만큼 호출됌
    // 설정하지 않았다면, 기본 물리시간인 0.02초에 한번씩 호출됌
    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}