using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class AmmoPickup : NetworkBehaviour
{
    public float rotationSpeed = 20f;

    public void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPickUp()
    {
        var parent = transform.parent.gameObject;
        if (!parent)
        {
            Debug.LogWarning("AmmoPickup parent not found!");
            return;
        }
        parent.GetComponent<AmmoSpawner>().DespawnAmmoPickup(gameObject);
    }
}
