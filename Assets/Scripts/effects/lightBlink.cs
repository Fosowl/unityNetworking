using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightBlink : MonoBehaviour
{
    // private
    private Light lightSource;
    // public
    public float minWaitTime = 0.1f;
    public float maxWaitTime = 0.5f;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;

    void Start()
    {
        lightSource = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            if (lightSource.enabled)
            {
                lightSource.intensity = Random.Range(minIntensity, maxIntensity); // Randomize intensity
            }
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime)); // Wait for a random duration before the next flicker
        }
    }
}
