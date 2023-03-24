using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    /// <summary>
    /// 日夜交替的图
    /// </summary>
    public RectTransform dayNightImage;
    /// <summary>
    /// 时刻块
    /// </summary>
    public RectTransform clockParent;
    /// <summary>
    /// 季节图片的组件
    /// </summary>
    public Image seasonImage;
    /// <summary>
    /// 年月日Text
    /// </summary>
    public TextMeshProUGUI dateText;
    /// <summary>
    /// 时钟时间
    /// </summary>
    public TextMeshProUGUI timeText;

    /// <summary>
    /// 季节的图片
    /// </summary>
    public Sprite[] seasonSprites;
    /// <summary>
    /// 时刻块的库
    /// </summary>
    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        MyEvnetHandler.GameMinuteEvent += OnMinuteEvent;
        MyEvnetHandler.GameDateEvent += OnGameDateEvent;
    }
    private void OnDisable()
    {
        MyEvnetHandler.GameMinuteEvent -= OnMinuteEvent;
        MyEvnetHandler.GameDateEvent -= OnGameDateEvent;
    }
    private void OnMinuteEvent(int m, int h)
    {
        timeText.text = $"{h:00}:{m:00}";
    }
    
    private void OnGameDateEvent(int h, int d, int m, int y, Season season)
    {
        dateText.text = $"{y}年{m:00}月{d:00}日";
        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(h);
        DayNightImageRotate(h);
    }

    private void SwitchHourImage(int hour)
    {
        var currentIndex = hour / 4;
        if (currentIndex == 0 )
        {
            foreach (var clock in clockBlocks)
            {
                clock.SetActive(false);
            }
            clockBlocks.FirstOrDefault()?.SetActive(true);
        }
        else
        {
            for (int i = 0; i < clockBlocks.Count; i++)
            {
                if (i <= currentIndex)
                {
                    clockBlocks[i].SetActive(true);
                }
                else
                {
                    clockBlocks[i].SetActive(false);
                }
            }
        }
    }

    private void DayNightImageRotate(int hour)
    {
        var targetR = new Vector3(0, 0, hour * 15);
        dayNightImage.DORotate(targetR,1f,RotateMode.Fast);
    }

}
