using UnityEngine;
using System.Collections; 
using UnityEngine.Rendering.Universal; 

public class DecalFader : MonoBehaviour
{
    [Tooltip("Tempo totale in secondi prima che il decal venga distrutto.")]
    public float lifetime = 10.0f;

    [Tooltip("Quanti secondi dura la dissolvenza finale.")]
    public float fadeDuration = 3.0f;

    private DecalProjector _decalProjector;

    void Start()
    {
        _decalProjector = GetComponent<DecalProjector>();

        StartCoroutine(FadeOutRoutine());

        Destroy(gameObject, lifetime);
    }

    private IEnumerator FadeOutRoutine()
    {
        float solidTime = lifetime - fadeDuration;
        if (solidTime > 0)
        {
            yield return new WaitForSeconds(solidTime);
        }

        float timer = 0;
        while (timer < fadeDuration)
        {
            _decalProjector.fadeFactor = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            timer += Time.deltaTime;
            yield return null; 
        }

        _decalProjector.fadeFactor = 0f;
    }
}