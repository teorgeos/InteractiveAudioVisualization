using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrumPlane : MonoBehaviour
{
    public AudioSource audioSource;
    public MeshFilter meshFilter;
    public float amplitude = 5f;
    public float smoothness = 0.1f;
    public static float globalWaveHeight = 0f; // 다른 스크립트에서 접근할 수 있는 전역 변수

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;

    void Start()
    {
        mesh = meshFilter.mesh;
        originalVertices = mesh.vertices;
        currentVertices = mesh.vertices;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        UpdateMeshWithAudio();
    }

    void UpdateMeshWithAudio()
    {
        if (!audioSource.isPlaying) return;

        float[] waveform = new float[1024];
        audioSource.GetOutputData(waveform, 0);

        Vector3[] targetVertices = new Vector3[originalVertices.Length];
        float totalHeight = 0f; // 전체 waveHeight 합산 값

        for (int i = 0; i < originalVertices.Length; i++)
        {
            float waveHeight = waveform[i % waveform.Length] * amplitude;
            targetVertices[i] = originalVertices[i] + new Vector3(0, waveHeight, 0);
            totalHeight += Mathf.Abs(waveHeight); // 높이의 절대값을 합산
        }

        // 평균 WaveHeight를 계산해서 전역 변수로 저장
        globalWaveHeight = totalHeight / originalVertices.Length;

        for (int i = 0; i < currentVertices.Length; i++)
        {
            currentVertices[i] = Vector3.Lerp(currentVertices[i], targetVertices[i], smoothness);
        }

        mesh.vertices = currentVertices;
        mesh.RecalculateNormals();
    }
}
