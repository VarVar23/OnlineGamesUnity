using UnityEngine;
using Fusion;

public class Health : NetworkBehaviour
{
    [SerializeField] private float _maximumHealth = 100;

    [Networked(OnChanged = nameof(OnHealthChanged))]
    public float CurrentHealth { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority == false) return; //Если клиент является хостом

        CurrentHealth = _maximumHealth;
    }

    public void ApplyDamage(float damage)
    {
        if(HasStateAuthority == false) return;

        var absDamage = Mathf.Abs(damage);

        if (CurrentHealth > absDamage)
        {
            CurrentHealth -= absDamage;
        }
        else
        {
            CurrentHealth = 0;
        }
    }

    public static void OnHealthChanged(Changed<Health> changed)
    {
        if (changed.Behaviour.HasInputAuthority)
            Debug.Log(changed.Behaviour.CurrentHealth);
    }
}