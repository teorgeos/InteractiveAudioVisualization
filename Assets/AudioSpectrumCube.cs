using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AudioSpectrumCube : MonoBehaviour
{
    public AudioSource audioSource;          // 오디오 소스
    public float heightMultiplier = 0.5f;    // 메시 울렁임의 강도
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    private Mesh mesh;                       // Cube Mesh
    private Vector3[] originalVertices;      // 원본 버텍스 배열
    private Vector3[] modifiedVertices;      // 수정된 버텍스 배열
    private float[] audioSpectrum;           // 오디오 스펙트럼 데이터

    void Start()
    {
        // Cube의 Mesh 가져오기
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;     // 원본 버텍스 저장
        modifiedVertices = mesh.vertices;     // 수정용 버텍스 초기화

        // 오디오 스펙트럼 데이터 배열 초기화
        audioSpectrum = new float[64]; // 2의 거듭제곱 권장
    }

    void Update()
    {
        // 오디오 스펙트럼 데이터 가져오기
        audioSource.GetSpectrumData(audioSpectrum, 0, fftWindow);

        // 모든 버텍스를 동시에 꿀렁이게 수정
        for (int i = 0; i < originalVertices.Length; i++)
        {
            float spectrumValue = audioSpectrum[i % audioSpectrum.Length];
            float heightOffset = Mathf.Sin(Time.time * spectrumValue * 10f) * heightMultiplier;

            // Y축을 중심으로 전체 Mesh가 꿀렁이게 만듦
            modifiedVertices[i] = originalVertices[i] + new Vector3(0, heightOffset, 0);
        }

        // Mesh에 수정된 버텍스 적용
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
