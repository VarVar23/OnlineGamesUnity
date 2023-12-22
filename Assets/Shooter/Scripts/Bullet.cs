using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _lifeTime = 5;
    [SerializeField] private float _speed = 5;
    [Networked] private TickTimer Life { get; set; }

    public void Init()
    {
        Life = TickTimer.CreateFromSeconds(Runner, _lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if(Life.Expired(Runner) == false)
        {
            transform.position += _speed * transform.forward * Runner.DeltaTime;
        }
        else
        {
            Runner.Despawn(Object);
        }
    }
}
