using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileUI : MonoBehaviour {
    public GameObject emptySaveGameObject;
    public GameObject hasSaveGameObject;

    [Header("Saved Game References")]
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI timePlayedText;
    public TextMeshProUGUI pizzasText;
    public void UpdateProfileUI(bool hasSaveData, MySaveData saveData) {
        Debug.LogWarningFormat("Has Save Data: {0}", hasSaveData);
        if(hasSaveData) {
            hasSaveGameObject.SetActive(true);
            emptySaveGameObject.SetActive(false);

            deathsText.text = "Deaths: " + saveData.Deaths;
            timePlayedText.text = saveData.timePlayed.ToString();
            pizzasText.text = "Pizzas: " + saveData.pizzaCounter;
        } else {
            hasSaveGameObject.SetActive(false);
            emptySaveGameObject.SetActive(true);
        }
    }
}
