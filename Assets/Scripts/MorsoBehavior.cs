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
    private const float MIN_OPACITY = 0f;
    private const float MAX_OPACITY = 0.9f;
    private readonly SyncVar<float> _opacity = new SyncVar<float>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

    private void Awake()
    {
        _opacity.OnChange += on_opacity;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        ModifyOpacity(0f); // Set Morso invisible by default
    }

    [ServerRpc(RunLocally = true, RequireOwnership = false)]
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
            var color = renderer.material.color;
            color.a = next;
            renderer.material.color = color;
            renderer.enabled = next > 0; // Disable renderer so that we hide all visual effects
        }
    }

    public void ModifyOpacity(float change)
    {
        SetOpacityServerRpc(Mathf.Clamp(_opacity.Value + change, MIN_OPACITY, MAX_OPACITY));
    }

    public void ToggleOpacity()
    {
        SetOpacityServerRpc(_opacity.Value == MIN_OPACITY ? MAX_OPACITY : MIN_OPACITY);
    }

    // Testing how to call other object's method
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner || context.phase != InputActionPhase.Started) { return; }

        GameObject otherObject = GameObject.Find("Player(Clone)"); // TODO: figure out a nicer way to find the player
        if (otherObject != null)
        {
            MorsoBehavior otherMorsoBehavior = otherObject.GetComponent<MorsoBehavior>();
            if (otherMorsoBehavior != null)
            {
                otherMorsoBehavior.ModifyOpacity(0.2f);
            }
            else
            {
                Debug.LogWarning($"MorsoBehavior not found.");
            }
        }
        else
        {
            Debug.LogWarning($"GameObject not found.");
        }
    }

    void OnTriggerEnter (Collider other)
    {
        // Check if light is hitting Morso
        if (other.gameObject.CompareTag("Light"))
        {
            Debug.Log($"Morso trigger, isLight");
            ModifyOpacity(1f);
        }
    }

    void OnTriggerExit (Collider other)
    {
        // Check if light is leaving Morso
        if (other.gameObject.CompareTag("Light"))
        {
            Debug.Log($"Morso trigger, isLight");
            ModifyOpacity(-1f);
        }
    }
}
