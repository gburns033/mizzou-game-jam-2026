using UnityEngine;
using TMPro;

public class UITimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float time;

    void Update()
    {
        time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }
}