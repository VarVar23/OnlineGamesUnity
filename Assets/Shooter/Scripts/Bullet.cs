using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _damage = 10;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _maxDistance = 15;
    private Vector3 _startSpawnPoint;

    public void Init()
    {
        _startSpawnPoint = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        if(!IsMaxDistancePassed() && !HasHit())
        {
            transform.position += _speed * transform.forward * Runner.DeltaTime;
        }
        else
        {
            Runner.Despawn(Object);
        }
    }

    private bool IsMaxDistancePassed()
    {
        return Vector3.Distance(transform.position, _startSpawnPoint) > _maxDistance;
    }

    private bool HasHit()
    {
        var (hasHit, hitResult) = LagCompensatedRaycast();
        if (hasHit) TryDealDamage(hitResult);
        return hasHit;
    }

    private (bool hasHit, LagCompensatedHit hit) LagCompensatedRaycast()
    {
        var hasHit = Runner.LagCompensation.Raycast(
            origin: transform.position,
            direction: transform.forward,
            length: _speed * Runner.DeltaTime,
            player: Object.InputAuthority,
            hit: out var hitResult,
            layerMask: -1,
            options: HitOptions.IncludePhysX
        );

        return (hasHit, hitResult);
    }

    private void TryDealDamage(LagCompensatedHit hit)
    {
        if(hit.GameObject.TryGetComponent<Health>(out var health))
        {
            health.ApplyDamage(_damage);
        }
    }
}