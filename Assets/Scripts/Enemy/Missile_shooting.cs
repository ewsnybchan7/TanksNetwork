using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Missile_shooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
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

    [PunRPC]
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
            // 타겟과 거리별로 lanch force 조절하는 구문 필요
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