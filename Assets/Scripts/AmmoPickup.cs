using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class AmmoPickup : NetworkBehaviour
{
    public float rotationSpeed = 20f;
    public int respawnTimeInSeconds = 2;
    private readonly SyncVar<bool> _isEnabled = new SyncVar<bool>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

    private void Awake()
    {
        _isEnabled.OnChange += OnIsEnabled;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        EnableAmmoPickup();
    }

    // SyncVar change event is only triggered when the value is changed on the server
    // and then synchronized to the clients.
    private void OnIsEnabled(bool prev, bool next, bool asServer)
    {
        var boxCollider = gameObject.GetComponent<BoxCollider>();
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!boxCollider || !meshRenderer)
        {
            Debug.LogWarning("AmmoPickup: BoxCollider or MeshRenderer missing!");
            return;
        }
        gameObject.GetComponent<BoxCollider>().enabled = next;
        gameObject.GetComponent<MeshRenderer>().enabled = next;
    }

    [ServerRpc(RunLocally = true, RequireOwnership = false)]
    private void SetIsEnabledServerRpc(bool value)
    {
        var prevValue = _isEnabled.Value;
        _isEnabled.Value = value;
    }

    public void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableAmmoPickup()
    {
        SetIsEnabledServerRpc(false);
        Invoke("EnableAmmoPickup", respawnTimeInSeconds);
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableAmmoPickup()
    {
        SetIsEnabledServerRpc(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPickUp()
    {
        DisableAmmoPickup();
    }
}
