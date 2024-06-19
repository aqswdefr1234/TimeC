using UnityEngine;
public class Disappearance : MonoBehaviour
{
    void Start()
    {
        Invoke("Disappear", 5f);
    }
    void Disappear()
    {
        Destroy(gameObject);
    }
}