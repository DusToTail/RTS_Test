using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// English: Visualize the changes in progress in the slider component used.
/// 日本語：進捗の変化をＳｌｉｄｅｒコンポーネントで表す。
/// </summary>
public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public IBuild produce;

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
