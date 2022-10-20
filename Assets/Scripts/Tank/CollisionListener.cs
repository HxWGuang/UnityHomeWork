using UnityEngine;
using Utilities.AttTypeDefine;

public class CollisionListener : MonoBehaviour
{
    public float m_MaxCollideForce = 50f;
    public float m_MaxDamage = 10;

    public static CollisionEvent onCollisionEnter = new CollisionEvent();

    // public float CalculateColldieDamage(float force)
    // {
    //     force = Mathf.Clamp(force, 0, m_MaxCollideForce);
    //     var rel = force / m_MaxCollideForce;
    //     var damage = m_MaxDamage * rel;
    //
    //     return damage;
    // }

    private void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) return;
        //
        // var force = (collision.impulse / Time.fixedDeltaTime).magnitude;
        //
        // var damage = CalculateColldieDamage(force);
        //
        // GetComponent<TankHealth>().TakeDamage(damage);

        onCollisionEnter.Invoke(gameObject, collision);
    }
}
