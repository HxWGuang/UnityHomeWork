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

    /**
     * 1 给每个范围内的坦克一个爆炸力
     * 2 计算范围内每个坦克受到的伤害
     * 3 播放爆炸特效
     * 4 播放爆炸音效
     * 5 销毁特效（播放完后销毁）
     * 6 销毁炮弹（立即）
     */
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            var targetRigbody = colliders[i].GetComponent<Rigidbody>();
            if (null == targetRigbody)
                continue;

            targetRigbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            var targetHealth = colliders[i].GetComponent<TankHealth>();
            if (null == targetHealth)
                continue;

            float damage = CalculateDamage(targetRigbody.position);
            targetHealth.TakeDamage(damage);
        }

        // todo:验证一下如果没有这句话会有什么区别
        // 没有这一行，爆炸特效就无了
        // 原因分析：
        // 炮弹碰撞后会在第60行立即销毁，特效会在特效播放完后销毁（销毁时间不一样）
        // 而特效对象挂在炮弹对象下面，炮弹被销毁时特效也会跟着被立即销毁
        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        var explosionDis = (targetPosition - transform.position).magnitude;

        var relativeDis = (m_ExplosionRadius - explosionDis) / m_ExplosionRadius;

        var damage = m_MaxDamage * relativeDis;

        damage = Mathf.Max(0, damage);

        return damage;
    }
}