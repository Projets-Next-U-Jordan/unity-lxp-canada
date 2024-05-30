using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class SC_CountdownTimer : MonoBehaviour
{

    public enum CountdownFormatting { DaysHoursMinutesSeconds, HoursMinutesSeconds, MinutesSeconds, Seconds };
    public CountdownFormatting countdownFormatting = CountdownFormatting.DaysHoursMinutesSeconds;
    public bool showMilliseconds = true;
    public double countdownTime = 600;

    double countdownInternal;
    bool countdownOver = false;
    public TextMeshProUGUI timerText;
    public SceneField sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        countdownInternal = countdownTime;
    }

    void Update() {
        if(countdownInternal > 0) {
            countdownInternal -= Time.deltaTime;
            if(countdownInternal < 0) {
                countdownInternal = 0;
            }
            timerText.text = FormatTime(countdownInternal, countdownFormatting, showMilliseconds);

            if (countdownInternal <= 15)
            {
                timerText.color = Color.Lerp(Color.white, Color.red, Mathf.Sin(Time.time * 5));
            }
        }
        else {
            if(!countdownOver) {
                countdownOver = true;
                Debug.Log("Countdown Over!");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

string FormatTime(double time, CountdownFormatting formatting, bool includeMilliseconds) {
    string timeText = "";

    int intTime = (int)time;
    int days = intTime / 86400;
    int hoursTotal = intTime / 3600;
    int hoursFormatted = hoursTotal % 24;
    int minutesTotal = intTime / 60;
    int minutesFormatted = minutesTotal % 60;
    int secondsTotal = intTime;
    int secondsFormatted = intTime % 60;
    int milliseconds = (int)(time * 100);
    milliseconds = milliseconds % 100;

     if (includeMilliseconds)
        {
            if (formatting == CountdownFormatting.DaysHoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}:{4:00}", days, hoursFormatted, minutesFormatted, secondsFormatted, milliseconds);
            }
            else if (formatting == CountdownFormatting.HoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", hoursTotal, minutesFormatted, secondsFormatted, milliseconds);
            }
            else if (formatting == CountdownFormatting.MinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}", minutesTotal, secondsFormatted, milliseconds);
            }
            else if (formatting == CountdownFormatting.Seconds)
            {
                timeText = string.Format("{0:00}:{1:00}", secondsTotal, milliseconds);
            }
        }
        else
        {
            if (formatting == CountdownFormatting.DaysHoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hoursFormatted, minutesFormatted, secondsFormatted);
            }
            else if (formatting == CountdownFormatting.HoursMinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}:{2:00}", hoursTotal, minutesFormatted, secondsFormatted);
            }
            else if (formatting == CountdownFormatting.MinutesSeconds)
            {
                timeText = string.Format("{0:00}:{1:00}", minutesTotal, secondsFormatted);
            }
            else if (formatting == CountdownFormatting.Seconds)
            {
                timeText = string.Format("{0:00}", secondsTotal);
            }
        }

        return timeText;
    }
}
