using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI progressText;

    public void UpdateProgressText(float value) {
        progressText.text = (Mathf.RoundToInt(value * 100) + "%");
    }
}
