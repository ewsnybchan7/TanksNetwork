using UnityEngine;
using Photon.Pun;

public class ShellExplosion : MonoBehaviour
{
    private LayerMask m_TankMask; //탱크가 존재하는 레이어를 마스킹
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;          //최대 데미지        
    public float m_ExplosionForce = 700f;            
    public float m_MaxLifeTime = 2f;         //포탄 유지 시간         
    public float m_ExplosionRadius = 5f;    //포탄 폭발 반경          

    public bool ownerIsPlayer;
    public int shotPvid;

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime); //유지 시간 이휴에 object를 destroy
    }

    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        if (ownerIsPlayer)
        {
            m_TankMask = LayerMask.GetMask("AI");
        }
        else
        {
            m_TankMask = LayerMask.GetMask("Players");
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask); //폭발 반경내에 있는 tankmask있는 colㅣider 를 구형범위로 수집

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>(); //반경 안의 collider의 rigidbody를 가져옴

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            //float damage = CalculateDamage(targetRigidbody.position); // target rigidbody와의 거리에 따라 데미지 계산

            float damage = 10.0f;
            // Deal this damage to the tank.

            targetHealth.TakeDamage(damage);
        }

        // Unparent the particles from the shell.
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system.
        m_ExplosionParticles.Play();

        // Play the explosion sound effect.
        m_ExplosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;

        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

        // Destroy the shell.
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}