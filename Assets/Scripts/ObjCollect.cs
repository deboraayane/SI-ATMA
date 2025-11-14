using System.Collections;
using UnityEngine;

public class Spheres : MonoBehaviour
{
    // Efeito sonoro de Coleta (CURTO)
    public AudioClip collectSfx;

    // NOVO: Clipe de Voz/Diálogo (MAIS LONGO)
    public AudioClip voiceClip;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    public float fadeDuration = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponentInParent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.SphereCollected();

            // 1. Toca o EFEITO SONORO (SFX)
            if (collectSfx != null)
                AudioSource.PlayClipAtPoint(collectSfx, transform.position, Mathf.Clamp01(sfxVolume));

            // 2. Toca o CLIPE DE VOZ logo em seguida
            // Usamos uma Coroutine para um pequeno atraso, se o clipe for longo.
            StartCoroutine(PlayVoiceAfterSfx());

            // 3. Inicia o Fade (e a destruição, que é mais longa que o áudio)
            StartCoroutine(FadeAndDestroy());
        }
    }

    // NOVA COROUTINE: Toca o clipe de voz após um pequeno atraso
    IEnumerator PlayVoiceAfterSfx()
    {
        if (voiceClip != null)
        {
            // Opcional: Pequeno atraso para garantir que o SFX curto comece primeiro.
            // 0.1s é geralmente o suficiente para um SFX rápido.
            yield return new WaitForSeconds(0.1f);

            // Toca a voz
            AudioSource.PlayClipAtPoint(voiceClip, transform.position, Mathf.Clamp01(sfxVolume));
        }
        yield return null;
    }

    // Restante do código FadeAndDestroy() permanece o mesmo...
    IEnumerator FadeAndDestroy()
    {
        // ... (Seu código de Fade e Destruição) ...

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;


        Renderer[] rends = GetComponentsInChildren<Renderer>();
        Material[] mats = new Material[rends.Length];
        Color[] startColors = new Color[rends.Length];

        for (int i = 0; i < rends.Length; i++)
        {
            if (rends[i] != null)
            {
                mats[i] = rends[i].material;
                if (mats[i].HasProperty("_Color")) startColors[i] = mats[i].color;
                else startColors[i] = Color.white;
            }
        }

        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / fadeDuration);

            // aplica fade alpha
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null) continue;
                if (mats[i].HasProperty("_Color"))
                {
                    Color c = startColors[i];
                    c.a = Mathf.Lerp(1f, 0f, u);
                    mats[i].color = c;
                }
            }

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, u);

            yield return null;
        }

        foreach (var r in rends) if (r != null) r.enabled = false;
        Destroy(gameObject);
    }
}