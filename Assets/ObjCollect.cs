using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public GameObject collectiblePrefab;
    public float timeBetweenSpawns = 5f;
    public int maxCollectibles = 3;

    private List<Transform> spawnPoints;
    private float spawnTimer;

    void Start()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform child in transform) spawnPoints.Add(child);

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Nenhum ponto de spawn encontrado!");
            enabled = false;
            return;
        }

        spawnTimer = timeBetweenSpawns;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            TrySpawnCollectible();
            spawnTimer = timeBetweenSpawns;
        }
    }

    void TrySpawnCollectible()
    {
        if (collectiblePrefab == null) return;
        if (GameObject.FindGameObjectsWithTag("Collectible").Length >= maxCollectibles) return;

        int i = Random.Range(0, spawnPoints.Count);
        Transform point = spawnPoints[i];

        // evita spawn sobre outros colliders
        if (Physics.OverlapSphere(point.position, 0.5f).Length > 0) return;

        Instantiate(collectiblePrefab, point.position, Quaternion.identity);
    }
}

public class ObjCollect : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip collectSfx;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Visual")]
    [Tooltip("Duração do efeito de desaparecimento em segundos")]
    public float dissolveDuration = 0.5f;

    Collider cachedCollider;
    Renderer[] renderers;
    Material[] runtimeMats;
    bool collected = false;

    void Awake()
    {
        cachedCollider = GetComponent<Collider>();
        if (cachedCollider != null) cachedCollider.isTrigger = true;

        renderers = GetComponentsInChildren<Renderer>();
        runtimeMats = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                runtimeMats[i] = renderers[i].material; // instancia material para alteração local
            else
                runtimeMats[i] = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(CollectRoutine());
    }

    IEnumerator CollectRoutine()
    {
        collected = true;
        if (cachedCollider != null) cachedCollider.enabled = false;

        // Toca som (se atribuído)
        if (collectSfx != null)
            AudioSource.PlayClipAtPoint(collectSfx, transform.position, Mathf.Clamp01(sfxVolume));

        // Fade alpha (se suportado) + scale down
        float t = 0f;
        Vector3 startScale = transform.localScale;

        Color[] startColors = new Color[runtimeMats.Length];
        for (int i = 0; i < runtimeMats.Length; i++)
        {
            var m = runtimeMats[i];
            if (m != null && m.HasProperty("_Color"))
                startColors[i] = m.color;
            else
                startColors[i] = Color.white;
        }

        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dissolveDuration);

            // aplica fade alpha nos materiais que suportam
            for (int i = 0; i < runtimeMats.Length; i++)
            {
                var m = runtimeMats[i];
                if (m == null) continue;
                if (m.HasProperty("_Color"))
                {
                    Color c = startColors[i];
                    c.a = Mathf.Lerp(1f, 0f, u);
                    m.color = c;
                }
            }

            // reduz escala
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, u);

            yield return null;
        }

        // garante invisibilidade e destroi
        foreach (var r in renderers) if (r != null) r.enabled = false;

        yield return null; // um frame para o som iniciar
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (runtimeMats == null) return;
        for (int i = 0; i < runtimeMats.Length; i++)
        {
            if (runtimeMats[i] != null)
                Destroy(runtimeMats[i]);
        }
    }
}
