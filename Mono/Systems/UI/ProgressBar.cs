using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public IProduce produce;

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeInHierarchy == false) { return; }
        if(produce == null) { return; }
        slider.minValue = 0;
        slider.maxValue = produce.maxProgressTime;
        slider.value = produce.curProgressTime;


    }
}
