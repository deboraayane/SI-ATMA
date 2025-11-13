using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CoreEditorDrawer<TData>;

uusing UnityEngine;
using System.Collections.Generic;
using System;


public class CollectibleSpawner : MonoBehaviour
{
    
    public GameObject collectiblePrefab;

    
    private List<Transform> spawnPoints;
    public float timeBetweenSpawns = 5f;
    private float spawnTimer;
    public int maxCollectibles = 3;

    void Start()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Nenhum ponto de spawn encontrado!");
            enabled = false;
        }
        spawnTimer = timeBetweenSpawns;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            TrySpawnCollectible();
            spawnTimer = timeBetweenSpawns;
        }
    }

    void TrySpawnCollectible()
    {
        if (GameObject.FindGameObjectsWithTag("Collectible").Length >= maxCollectibles) return;

        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        Transform chosenPoint = spawnPoints[randomIndex];

       
        if (Physics.OverlapSphere(chosenPoint.position, 0.5f).Length > 0)
        {
            
            return;
        }

        Instantiate(collectiblePrefab, chosenPoint.position, Quaternion.identity);
    }
}


public class Collectible : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
           
            Debug.Log("Objeto Coletado! Adicionar pontos aqui.");

            
            Destroy(gameObject);
        }
    }
}