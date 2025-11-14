using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneColorFilter : MonoBehaviour
{
    public Image filterImage;
    public float fadeDuration = 0.5f; 

    public float holdDuration = 0.5f; 
    public Color normalColor = new Color(0, 0, 0, 0); 

    private Coroutine currentRoutine;

    void Start()
    {
        if (filterImage != null)
        {
            
            filterImage.color = normalColor;
        }
    }

   
    public void TriggerFilter(Color targetColor)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeToColor(targetColor));
    }

   
    public void ClearFilter()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeToColor(normalColor));
    }

    
    public void TriggerFilterAndReturn(Color targetColor)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FlashFilter(targetColor));
    }

    IEnumerator FlashFilter(Color targetColor)
    {
        yield return StartCoroutine(FadeToColor(targetColor));

        yield return new WaitForSeconds(holdDuration);

        yield return StartCoroutine(FadeToColor(normalColor));

        currentRoutine = null; 
    }
  
    IEnumerator FadeToColor(Color targetColor)
    {
        Color startColor = filterImage.color;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
          
            filterImage.color = Color.Lerp(startColor, targetColor, time / fadeDuration);
            yield return null;
        }

        
        filterImage.color = targetColor;
        
    }
}