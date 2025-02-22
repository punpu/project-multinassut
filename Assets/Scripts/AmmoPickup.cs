using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public float speed = 20f;

    public void Update()
    {
        transform.Rotate(speed * Time.deltaTime * Vector3.up);
    }
}
