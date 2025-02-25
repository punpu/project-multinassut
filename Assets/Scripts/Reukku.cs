using FishNet;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using Unity.Cinemachine;

public class Reukku : NetworkBehaviour
{    
  [SerializeField] private Transform _cameraTransform;
  [SerializeField] private float _fireRate = 0.5f; 
  [SerializeField] private float weaponRange = 5000f;
  [SerializeField] private GameObject explosion;
  [SerializeField] private GameObject visualProjectile;
  [SerializeField] private float _weaponDamage = 10f;
  public readonly int MAX_AMMO = 6;
  public readonly int AMMO_PICKUP_AMOUNT = 3;
  private int _ammo; // TODO: update this when Reukku is fired
  private PlayerUI _playerUI;
  private float _nextFireTime = 0f;
  private AudioManager _audioManager;
  private void Awake()
  {
    _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
  }
  public void OnFire(InputAction.CallbackContext context)
  {
    if(context.phase != InputActionPhase.Started) {return;}

    if (Time.time >= _nextFireTime && _ammo > 0)
    {
      var position = transform.position;
      _audioManager.PlaySfx("reukku-shot", position);
      if (visualProjectile != null) {
        Instantiate(visualProjectile, _cameraTransform.position, _cameraTransform.rotation);
      }

      _ammo -= 1;
      _nextFireTime = Time.time + _fireRate;

      if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, weaponRange))
      {
        if (hit.distance > weaponRange)
        {
          return;
        }
        Debug.Log("Reukku hit " + hit.transform.name);
        if (hit.transform.name == "ReadyCube")
        {
          var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          gameManager.StartGame();
        }
        var decal = Instantiate(explosion, hit.point, Quaternion.identity);
        Destroy(decal, 0.5f);

        if (hit.transform.GetComponentInParent<Health>())
        {
          hit.transform.GetComponentInParent<Health>().TakeDamage(_weaponDamage);
        }
      }
      else
      {
        Debug.Log("Reukku missed");
      }
    }
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {  
    _playerUI = GetComponentInChildren<PlayerUI>();
    _ammo = MAX_AMMO;
  }

  // Update is called once per frame
  void Update()
  {
    if (_playerUI && base.IsOwner)
    {
      _playerUI.SetAmmo(_ammo);
    }
  }

  public void SetAmmo(int newAmmo)
  {
    _ammo = Mathf.Clamp(newAmmo, 0, MAX_AMMO);
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("AmmoPickup") && _ammo < MAX_AMMO)
    {
      SetAmmo(_ammo + AMMO_PICKUP_AMOUNT);
      other.gameObject.GetComponent<AmmoPickup>().OnPickUp();
    }
  }
}
