using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [Header("Particle Settings")]
    public ParticleSystem particleSystem;        // 파티클 시스템
    public float maxRandomizePosition = 50f;     // 최대 randomizePosition 값
    public float minRandomizePosition = 0f;      // 최소 randomizePosition 값
    public float changeInterval = 0.5f;          // 변화 적용 주기 (초 단위)
    public float autoEffectInterval = 3f;        // 파티클 효과 변화 주기

    [Header("RMS Value")]
    public float currentRMS = 0f;                // 전달받은 RMS 값

    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ColorOverLifetimeModule colorModule;

    private float timer = 0f;                    // randomizePosition 주기 타이머
    private float effectTimer = 0f;              // 파티클 효과 변경 주기 타이머

    void Start()
    {
        if (particleSystem == null)
        {
            Debug.LogError("ParticleSystem이 설정되지 않았습니다.");
            return;
        }

        // 모듈 가져오기
        shapeModule = particleSystem.shape;
        mainModule = particleSystem.main;
        emissionModule = particleSystem.emission;
        colorModule = particleSystem.colorOverLifetime;

        // 초기값 설정
        UpdateRandomizePosition();
    }

    void Update()
    {
        timer += Time.deltaTime;
        effectTimer += Time.deltaTime;

        // 일정 주기마다 randomizePosition 업데이트
        if (timer >= changeInterval)
        {
            UpdateRandomizePosition();
            timer = 0f;
        }

        // 일정 주기마다 파티클 효과를 화려하게 변경
        if (effectTimer >= autoEffectInterval)
        {
            ApplyRandomEffect();
            effectTimer = 0f;
        }
    }

    void UpdateRandomizePosition()
    {
        // RMS 값을 기반으로 randomizePosition 계산
        float newRandomizePosition = Mathf.Lerp(maxRandomizePosition, minRandomizePosition, Mathf.Clamp01(currentRMS));

        // Shape 모듈에 값 적용
        shapeModule.randomPositionAmount = newRandomizePosition;

        Debug.Log($"RMS: {currentRMS}, RandomizePosition: {newRandomizePosition}");
    }

    void ApplyRandomEffect()
    {
        // 파티클 메인 속성 변경
        mainModule.startSize = Random.Range(0.5f, 3.0f);  // 파티클 크기 랜덤 변경
        mainModule.startSpeed = Random.Range(1f, 10f);    // 파티클 속도 랜덤 변경
        mainModule.startLifetime = Random.Range(1f, 5f);  // 수명 랜덤 변경

        // 색상 변화 적용
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(Random.value, Random.value, Random.value), 0.0f),
                new GradientColorKey(new Color(Random.value, Random.value, Random.value), 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorModule.color = new ParticleSystem.MinMaxGradient(gradient);

        // 방출량 랜덤 설정
        emissionModule.rateOverTime = Random.Range(10f, 100f);

        Debug.Log("Random Particle Effect Applied");
    }

    public void UpdateRMS(float rms)
    {
        currentRMS = rms;
    }
}
