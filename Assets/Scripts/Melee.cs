using UnityEngine;
using UnityEngine.InputSystem;

public class Melee : MonoBehaviour
{    
  [SerializeField] private Transform _cameraTransform;
  [SerializeField] private float _fireRate = 0.5f; 
  [SerializeField] private float weaponRange = 0.1f;
  [SerializeField] private GameObject explosion;
  [SerializeField] private float _weaponDamage = 10f;

  private float _nextFireTime = 0f;
  private AudioManager _audioManager;
  private void Awake()
  {
    _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
  }

  public void OnFire(InputAction.CallbackContext context)
  {
    if (Time.time >= _nextFireTime)
    {
      _nextFireTime = Time.time + _fireRate;

      RaycastHit hit;
      Debug.Log("Melee is on fire");
      if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, weaponRange))
      {
        if(hit.distance > weaponRange)
        {
          Debug.Log("Out of range");
          _audioManager.PlayLocalSfx("miss");
          return;
        }
        Debug.Log("Melee hit " + hit.transform.name);
        if(hit.transform.name == "ReadyCube")
        {
          gameObject.GetComponent<Teleport>().TeleportPlayer();
          Debug.Log("Melee hit ready cube");
        }
        var decal = Instantiate(explosion, hit.point, Quaternion.identity);
        Destroy(decal, 0.5f);

        Debug.Log("Melee hit " + hit.transform.name);
        if(hit.transform.GetComponentInParent<Health>())
        {
        hit.transform.GetComponentInParent<Health>().TakeDamage(_weaponDamage);
          _audioManager.PlayLocalSfx("hit");
        }
        else 
        {
          _audioManager.PlayLocalSfx("hit-wall");
        }
      }
      else
      {
        Debug.Log("Melee missed");
        _audioManager.PlayLocalSfx("miss");
      }
    }
    else
    {
      Debug.Log("Cannot Melee yet. Waiting for fire rate cooldown.");
    }
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {  
       
  }

  // Update is called once per frame
  void Update()
  {
        
  }
}
