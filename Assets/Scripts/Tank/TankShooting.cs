using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TankShooting : MonoBehaviour
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

    private PhotonView pv;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; //차징 속도 계산

        m_AimSlider.gameObject.SetActive(pv.IsMine);
    }
    

    private void Update()
    {
        if (m_IsAI)
        {
            return;
        }

        if (!pv.IsMine) return;

        // Track the current state of the fire button and make decisions based on the current launch force.
        // 에임 슬라이더 디폴트 값으로 설정
        m_AimSlider.value = m_MinLaunchForce;

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // ... use the max force and launch the shell.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
            pv.RPC("maxTimeFire", RpcTarget.Others);
        }
        // Otherwise, fire 버튼 눌렸을때(눌린상태)(처음누른상태 진입 체크)
        else if (Input.GetMouseButtonDown(0))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // Change the clip to the charging clip and start it playing.
            m_ShootingAudio.clip = m_ChargingClip; //차징 클립 재생
            m_ShootingAudio.Play();
            pv.RPC("mouseButtonDownFire", RpcTarget.Others);
        }
        // Otherwise, 버튼 눌린(홀드) 상태에서 발사 안됬을 경우
        else if (Input.GetMouseButton(0) && !m_Fired)
        {
            //Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
            pv.RPC("mouseButtonFire", RpcTarget.Others);
        }
        // Otherwise, 버튼 릴리스 상태에서 발사 안됬을 경우
        else if (Input.GetMouseButtonUp(0) && !m_Fired)
        {
            // ... launch the shell.
            Fire();
            pv.RPC("mouseButtonUpFire", RpcTarget.Others);
        }
    }

    [PunRPC]
    void maxTimeFire()
    {
        // at max charge, not yet fired
        m_CurrentLaunchForce = m_MaxLaunchForce;
        Fire();
    }

    [PunRPC]
    void mouseButtonDownFire()
    {
        // have we pressed fire for the first time?
        m_Fired = false;
        m_CurrentLaunchForce = m_MinLaunchForce;

        m_ShootingAudio.clip = m_ChargingClip;
        m_ShootingAudio.Play();
    }

    [PunRPC]
    void mouseButtonFire()
    {
        // Holding the fire button, not yet fired
        m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

        m_AimSlider.value = m_CurrentLaunchForce;
    }

    [PunRPC]
    void mouseButtonUpFire()
    {
        Fire();
    }

    public void Fire()
    {
        // Instantiate and launch the shell.
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        if (GetComponent<NetworkPlayer>())
            shellInstance.GetComponent<ShellExplosion>().ownerIsPlayer = true;
        else
            shellInstance.GetComponent<ShellExplosion>().ownerIsPlayer = false;

        //AI 라면 평균값으로 shell 발사
        if (m_IsAI)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce / 2.0f;
        }

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}