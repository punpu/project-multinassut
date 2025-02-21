using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Serializing.Helping;
using FishNet.Transporting;
using UnityEngine.InputSystem;

public class MorsoBehavior : NetworkBehaviour
{
    private readonly SyncVar<float> _opacity = new SyncVar<float>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

    private void Awake()
    {
        _opacity.OnChange += on_opacity;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        SetOpacity(0f); // Use this for Morso
    }

    [ServerRpc(RunLocally = true)]
    private void SetOpacityServerRpc(float value)
    {
        float prevValue = _opacity.Value;
        _opacity.Value = value;
        on_opacity(prevValue, value, true); // Manually invoke the callback
    }

    // SyncVar change event is only triggered when the value is changed on the server
    // and then synchronized to the clients.
    private void on_opacity(float prev, float next, bool asServer)
    {
        var renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Debug.Log($"Opacity changed from {prev} to {next}");
            var color = renderer.material.color;
            color.a = next;
            renderer.material.color = color;
            renderer.enabled = next > 0; // Disable renderer if opacity is 0
        }
        else
        {
            Debug.LogError("No renderer found");
        }
    }

    public void SetOpacity(float value)
    {
        if (IsOwner)
        {
            SetOpacityServerRpc(value);
        }
    }

    public void SetFullOpacity()
    {
        SetOpacity(1f);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner || context.phase != InputActionPhase.Started) { return; }
        SetOpacity(_opacity.Value == 0f ? 1f : 0f);
    }
}
