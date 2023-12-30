using UnityEngine;
using Fusion;

public class Item : NetworkBehaviour
{
    [SerializeField] private GameObject _body;

    [Networked(OnChanged = nameof(OnItemPicked))]
    public NetworkBool IsLifted { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority == false) return;

        IsLifted = false;
    }

    public void PickUp(NetworkObject networkObject)
    {
        if(IsLifted == false)
        {
            IsLifted = true;
        }
    }

    public void OnPicked()
    {
        _body.SetActive(!IsLifted);
    }

    public static void OnItemPicked(Changed<Item> changed)
    {
        if(changed.Behaviour.HasInputAuthority)
        {

        }

        changed.Behaviour.OnPicked();
    }
}