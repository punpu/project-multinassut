using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerLook : NetworkBehaviour
{

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _sensitivity = 0.04f;
    [Range(0f, 90f)][SerializeField] private float _yRotationLimit = 88f;

    private Vector2 _rotation = Vector2.zero;
    private float _cameraPitch = 0f;
    private static Vector3 _vectorRight = Vector3.right;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _cameraTransform.GetComponent<CinemachineCamera>().enabled = IsOwner;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }
        var mouseVector = context.ReadValue<Vector2>();
        _rotation.x = mouseVector.x * _sensitivity;
        _rotation.y = mouseVector.y * _sensitivity;
        _cameraPitch -= _rotation.y;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -_yRotationLimit, _yRotationLimit);

        _cameraTransform.localEulerAngles = _vectorRight * _cameraPitch;
        transform.Rotate(0f, _rotation.x, 0f);
    }

}

