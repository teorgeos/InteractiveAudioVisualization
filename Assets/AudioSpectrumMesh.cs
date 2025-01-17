using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AudioSpectrumMesh : MonoBehaviour
{
    public AudioSource audioSource;      // 오디오 소스
    public int gridResolution = 256;     // 그리드의 해상도 (x축 방향)
    public float spacing = 0.1f;         // 각 그리드의 간격
    public float heightMultiplier = 2f;  // 오디오 데이터에 따른 높이 배율
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    private Mesh mesh;                   // Procedural Mesh
    private Vector3[] vertices;          // 버텍스 배열
    private int[] triangles;             // 삼각형 인덱스
    private float[] audioSpectrum;       // 오디오 스펙트럼 데이터

    void Start()
    {
        // 스펙트럼 데이터 초기화
        audioSpectrum = new float[gridResolution];

        // Mesh 초기화
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateGrid();
        UpdateMesh();
    }

    void Update()
    {
        // 오디오 스펙트럼 가져오기
        audioSource.GetSpectrumData(audioSpectrum, 0, fftWindow);

        // 버텍스 Y축 변형 (오디오 데이터 적용)
        for (int x = 0; x < gridResolution; x++)
        {
            float height = audioSpectrum[x] * heightMultiplier;

            // 버텍스 위치 업데이트
            int index = x;
            vertices[index].y = height;
        }

        UpdateMesh();
    }

    void CreateGrid()
    {
        vertices = new Vector3[gridResolution];
        triangles = new int[(gridResolution - 1) * 6];

        // 버텍스 생성
        for (int x = 0; x < gridResolution; x++)
        {
            vertices[x] = new Vector3(x * spacing, 0, 0);
        }

        // 삼각형 생성 (라인 그리드)
        for (int i = 0; i < gridResolution - 1; i++)
        {
            int vertexIndex = i;
            int triIndex = i * 6;

            triangles[triIndex + 0] = vertexIndex;
            triangles[triIndex + 1] = vertexIndex + 1;
            triangles[triIndex + 2] = vertexIndex;

            triangles[triIndex + 3] = vertexIndex + 1;
            triangles[triIndex + 4] = vertexIndex;
            triangles[triIndex + 5] = vertexIndex + 1;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
