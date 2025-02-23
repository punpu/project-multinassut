using UnityEngine;

public class CursorController : MonoBehaviour
{
    private GameObject _fishNetHud;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _fishNetHud = GameObject.Find("NetworkHudCanvas");
        if (_fishNetHud != null)
        {
            _fishNetHud.SetActive(false);
        }
        Debug.Log("Cursor locked; toggle by pressing H");
    }

    void Update()
    {        
        // Hide/Show mouse cursor
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleCursorLock();
        }
    }

    // Toggle mouse cursor lock mode
    public void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (_fishNetHud != null)
            {
                _fishNetHud.SetActive(false);
            }
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
            if (_fishNetHud != null)
            {
                _fishNetHud.SetActive(true);
            }
        }
    }
}
