using UnityEngine;
using UnityEngine.Events;

namespace Utilities.AttTypeDefine
{
    #region 枚举

    public enum AttackType
    {
        eCloseRange,
        eLongRange,
    }

    #endregion

    #region 结构体

    [System.Serializable]
    public struct EnemySpawnArea
    {
        public Transform pos;
        public Vector3 size;
    }

    [System.Serializable]
    public struct EnemyColorMapping
    {
        public AttackType type;
        public Color color;
    }

    #endregion

    #region 事件

    public class CollisionEvent : UnityEvent<GameObject, Collision>{}

    #endregion
}
