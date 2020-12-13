using UnityEngine;
using UnityEngine.UI;

public class Missile_shooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 3f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f; //min 차지에서 max 차지까지 걸리는 시간
    public bool m_IsAI;


    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber; //몇번째 플레이어 의 fire 버튼인지 받음

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; //차징 속도 계산
    }


    private void Update()
    {
        if (m_IsAI)
        {
            return;
        }
        // Track the current state of the fire button and make decisions based on the current launch force.
        // 에임 슬라이더 디폴트 값으로 설정
        m_AimSlider.value = m_MinLaunchForce;

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // ... use the max force and launch the shell.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // Otherwise, fire 버튼 눌렸을때(눌린상태)(처음누른상태 진입 체크)
        else if (Input.GetButtonDown(m_FireButton))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // Change the clip to the charging clip and start it playing.
            m_ShootingAudio.clip = m_ChargingClip; //차징 클립 재생
            m_ShootingAudio.Play();
        }
        // Otherwise, 버튼 눌린(홀드) 상태에서 발사 안됬을 경우
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // Otherwise, 버튼 릴리스 상태에서 발사 안됬을 경우
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // ... launch the shell.
            Fire();
        }
    }


    public void Fire()
    {
        // Instantiate and launch the shell.
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;
        
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position,m_FireTransform.rotation) as Rigidbody;

        
        //AI 라면 평균값으로 shell 발사
        if (m_IsAI)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce / 3.0f;
        }

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        //shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ;



        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;

    }
}