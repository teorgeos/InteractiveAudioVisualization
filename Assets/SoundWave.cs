using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave
{
    private LineRenderer lineRenderer;   // 물결을 표현할 LineRenderer
    private float radius;                // 물결 반지름
    private float expansionSpeed;        // 반지름 확장 속도
    private float maxRadius;             // 물결이 퍼지는 최대 반지름
    private float lifeTime;              // 물결 수명
    private float age;                   // 물결의 경과 시간
    private float waveHeight;            // 울렁이는 높이

    public SoundWave(LineRenderer lr, float initialRadius, float speed, float maxR, float life, float height)
    {
        lineRenderer = lr;
        radius = initialRadius;
        expansionSpeed = speed;
        maxRadius = maxR;
        lifeTime = life;
        age = 0f;
        waveHeight = height;

        UpdateLineRenderer();
    }

    public bool UpdateWave(float deltaTime)
    {
        // 시간에 따라 반지름 확장 및 수명 증가
        radius += expansionSpeed * deltaTime;
        age += deltaTime;

        // 투명도 조절 (시간이 지남에 따라 서서히 사라짐)
        float alpha = Mathf.Lerp(1f, 0f, age / lifeTime);
        Color color = lineRenderer.startColor;
        color.a = alpha;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        UpdateLineRenderer();

        // 수명 또는 최대 반지름 초과 시 삭제
        if (age >= lifeTime || radius >= maxRadius)
        {
            GameObject.Destroy(lineRenderer.gameObject);
            return true;
        }
        return false;
    }

    private void UpdateLineRenderer()
    {
        // 원형으로 LineRenderer의 점을 배치
        int points = lineRenderer.positionCount;
        for (int i = 0; i < points; i++)
        {
            float angle = i * 2 * Mathf.PI / (points - 1);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // 울렁임을 고정값으로 적용
            float y = Mathf.Sin(angle * 4f) * waveHeight;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));
        }
    }
}


