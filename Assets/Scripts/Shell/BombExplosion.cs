using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombExplosion : MonoBehaviour
{
    public GameObject bomb;
    public LayerMask m_TankMask; //탱크가 존재하는 레이어를 마스킹
    public GameObject explosionPrefab;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 100f;          //최대 데미지        
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 3.9f;         //포탄 유지 시간         
    public float m_ExplosionRadius = 15f;    //포탄 폭발 반경    
                                             // Start is called before the first frame update
    [PunRPC]
    public void inite()
    {
        //Destroy(gameObject, m_MaxLifeTime); //유지 시간 이휴에 object를 destroy
        Invoke("Detonate", 3.5f);
    }

    // Update is called once per frame

    public void Detonate()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Vector3 explotionPosition = bomb.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explotionPosition, m_ExplosionRadius, m_TankMask);
        foreach (Collider hit in colliders)
        {
            Rigidbody targetRigidbody = hit.GetComponent<Rigidbody>(); //폭발 반경 안의 collider의 rigidbody를 가져옴
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            //float damage = CalculateDamage(targetRigidbody.position); // target rigidbody와의 거리에 따라 데미지 계산

            float damage = 50.0f;
            // Deal this damage to the tank.
            targetHealth.TakeDamage(damage);

        }

        // Play the explosion sound effect.
        //m_ExplosionAudio.Play();
        //Destroy particle
        
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
