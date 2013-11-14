using UnityEngine;
using System.Collections;

public class ClickDetector : tk2dUIBaseItemControl
{
    void OnEnable()
    {
		uiItem.OnClick += DoDis;
    }

    void OnDisable()
    {
        uiItem.OnClick -= DoDis;
    }


    private void DoDis()
    {
		Debug.Log("Doin it");
    }
}
