using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider sliderMeter;
    public Slider sliderDamage;
    public Image healthImage;
    public Image damageImage;
    public float reductionAmount;           //controls how fast damage fill depletes


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //adjust damage gauge
        StartCoroutine(ReduceDamageBar());
    }

    IEnumerator ReduceDamageBar()
    {
        while (sliderDamage.value > sliderMeter.value)
        {
            sliderDamage.value -= reductionAmount * Time.deltaTime;
            yield return null;
        }
    }

    public void AdjustMeter(float amount)
    {
        sliderMeter.value += amount;

        if (sliderMeter.value < sliderDamage.value)  //if we took damage, do nothing more because couroutine will handle damage bar value
            return;

        sliderDamage.value = sliderMeter.value;
    }

    public void SetMaxValue(float amount)
    {
        sliderMeter.maxValue = amount;
        sliderMeter.value = sliderMeter.maxValue;
        sliderDamage.maxValue = sliderMeter.maxValue;
        sliderDamage.value = sliderMeter.value;
    }
}
