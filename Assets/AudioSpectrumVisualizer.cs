using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioSpectrumVisualizer : MonoBehaviour
{
    public AudioSource audioSource;       // 오디오 소스
    public int spectrumResolution = 64;   // 주파수 데이터 해상도
    public float heightMultiplier = 1f;   // 높이 배율
    public float lineSpacing = 30f;      // 라인 간 간격 (Z축, 더 넓게 설정)
    public float lineWidth = 0.00001f;      // 선의 두께 (더 얇게 설정)
    public Material lineMaterial;         // LineRenderer에 사용할 Material

    private List<Vector3[]> linePositions = new List<Vector3[]>(); // 포지션 배열 리스트
    private List<LineRenderer> lines = new List<LineRenderer>();   // 라인 리스트
    private float[] spectrumData;         // 주파수 데이터

    void Start()
    {
        spectrumData = new float[spectrumResolution];

        // 초기 라인 생성
        for (int i = 0; i < 50; i++) // 50개의 라인
        {
            GameObject lineObj = new GameObject($"SpectrumLine_{i}");
            lineObj.transform.parent = transform;
            lineObj.transform.localPosition = new Vector3(0, 0, -i * lineSpacing); // 라인 간격 조정

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = spectrumResolution;
            lr.material = lineMaterial;
            lr.startWidth = lineWidth; // 선 두께 조정
            lr.endWidth = lineWidth;

            lines.Add(lr);
            linePositions.Add(new Vector3[spectrumResolution]);
        }
    }

    void Update()
    {
        // 스펙트럼 데이터 가져오기
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // 이전 라인 위치 밀어내기
        for (int i = linePositions.Count - 1; i > 0; i--)
        {
            for (int j = 0; j < spectrumResolution; j++)
            {
                linePositions[i][j] = linePositions[i - 1][j];
            }
        }

        // 새로운 스펙트럼 데이터를 첫 번째 라인에 반영
        for (int j = 0; j < spectrumResolution; j++)
        {
            float x = j * 0.1f; // X축 위치
            float y = Mathf.Clamp(spectrumData[j] * heightMultiplier, 0, 2f); // 높이값 제한
            linePositions[0][j] = new Vector3(x, y, 0);
        }

        // 모든 라인의 위치 업데이트
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].SetPositions(linePositions[i]);
        }
    }
}
