using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TankHealth : MonoBehaviour, IPunObservable
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public bool IsAI;

    public float getCurHealth() { return m_CurrentHealth;}

    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    public float m_CurrentHealth;  
    private bool m_Dead;            

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    
    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        //Hashtable playerProperty = GetComponent<PhotonView>().Owner.CustomProperties;

        m_CurrentHealth -= amount;

        //SetHealthUI();
        GetComponent<PhotonView>().RPC("SetHealthUI", RpcTarget.AllBuffered);

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            GetComponent<PhotonView>().RPC("OnDeath", RpcTarget.AllBuffered);
            //OnDeath();
        }
    }

    [PunRPC]
    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    [PunRPC]
    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();



        gameObject.SetActive(false); //tank off

        // if ai 인지
        if(IsAI)
            GetComponent<PhotonView>().RPC("aiDeath", RpcTarget.AllBuffered);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(m_CurrentHealth);
        }
        else
        {
            m_CurrentHealth = (float)stream.ReceiveNext();

        }
    }

    [PunRPC]
    private void aiDeath()
    {
        FindObjectOfType<GameManager>().increaseKill();
    }
}