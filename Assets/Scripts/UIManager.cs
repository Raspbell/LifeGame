using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    private Slider slider;
    public TextMeshProUGUI text;
    public LifeGame lifeGame;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value => {
            ChangeInterval(text, lifeGame);
        });
    }

    public void ChangeInterval(TextMeshProUGUI text, LifeGame lifeGame)
    {
        float value = slider.value;
        text.text = value.ToString("0.00");
        lifeGame.interval = value;
    }
}
