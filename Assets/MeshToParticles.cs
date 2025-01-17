using UnityEngine;

public class ParticleToMeshOnClick : MonoBehaviour
{
    public MeshFilter meshFilter;           // 곰 모델의 Mesh Filter
    public ParticleSystem particleSystem;   // 파티클 시스템
    public float moveSpeed = 5f;            // 파티클 이동 속도

    private ParticleSystem.Particle[] particles; // 파티클 배열
    private Vector3[] originalPositions;         // 초기 랜덤 위치
    private Vector3[] meshVertices;              // Mesh 정점 위치
    private bool moveToMesh = false;             // 파티클 이동 여부

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

        // 파티클 초기화
        particles = new ParticleSystem.Particle[meshVertices.Length];
        originalPositions = new Vector3[meshVertices.Length];

        for (int i = 0; i < meshVertices.Length; i++)
        {
            // 파티클을 랜덤 위치에 초기화
            originalPositions[i] = Random.insideUnitSphere * 5f; // 공중에 흩어진 상태
            particles[i].position = originalPositions[i];
            particles[i].startSize = 0.05f; // 파티클 크기
            particles[i].startColor = Color.white; // 파티클 색상
        }

        particleSystem.SetParticles(particles, particles.Length);
    }

    void Update()
    {
        // 마우스 클릭 시 파티클이 Mesh로 이동
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 클릭
        {
            moveToMesh = true;
        }

        if (moveToMesh)
        {
            MoveParticlesToMesh();
        }
    }

    void MoveParticlesToMesh()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            // Mesh 정점 위치로 부드럽게 이동
            Vector3 targetPosition = meshFilter.transform.TransformPoint(meshVertices[i]);
            particles[i].position = Vector3.Lerp(particles[i].position, targetPosition, Time.deltaTime * moveSpeed);
        }

        particleSystem.SetParticles(particles, particles.Length);
    }
}
