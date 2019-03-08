using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileUI : MonoBehaviour {
    public GameObject emptySaveGameObject;
    public GameObject hasSaveGameObject;
    public int slotNumber = -1;

    [Header("Saved Game References")]
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI timePlayedText;
    public TextMeshProUGUI pizzasText;

    public void UpdateProfileUI(bool hasSaveData, MySaveData saveData) {
        if(hasSaveData) {
            hasSaveGameObject.SetActive(true);
            emptySaveGameObject.SetActive(false);

            int minutesPlayed = Mathf.FloorToInt(saveData.timePlayed / 60);
            int hoursPlayed = Mathf.FloorToInt(minutesPlayed / 60);
            int minutesRemainder = Mathf.CeilToInt(minutesPlayed % 60);


            deathsText.text = "x" + saveData.Deaths;
            timePlayedText.text = hoursPlayed + "h" + minutesRemainder + "min";
            pizzasText.text = "x" + saveData.pizzaCounter;
        } else {
            hasSaveGameObject.SetActive(false);
            emptySaveGameObject.SetActive(true);
        }
    }
}
