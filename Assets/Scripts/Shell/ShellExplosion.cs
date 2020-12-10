using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        // Physics.OverlapSphere: 현재 본인의 위치에서 반지름 값 만큼의 범위에 있는 모든 Collider를 얻을 수 있는 함수
        // layerMask에 의해 특정 layer만을 얻을 수 찾을 수 있음
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody) continue;

            // Rigidbody.AddExplosionForce: 객체의 Rigidbody를 이용하여 폭발력을 적용 받음
            // float: 폭발력 / Vector3: 폭발 위치(원점) / float: 폭발 원 반경
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth) continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position; // 원점에서 탱크까지의 벡터

        float explosionDistance = explosionToTarget.magnitude; // 벡터의 크기

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; // 상대 길이(비율)

        float damage = relativeDistance * m_MaxDamage; // 상대 길이에 따른 데미지

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}