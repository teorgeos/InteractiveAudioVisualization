using UnityEngine;

public class ParticleOnVertices : MonoBehaviour
{
    public MeshFilter meshFilter;                // 곰 모델의 Mesh Filter
    public ParticleSystem particleSystem;        // 파티클 시스템
    public Color vertexColor = Color.red;        // 정점 색상

    [Header("Dynamic Particle Settings")]
    public float weatherValue = 0f;              // 날씨 API에서 받아올 값 (예: 강수량, 적설량 등)
    public float soundValue = 0f;                // 특정 소리 크기 (0 ~ 1)

    private ParticleSystem.Particle[] particles; // 파티클 배열
    private Vector3[] meshVertices;              // Mesh의 정점 위치
    private ParticleSystem.MainModule mainModule;
    private float randomizePosition = 500f;      // 랜덤 위치 초기값
    private int minParticles = 1000;             // 최소 파티클 수
    private int maxParticles = 5000;             // 최대 파티클 수

    void Start()
    {
        if (meshFilter == null || particleSystem == null)
        {
            Debug.LogError("Mesh Filter 또는 Particle System이 설정되지 않았습니다.");
            return;
        }

        // Mesh 정점 가져오기
        Mesh mesh = meshFilter.mesh;
        meshVertices = mesh.vertices;

        // ParticleSystem Main 모듈 가져오기
        mainModule = particleSystem.main;
        mainModule.maxParticles = minParticles;

        // 파티클 초기화
        particles = new ParticleSystem.Particle[meshVertices.Length];
        UpdateParticles();
    }

    void Update()
    {
        UpdateDynamicSettings();
        UpdateParticles();
    }

    void UpdateDynamicSettings()
    {
        // 날씨 값이 클수록 maxParticles 증가 (최소 1000, 최대 maxParticles)
        int dynamicMaxParticles = Mathf.Clamp((int)(weatherValue * 1000), minParticles, maxParticles);
        mainModule.maxParticles = dynamicMaxParticles;

        // 특정 소리가 클수록 Randomize Position 값 감소 (최소 0, 최대 500)
        randomizePosition = Mathf.Clamp(500f - (soundValue * 500f), 0f, 500f);
    }

    void UpdateParticles()
    {
        for (int i = 0; i < meshVertices.Length; i++)
        {
            // 파티클 위치를 랜덤하게 변경
            Vector3 randomOffset = Random.insideUnitSphere * randomizePosition;
            particles[i].position = meshFilter.transform.TransformPoint(meshVertices[i]) + randomOffset;
            particles[i].startSize = 0.05f;
            particles[i].startColor = vertexColor;
        }

        particleSystem.SetParticles(particles, particles.Length);
    }
}
