using UnityEngine;
using UnityEngine.InputSystem;

public class Reukku : MonoBehaviour
{    
  [SerializeField] private Transform _cameraTransform;
  [SerializeField] private float _fireRate = 0.5f; 
  [SerializeField] private float weaponRange = 5000f;
  [SerializeField] private GameObject explosion;
  [SerializeField] private float _weaponDamage = 10f;

  private float _nextFireTime = 0f;

  public void OnFire(InputAction.CallbackContext context)
  {
    if (Time.time >= _nextFireTime)
    {
      _nextFireTime = Time.time + _fireRate;

      RaycastHit hit;
      Debug.Log("Reukku is on fire");
      if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, weaponRange))
      {
        Debug.Log(hit.distance + " meters." + weaponRange + " meters.");
        if(hit.distance > weaponRange)
        {
          Debug.Log("Out of range");
          return;
        }
        {
          Debug.Log("Reukku hit something at " + hit.distance + " meters");
        }
        Debug.Log("Reukku hit " + hit.transform.name);
        if(hit.transform.name == "ReadyCube")
        {
          //gameObject.GetComponent<Teleport>().TeleportPlayer();
          var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          gameManager.StartGame();
          Debug.Log("Reukku hit ready cube");
        }
        var decal = Instantiate(explosion, hit.point, Quaternion.identity);
        Destroy(decal, 0.5f);

        hit.transform.GetComponent<Health>().TakeDamage(_weaponDamage);
      }
      else
      {
        Debug.Log("Reukku missed");
      }
    }
    else
    {
      Debug.Log("Cannot fire yet. Waiting for fire rate cooldown." + _nextFireTime);
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
