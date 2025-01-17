using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;       // 오디오 소스
    public Material waveMaterial;         // Line Renderer에 사용할 소재
    public int resolution = 64;           // 원형 점 수
    public Color baseColor = Color.cyan;  // 기본 색상
    public float colorIntensity = 2f;     // 색상 강도 조절

    private LineRenderer[] rings;         // 3개의 고정된 원
    private float[] spectrumData = new float[128]; // 주파수 데이터

    void Start()
    {
        // 고정된 위치에 3개의 Line Renderer 초기화
        rings = new LineRenderer[3];
        for (int i = 0; i < rings.Length; i++)
        {
            GameObject ringObj = new GameObject($"Ring_{i}");
            ringObj.transform.parent = this.transform;

            LineRenderer lr = ringObj.AddComponent<LineRenderer>();
            lr.material = waveMaterial;
            lr.positionCount = resolution + 1; // 원을 닫기 위해 시작점 반복
            lr.loop = true;
            lr.startWidth = 0.2f;
            lr.endWidth = 0.2f;
            rings[i] = lr;

            // 각 원의 반지름을 다르게 설정
            float radius = 2f + i * 2f; // 원마다 다른 크기
            SetCirclePoints(lr, radius);
        }
    }

    void Update()
    {
        // 오디오 주파수 데이터 가져오기
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        float volume = GetAverageVolume();

        // 각 원의 색상을 업데이트
        UpdateRingColors(volume);
    }

    void SetCirclePoints(LineRenderer lr, float radius)
    {
        for (int i = 0; i <= resolution; i++)
        {
            float angle = i * 2 * Mathf.PI / resolution;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    float GetAverageVolume()
    {
        // 주파수 데이터의 평균 볼륨 계산
        float sum = 0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            sum += spectrumData[i];
        }
        return sum / spectrumData.Length;
    }

    void UpdateRingColors(float volume)
    {
        // 볼륨에 따라 색상 강도와 알파값 조절
        for (int i = 0; i < rings.Length; i++)
        {
            Gradient gradient = new Gradient();

            // 동적으로 색상 및 알파값 적용
            Color dynamicColor = baseColor * Mathf.Clamp(volume * colorIntensity, 0.5f, 2f);
            dynamicColor.a = Mathf.Clamp(volume * 0.5f, 0.2f, 0.8f); // 알파값 설정

            gradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(dynamicColor, 0.0f), // 시작 색상
                new GradientColorKey(baseColor * 0.5f, 1.0f) // 끝 색상
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(dynamicColor.a, 0.0f), // 시작 알파
                new GradientAlphaKey(0.0f, 1.0f) // 끝 알파
                }
            );

            rings[i].colorGradient = gradient;
        }
    }

}
