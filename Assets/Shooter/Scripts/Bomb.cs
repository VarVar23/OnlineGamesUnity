using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    [Networked] private TickTimer _life { get; set; }
    [SerializeField] private float _lifeTime = 5;
    [SerializeField] private float _bodyRadius = 0.3f;
    [SerializeField] private float _areaRadius = 5;
    [SerializeField] private float _areaDamage = 50;
    [SerializeField] private float _areaImpulse = 20;
    private Rigidbody _rigidbody;

    public void Init(Vector3 force)
    {
        _life = TickTimer.CreateFromSeconds(Runner, _lifeTime);
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = force;
    }

    public override void FixedUpdateNetwork()
    {
        if(HasStateAuthority == false) return;

        if(IsLifeTimePassed() || HasHit())
        {
            Detonate();
            Runner.Despawn(Object);
        }
    }

    private bool IsLifeTimePassed()
    {
        return _life.Expired(Runner);
    }

    private bool HasHit()
    {
        var hitBoxManager = Runner.LagCompensation;

        var hasHit = hitBoxManager.Raycast(
            origin: transform.position,
            direction: _rigidbody.velocity.normalized,
            length: _rigidbody.velocity.magnitude * Runner.DeltaTime + _bodyRadius,
            player: Object.InputAuthority,
            hit: out var _,
            options: HitOptions.IncludePhysX
        );

        return hasHit;
    }

    private void Detonate()
    {
        var (count, areaHits) = OverlapSphere();

        for(int i = 0; i < count; i++)
        {
            GameObject otherGameObject = areaHits[i].GameObject;

            if (otherGameObject)
            {
                TryDealDamage(otherGameObject);
                TryApplyImpulse(otherGameObject);
            }
        }
    }

    private (int count, List<LagCompensatedHit> areaHits) OverlapSphere()
    {
        var areaHits = new List<LagCompensatedHit>();

        int count = Runner.LagCompensation.OverlapSphere(
            origin: transform.position,
            radius: _areaRadius,
            player: Object.InputAuthority,
            hits: areaHits,
            layerMask: -1,
            options: HitOptions.IncludePhysX
        );

        return (count, areaHits);
    }

    private void TryDealDamage(GameObject otherGameObject)
    {
        if(otherGameObject.TryGetComponent<Health>(out var health))
        {
            var distance = Vector3.Distance(transform.position, otherGameObject.transform.position);

            if(distance < _areaRadius)
            {
                var damage = _areaDamage * ((_areaRadius - distance) / _areaRadius);
                health.ApplyDamage(damage);
            }
        }
    }

    private void TryApplyImpulse(GameObject otherGameObject)
    {
        if (otherGameObject.TryGetComponent<NetworkCharacterControllerPrototype>(out var characterController))
        {
            var distance = Vector3.Distance(transform.position, otherGameObject.transform.position);

            if (distance < _areaRadius)
            {
                var direction = otherGameObject.transform.position - transform.position;
                var horizontalDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
                var impulse = _areaImpulse * ((_areaRadius - distance) / _areaRadius);
                characterController.Velocity += horizontalDirection * impulse;
                characterController.Move(Vector3.zero);
            }
        }
    }
}