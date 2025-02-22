using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Serializing.Helping;
using FishNet.Transporting;

public class Lantern : NetworkBehaviour
{
    private const float MIN_BATTERY = 0f;
    private const float MAX_BATTERY = 1f;
    private readonly SyncVar<bool> _isOn = new SyncVar<bool>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));
    // private readonly SyncVar<float> _battery = new SyncVar<float>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));
    private float _battery = 1f;

    private void Awake()
    {
        _isOn.OnChange += on_isOn;
        // _battery.OnChange += on_battery;
    }

    // private void on_battery(float prev, float next, bool asServer)
    // {
    //     Debug.Log($"Battery hanged from {prev} to {next}");
    //     if (next <= MIN_BATTERY)
    //     {
    //         Debug.Log("Lantern is out of battery");
    //     }
    // }

    private void on_isOn(bool prev, bool next, bool asServer)
    {
        Debug.Log($"Lantern is turned {(next ? "on" : "off")}");
        // if ()
    }

    // [ServerRpc(RunLocally = true, RequireOwnership = false)]
    // private void SetBatteryServerRpc(float value)
    // {
    //     float prevValue = _battery.Value;
    //     _battery.Value = value;
    //     on_battery(prevValue, value, true); // Manually invoke the callback
    // }

    [ServerRpc(RunLocally = true, RequireOwnership = false)]
    private void SetIsOnServerRpc(bool value)
    {
        Debug.Log($"SetIsOnServerRpc, value: {value}");
        if (value && _battery <= MIN_BATTERY)
        {
            Debug.Log("Lantern is out of battery");
            return;
        }
        var prevValue = _isOn.Value;
        _isOn.Value = value;
        on_isOn(prevValue, value, true); // Manually invoke the callback
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        // ModifyBattery(1f); // Set Lantern fully charged by default
    }

    public void ModifyBattery(float change)
    {
        // SetBatteryServerRpc(Mathf.Clamp(_battery.Value + change, MIN_BATTERY, MAX_BATTERY));
    }

    public void ToggleLight()
    {
        SetIsOnServerRpc(!_isOn.Value);
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner || context.phase != InputActionPhase.Started) { return; }
        ToggleLight();
    }
}
