using UnityEngine;
using Fusion;

public class Trap : NetworkBehaviour
{
    private Collider[] _colliders = new Collider[10];

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        var count = Runner.GetPhysicsScene().OverlapBox(
            center: transform.position,
            halfExtents: Vector3.one,
            results: _colliders,
            orientation: Quaternion.identity,
            layerMask: -1
        );

        for(int i = 0; i < count; i++ )
        {
            if(_colliders[i].TryGetComponent<Health>(out var health))
            {
                health.ApplyDamage(0.1f);
            }
        }
    }
}