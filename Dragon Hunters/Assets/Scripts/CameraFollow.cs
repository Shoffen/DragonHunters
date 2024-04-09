using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private const float shakeIntensity = 0.1f;
    private Vector3 offset;
    public float smoothing = 5f;
    public float fixedYCoordinate = 1f;
    public float fixedZCoordinate;
    public float currentShakeIntensity = 0.1f;
    public float shakeIncreaseRate = 0.02f;
    public float maxShakeIntensity = 0.5f;
    public bool needShake;
    private Vector3 randomShake;
    public bool isWaveInProgress;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;
    private bool released = false;
    [SerializeField] private Transform target;

    private void Start()
    {
        offset = transform.position - (target != null ? target.position : Vector3.zero);
        isWaveInProgress = false;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            if (needShake)
            {
                currentShakeIntensity = Mathf.Clamp(currentShakeIntensity + shakeIncreaseRate * Time.deltaTime, 0f, maxShakeIntensity);
                randomShake = new Vector3(Random.Range(-currentShakeIntensity, currentShakeIntensity), 0f, Random.Range(-currentShakeIntensity, currentShakeIntensity));
            }
            else if (released)
            {
                randomShake = new Vector3(Random.Range(-0.45f, 0.45f), 0f, Random.Range(-0.45f, 0.45f));
            }
            else
            {
                currentShakeIntensity = shakeIntensity;
                randomShake = Vector3.zero;
            }

            Vector3 targetCamPos = new Vector3(target.position.x, fixedYCoordinate, fixedZCoordinate) + offset + randomShake;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("CameraFollow: Target object is null.");
        }
    }

    public void ShakeAfterRelease()
    {
        released = true;
        StartCoroutine(ResetRelease());
    }

    private IEnumerator ResetRelease()
    {
        yield return new WaitForSeconds(0.25f);
        released = false;
    }
}
