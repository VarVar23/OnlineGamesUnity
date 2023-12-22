using Fusion;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    [SerializeField] private float _lifeTime = 5;
    [Networked] private TickTimer _life { get; set; }


    public void Init(float force)
    {
        _life = TickTimer.CreateFromSeconds(Runner, _lifeTime);
        GetComponent<Rigidbody>().velocity = force * transform.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if(_life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
