using UnityEngine;
using Utilities.AttTypeDefine;

public class CollisionListener : MonoBehaviour
{
    public static CollisionEvent onCollisionEnter = new CollisionEvent();

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter.Invoke(gameObject, collision);
    }
}