using UnityEngine;

public class AddColliders : MonoBehaviour
{
    [ContextMenu("Adicionar Box Colliders")]
    void AddBoxColliders()
    {
        // percorre todos os filhos do objeto
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Collider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }
        }
        Debug.Log("Box Colliders adicionados a todos os filhos sem collider!");
    }
}
