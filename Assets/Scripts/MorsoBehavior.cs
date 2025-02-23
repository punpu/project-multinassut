using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

// Note: Morso is a Player with this script and no Lantern!
public class MorsoBehavior : NetworkBehaviour
{
    private const float MIN_OPACITY = 0f;
    private const float MAX_OPACITY = 0.9f;
    private readonly SyncVar<float> _opacity = new SyncVar<float>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

    private void Awake()
    {
        _opacity.OnChange += OnOpacity;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        // ModifyOpacity(-1f); // Set Morso invisible by default
    }

    [ServerRpc(RunLocally = true, RequireOwnership = false)]
    private void SetOpacityServerRpc(float value)
    {
        float prevValue = _opacity.Value;
        _opacity.Value = value;
        OnOpacity(prevValue, value, true); // Manually invoke the callback
    }

    // SyncVar change event is only triggered when the value is changed on the server
    // and then synchronized to the clients.
    private void OnOpacity(float prev, float next, bool asServer)
    {
        var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            foreach (var material in renderer.materials)
            {
                var color = material.color;
                color.a = next;
                material.color = color;
                renderer.enabled = next > 0; // Disable renderer so that we hide all visual effects
            }
        }
        else
        {
            Debug.LogWarning("Skinned Mesh renderer missing!");
        }
    }

    public void ModifyOpacity(float deltaOpacity)
    {
        SetOpacityServerRpc(Mathf.Clamp(_opacity.Value + deltaOpacity, MIN_OPACITY, MAX_OPACITY));
    }

    public void ToggleOpacity()
    {
        SetOpacityServerRpc(_opacity.Value == MIN_OPACITY ? MAX_OPACITY : MIN_OPACITY);
    }

    void OnTriggerEnter (Collider other)
    {
        // Check if light is hitting Morso
        if (other.gameObject.CompareTag("Light"))
        {
            ModifyOpacity(1f);
        }
    }

    void OnTriggerExit (Collider other)
    {
        // Check if light is leaving Morso
        if (other.gameObject.CompareTag("Light"))
        {
            ModifyOpacity(-1f);
        }
    }
}
