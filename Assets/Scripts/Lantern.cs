using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.Collections;

public class Lantern : NetworkBehaviour
{
    private const bool INITIAL_SWITCHED_ON_VALUE = false;
    private const int LIGHT_INTENSITY_OFF = 0;
    private const int LIGHT_INTENSITY_ON = 300;
    private const int MIN_BATTERY = 0;
    private const int MAX_BATTERY = 10;
    private readonly SyncVar<bool> _isSwitchedOn = new SyncVar<bool>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));
    private int _battery = MAX_BATTERY;

    private void Awake()
    {
        _isSwitchedOn.OnChange += OnIsSwitchedOn;
    }

    void Start()
    {
        StartCoroutine(BatteryUpdate());
    }

    IEnumerator BatteryUpdate() {
        while (true) {
            if (_isSwitchedOn.Value && _battery > MIN_BATTERY)
            {
                ModifyBattery(-2);
            }
            else if (!_isSwitchedOn.Value && _battery < MAX_BATTERY)
            {
                ModifyBattery(1);
            }
            yield return new WaitForSeconds(1);
        }    
    }

    private void OnIsSwitchedOn(bool prev, bool next, bool asServer)
    {
        var spotLight = GetComponentInChildren<Light>();
        var collider = GetComponentInChildren<MeshCollider>();
        if (!spotLight)
        {
            Debug.LogWarning("SpotLight missing!");
            return;
        }
        if (!collider)
        {
            Debug.LogWarning("Collider missing!");
            return;
        }
        spotLight.intensity = next ? LIGHT_INTENSITY_ON : LIGHT_INTENSITY_OFF;
        collider.enabled = next;
    }

    [ServerRpc(RunLocally = true, RequireOwnership = false)]
    private void SetIsOnServerRpc(bool value)
    {
        if (value && _battery <= MIN_BATTERY)
        {
            // Lantern is out of battery
            return;
        }
        var prevValue = _isSwitchedOn.Value;
        _isSwitchedOn.Value = value;
        OnIsSwitchedOn(prevValue, value, true); // Manually invoke the callback
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        SetIsOnServerRpc(INITIAL_SWITCHED_ON_VALUE);
    }

    public void ModifyBattery(int deltaBattery)
    {
        _battery = Mathf.Clamp(_battery + deltaBattery, MIN_BATTERY, MAX_BATTERY);
        if (_battery <= MIN_BATTERY)
        {
            SetIsOnServerRpc(false);
        }
    }

    public void ToggleLight()
    {
        SetIsOnServerRpc(!_isSwitchedOn.Value);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner || context.phase != InputActionPhase.Started) { return; }
        ToggleLight();
    }
}
