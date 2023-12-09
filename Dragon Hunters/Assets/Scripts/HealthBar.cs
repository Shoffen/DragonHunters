using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
   
    // Start is called before the first frame update
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);

    }
    public void ApplyDamage(int value)
    {
        if ((slider.value - value) <= 0)
        {
            slider.value = 0;
        }
        else
        {
            slider.value -= value;
        }
        
        fill.color = gradient.Evaluate(slider.normalizedValue);

    }
    
}
