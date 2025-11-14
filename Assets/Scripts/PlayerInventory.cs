using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int NSpheres { get; private set; }

    public UnityEvent<PlayerInventory> OnSphereCollected;

    public void SphereCollected()
    {
        NSpheres++;
        OnSphereCollected.Invoke(this);
    }
}   