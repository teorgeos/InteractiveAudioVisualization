using UnityEngine;

public class DynamicParticleController : MonoBehaviour
{
    public MeshFilter meshFilter;                // 곰 모델의 Mesh Filter
    public ParticleSystem particleSystem;        // 파티클 시스템
    public Color vertexColor = Color.red;        // 정점 색상

    private ParticleSystem.Particle[] particles; // 파티클 배열
    private Vector3[] meshVertices;              // Mesh의 정점 위치
    private ParticleSystem.ShapeModule shapeModule; // Shape 모듈

    private float maxRandomizePosition = 500f;   // 최대 randomizePosition 값
    private float minRandomizePosition = 0f;     // 최소 randomizePosition 값 (볼륨 최댓값일 때)

    void Start()
    {
        if (meshFilter == null || particleSystem == null)
        {
            Debug.LogError("Mesh Filter 또는 Particle System이 설정되지 않았습니다.");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        meshVertices = mesh.vertices;

        // Shape 모듈 가져오기
        shapeModule = particleSystem.shape;

        // 초기 Shape 모듈 설정
        shapeModule.randomPositionAmount = maxRandomizePosition;

        particles = new ParticleSystem.Particle[meshVertices.Length];
    }

    public void SetRandomizePositionByRMS(float rms)
    {
        // RMS 값이 클수록 randomizePosition이 작아지게 설정 (최대 500 → 최소 0)
        float newRandomizePosition = Mathf.Lerp(maxRandomizePosition, minRandomizePosition, rms);

        // Shape 모듈의 Randomize Position 값 변경
        shapeModule.randomPositionAmount = newRandomizePosition;

        Debug.Log($"RMS: {rms}, RandomizePosition: {newRandomizePosition}");
    }
}
