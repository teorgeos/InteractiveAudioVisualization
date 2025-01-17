using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAudioController : MonoBehaviour
{
    public static int globalClipIndex = 0;
    public List<AudioClip> audioClips;
    public float pushDistance = 0.2f;
    public float pushSpeed = 3f;
    public float rotationSpeed = 10f;
    public float bounceAmplitude = 1.0f;
    public float bounceSpeed = 15f;

    [Header("Scale Settings")]
    public float minScale = 1.0f;   // 최소 스케일
    public float maxScale = 1.5f;   // 최대 스케일
    public float scaleSpeed = 5f;   // 스케일 변화 속도

    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Vector3 originalScale; // 원래 크기 저장
    private bool isAnimating = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        originalPosition = transform.position;
        originalScale = transform.localScale; // 초기 스케일 저장
    }

    void Update()
    {
        // 오브젝트 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        // globalWaveHeight를 사용하여 튀는 효과
        ApplyBounceEffect();

        // 소리에 따라 오브젝트 스케일 조정
        if (audioSource.isPlaying)
        {
            AdjustScaleByAudio();
        }
        else
        {
            // 소리 종료 후 원래 크기로 되돌아감
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSpeed);
        }
    }

    void OnMouseDown()
    {
        if (isAnimating || audioSource.isPlaying)
            return;

        PlayGlobalAudioClip();
        StartCoroutine(PlayPushAnimation());
    }

    void PlayGlobalAudioClip()
    {
        if (audioClips.Count > 0)
        {
            audioSource.clip = audioClips[globalClipIndex % audioClips.Count];
            audioSource.Play();
            globalClipIndex++;
        }
    }

    void ApplyBounceEffect()
    {
        // AudioSpectrumPlane에서 계산된 globalWaveHeight 값을 사용
        float bounceOffset = AudioSpectrumPlane.globalWaveHeight * bounceAmplitude;

        // 오브젝트 Y축 위치를 튀게 설정
        Vector3 bouncePosition = originalPosition + new Vector3(0, bounceOffset, 0);
        transform.position = Vector3.Lerp(transform.position, bouncePosition, Time.deltaTime * bounceSpeed);
    }

    void AdjustScaleByAudio()
    {
        // 오디오 RMS 값 계산
        float rms = CalculateRMS();

        // RMS 값을 기반으로 스케일 계산
        float scaleFactor = Mathf.Lerp(minScale, maxScale, rms * 10f); // rms 값 확장

        // 스케일을 부드럽게 조정
        Vector3 targetScale = originalScale * scaleFactor;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    float CalculateRMS()
    {
        float[] samples = new float[1024];
        audioSource.GetOutputData(samples, 0);

        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / samples.Length); // RMS 값 반환
    }

    IEnumerator PlayPushAnimation()
    {
        isAnimating = true;
        Vector3 pushedPosition = originalPosition - transform.forward * pushDistance;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(originalPosition, pushedPosition, elapsedTime);
            elapsedTime += Time.deltaTime * pushSpeed;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(pushedPosition, originalPosition, elapsedTime);
            elapsedTime += Time.deltaTime * pushSpeed;
            yield return null;
        }

        transform.position = originalPosition;
        isAnimating = false;
    }
}
