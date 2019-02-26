using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{   
    public static SaveSystem instance;
    public MySaveData myData;
    [HideInInspector]
    public int currentSlot = -1;
    private GameObject player;

    void Start() {
        if(instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        myData = new MySaveData();
        myData.Reset();
        DontDestroyOnLoad(gameObject);
    }


    private void UISaveGame(int slot) {
        if(slot <= -1) {
            Debug.LogError("SaveSystem: Invalid given save slot, you cant save a game on a negative value slot.");
            return;
        }
        PlayerPrefs.SetInt(slot + "_isBeingUsed",1);
        PlayerPrefs.SetInt(slot + "_Jumps",myData.Jumps);
        PlayerPrefs.SetInt(slot + "_Deaths",myData.Deaths);
        PlayerPrefs.SetFloat(slot + "_timePlayed",myData.timePlayed);
        PlayerPrefs.SetInt(slot + "_pizzaCounter",myData.pizzaCounter);
        PlayerPrefs.SetInt(slot + "_lastStage",myData.lastStage);
        PlayerPrefs.SetInt(slot + "_isInStage",myData.isInStage ? 1 : 0);
        PlayerPrefs.SetFloat(slot + "_positionInStage_x",myData.positionInStage.x);
        PlayerPrefs.SetFloat(slot + "_positionInStage_y",myData.positionInStage.y);
        PlayerPrefs.SetFloat(slot + "_positionInStage_z",myData.positionInStage.z);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// This function saves the game in the current slot
    /// </summary>

    public void UISaveGame() {
        if(currentSlot == -1) {
            Debug.LogError("SaveSystem: Invalid current save slot, are you trying to save a game without loading a game?");
            return;
        }
        PlayerPrefs.SetInt(currentSlot + "_isBeingUsed",1);
        PlayerPrefs.SetInt(currentSlot + "_Jumps",myData.Jumps);
        PlayerPrefs.SetInt(currentSlot + "_Deaths",myData.Deaths);
        PlayerPrefs.SetFloat(currentSlot + "_timePlayed",myData.timePlayed);
        PlayerPrefs.SetInt(currentSlot + "_pizzaCounter",myData.pizzaCounter);
        PlayerPrefs.SetInt(currentSlot + "_lastStage",myData.lastStage);
        PlayerPrefs.SetInt(currentSlot + "_isInStage",myData.isInStage ? 1 : 0);
        PlayerPrefs.SetFloat(currentSlot + "_positionInStage_x",myData.positionInStage.x);
        PlayerPrefs.SetFloat(currentSlot + "_positionInStage_y",myData.positionInStage.y);
        PlayerPrefs.SetFloat(currentSlot + "_positionInStage_z",myData.positionInStage.z);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// This function saves the game in the current slot in the next frame
    /// </summary>

    public void UISaveGameNextFrame(){
        StartCoroutine(SaveNextFrameCoroutine());
    }

    public void UILoadGame(int slot) {
        if(!PlayerPrefs.HasKey(slot + "_isBeingUsed")) {
            Debug.LogError("SaveSystem: Can't load, the given slot doesn't exist.");
        }
        if(PlayerPrefs.GetInt(slot + "_isBeingUsed") == 0) {
            Debug.LogError("SaveSystem: Can't load, the given slot isn't in use.");
            return;
        }
        currentSlot = slot;
        myData.Jumps = PlayerPrefs.GetInt(currentSlot + "_Jumps");
        myData.Deaths = PlayerPrefs.GetInt(currentSlot + "_Deaths");
        myData.timePlayed = PlayerPrefs.GetFloat(currentSlot + "_timePlayed");
        myData.pizzaCounter = PlayerPrefs.GetInt(currentSlot + "_pizzaCounter");
        myData.lastStage = PlayerPrefs.GetInt(currentSlot + "_lastStage");
        myData.isInStage = PlayerPrefs.GetInt(currentSlot + "_isInStage") == 1;
        myData.positionInStage.x = PlayerPrefs.GetInt(currentSlot + "_positionInStage_x");
        myData.positionInStage.y = PlayerPrefs.GetInt(currentSlot + "_positionInStage_y");
        myData.positionInStage.z = PlayerPrefs.GetInt(currentSlot + "_positionInStage_z");
        if(myData.isInStage) {
            LevelManagement.LevelManager.instance.LoadLevel(myData.lastStage);
            GameObject.FindGameObjectWithTag("Checkpoint System").GetComponent<CheckpointSystemBehavior>().transform.position = myData.positionInStage;
        } else {
            LevelManagement.LevelManager.instance.LoadLevel("Hub");
        }
    }

    public void UIResetGame(int slot) {
        myData.Reset();
        UISaveGame(slot);
        PlayerPrefs.SetInt(slot + "_isBeingUsed",0);
        PlayerPrefs.Save();
    }

    public void UIResetAllSaves() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void UICreateSaveSlot(int slot) {
        if(PlayerPrefs.HasKey(slot + "_isBeingUsed")) {
            if(PlayerPrefs.GetInt(slot + "_isBeingUsed") == 1){
                Debug.LogError("SaveSystem: Can't create a save slot in this slot because it is already being used.");
                return;
            }
        }
        myData.Reset();
        UISaveGame(slot);
    }

    /// <summary>
    /// This function is used to verify if a given slot is in use
    /// </summary>

    public bool UISlotIsInUse(int slot) {
        if(!PlayerPrefs.HasKey(slot + "_isBeingUsed")) {
            return false;
        }
        if(PlayerPrefs.GetInt(slot + "_isBeingUsed") == 0) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// This function is used to analyse the data from a save slot, without loading the game
    /// </summary>

    public MySaveData UIExtractInfo(int slot) {
        MySaveData ms = new MySaveData();
        ms.Jumps = PlayerPrefs.GetInt(slot + "_Jumps");
        ms.Deaths = PlayerPrefs.GetInt(slot + "_Deaths");
        ms.timePlayed = PlayerPrefs.GetFloat(slot + "_timePlayed");
        ms.pizzaCounter = PlayerPrefs.GetInt(slot + "_pizzaCounter");
        ms.lastStage = PlayerPrefs.GetInt(slot + "_lastStage");
        ms.isInStage = PlayerPrefs.GetInt(slot + "_isInStage") == 1;
        ms.positionInStage.x = PlayerPrefs.GetInt(slot + "_positionInStage_x");
        ms.positionInStage.y = PlayerPrefs.GetInt(slot + "_positionInStage_y");
        ms.positionInStage.z = PlayerPrefs.GetInt(slot + "_positionInStage_z");
        return ms;
    }


    public void OnLevelEnter(int level) {
        SetLastStage(level);
        SetIsInStage(true);
    }

    public void OnLevelExit() {
        SetIsInStage(false);
        SetPositionInStage(Vector3.zero);
    }

    void OnApplicationQuit() {
        if(currentSlot != -1)
            UISaveGame();
    }

    IEnumerator SaveNextFrameCoroutine() {
        yield return null;
        UISaveGame();
    }

    // ----------------------------
    // This point onward is used in-game to update the MySaveData instance

    public void Jumped() {
        myData.Jumps++;
    }
    public void Died() {
        myData.Deaths++;
    }
    public void TickTimePlayed() {
        myData.timePlayed += Time.deltaTime;
    }

    public void GotPizza() {
        myData.pizzaCounter++;
    }

    public void RemovePizza() {
        myData.pizzaCounter--;
    }

    public void SetLastStage(int stage) {
        myData.lastStage = stage;
    }

    public void SetIsInStage(bool isInStage) {
        myData.isInStage = isInStage;
    }

    public void SetPositionInStage(Vector3 position) {
        myData.positionInStage = position;
    }
}