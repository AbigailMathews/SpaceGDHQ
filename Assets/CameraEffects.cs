using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField]
    float shakeDuration = .1f;
    [SerializeField]
    float shakeAmount = .3f;
    public IEnumerator Shake()
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        Debug.Log("Shaking");

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1 * shakeAmount, shakeAmount);
            float y = Random.Range(-1 * shakeAmount, shakeAmount);

            transform.position = new Vector3(x, y, startPosition.z);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        transform.position = startPosition;
    }
}
