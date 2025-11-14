using System.Collections;
using UnityEngine;


public class Spheres : MonoBehaviour
{
  
    public AudioClip collectSfx;
    public AudioClip voiceClip;
    public float sfxVolume = 1f;

    [Range(1f, 2f)] public float voiceBoost = 1.2f;

    public float fadeDuration = 0.5f;

   
    public SceneColorFilter sceneColorFilter; 
    public Color filterColor = new Color(0f, 1f, 1f, 0.3f); 
    public float filterHoldTime = 0.3f; 


    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponentInParent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.SphereCollected();

            if (collectSfx != null)
                AudioSource.PlayClipAtPoint(collectSfx, transform.position, Mathf.Clamp01(sfxVolume));

            StartCoroutine(PlayVoiceAfterSfx());

            
            if (sceneColorFilter != null)
                StartCoroutine(ApplyAndClearFilter());

            StartCoroutine(FadeAndDestroy());
        }
    }

    
    IEnumerator ApplyAndClearFilter()
    {
        
        sceneColorFilter.TriggerFilter(filterColor);

        
        yield return new WaitForSeconds(filterHoldTime);

        
        sceneColorFilter.ClearFilter();
    }


    IEnumerator PlayVoiceAfterSfx()
    {
        if (voiceClip != null)
        {
            yield return new WaitForSeconds(0.1f);

            float finalVoiceVolume = sfxVolume * voiceBoost;

            AudioSource.PlayClipAtPoint(voiceClip, transform.position, Mathf.Clamp01(finalVoiceVolume));
        }
        yield return null;
    }

    IEnumerator FadeAndDestroy()
    {
       
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