using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class WeatherDisplay : MonoBehaviour
{
    public string apiKey = "14519e70597f929ead3b856ee9b1800c";
    public string cityName = "Sapporo";

    [Header("Weather Data")]
    public static float temperature = 0f;
    public static float snowfall = 0f;
    public static string weatherStatus;

    [Header("UI Elements")]
    public TMP_Text weatherText;
    private string apiUrl;

    void Start()
    {
        apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric";
        StartCoroutine(GetWeatherData());
    }

    IEnumerator GetWeatherData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseWeatherData(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"API 요청 실패: {request.error}");
            }
        }
    }

    void ParseWeatherData(string json)
    {
        WeatherData weatherData = JsonUtility.FromJson<WeatherData>(json);

        // 데이터 갱신
        temperature = weatherData.main.temp;
        weatherStatus = weatherData.weather[0].main;
        snowfall = weatherData.snow?.h1 ?? 0f;

        Debug.Log($"온도: {temperature}°C, 적설량: {snowfall}mm, 날씨 상태: {weatherStatus}");

        // 날씨 데이터를 UI에 업데이트
        UpdateWeatherText(); // <- 추가된 부분
    }

    void UpdateWeatherText()
    {
        // Text UI에 모든 날씨 정보 표시
        if (weatherText != null)
        {
            weatherText.text = $"Temperature: {temperature}°C\n" +
                               $"Snowfall: {snowfall} mm\n" +
                               $"Weather Status: {weatherStatus}";
        }
        else
        {
            Debug.LogError("WeatherText UI가 설정되지 않았습니다.");
        }
    }

    // JSON 데이터 클래스
    [System.Serializable]
    public class WeatherData
    {
        public Main main;
        public Weather[] weather;
        public Snow snow;
    }

    [System.Serializable]
    public class Main { public float temp; }

    [System.Serializable]
    public class Weather { public string main; }

    [System.Serializable]
    public class Snow { public float h1; }
}
