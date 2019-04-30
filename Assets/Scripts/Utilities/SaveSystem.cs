using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;


public class SaveSystem : MonoBehaviour
{   
    public static SaveSystem instance;
    public MySaveData myData;
    [HideInInspector]
    public int currentSlot = -1;
    private GameObject player;
    private float m_tempTime = 0;

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
        XmlSerializer serializer = new XmlSerializer(typeof(MySaveData));;
        using (StringWriter sw = new StringWriter()) {
            serializer.Serialize(sw,myData);
            PlayerPrefs.SetString("slot_" + slot,sw.ToString());
            PlayerPrefs.SetInt("slot_" + slot + "_isBeingUsed",1);
        }
        
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

        XmlSerializer serializer = new XmlSerializer(typeof(MySaveData));;
        using (StringWriter sw = new StringWriter()) {
            serializer.Serialize(sw,myData);
            PlayerPrefs.SetString("slot_" + currentSlot,sw.ToString());
            PlayerPrefs.SetInt("slot_" + currentSlot + "_isBeingUsed",1);
        }
        
        PlayerPrefs.Save();
    }

    /// <summary>
    /// This function saves the game in the current slot in the next frame
    /// </summary>

    public void UISaveGameNextFrame(){
        StartCoroutine(SaveNextFrameCoroutine());
    }

    public void UILoadGame(int slot) {
        if(!PlayerPrefs.HasKey("slot_" + slot + "_isBeingUsed")) {
            Debug.LogError("SaveSystem: Can't load, the given slot doesn't exist.");
            return;
        }
        if(PlayerPrefs.GetInt("slot_" + slot + "_isBeingUsed") == 0) {
            Debug.LogError("SaveSystem: Can't load, the given slot isn't in use.");
            return;
        }
        currentSlot = slot;

        XmlSerializer serializer = new XmlSerializer(typeof(MySaveData));
       
        using (StringReader reader = new StringReader(PlayerPrefs.GetString("slot_" + slot))) {
            myData = serializer.Deserialize(reader) as MySaveData;
        } 

        if(myData.isInStage) {
            LevelManagement.LevelManager.instance.LoadLevel(myData.lastStage,false);
        } else {
            LevelManagement.LevelManager.instance.LoadLevel(2,false);
        }
    }

    public void UIResetGame(int slot) {
        myData.Reset();
        UISaveGame(slot);
        PlayerPrefs.SetInt("slot_" + slot + "_isBeingUsed",0);
        PlayerPrefs.Save();
    }

    public void UIResetAllSaves() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void UICreateSaveSlot(int slot) {
        if(PlayerPrefs.HasKey("slot_" + slot + "_isBeingUsed")) {
            if(PlayerPrefs.GetInt("slot_" + slot + "_isBeingUsed") == 1){
                Debug.LogError("SaveSystem: Can't create a save slot in this slot because it is already being used.");
                return;
            }
        }
        currentSlot = slot;
        myData.Reset();
        UISaveGame(slot);
    }

    /// <summary>
    /// This function is used to verify if a given slot is in use
    /// </summary>

    public bool UISlotIsInUse(int slot) {
        if(!PlayerPrefs.HasKey("slot_" + slot + "_isBeingUsed")) {
            return false;
        }
        if(PlayerPrefs.GetInt("slot_" + slot + "_isBeingUsed") == 0) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// This function is used to analyse the data from a save slot, without loading the game
    /// </summary>

    public MySaveData UIExtractInfo(int slot) {
        MySaveData ms = new MySaveData();
        if(!PlayerPrefs.HasKey("slot_" + slot + "_isBeingUsed")) {
            Debug.Log("SaveSystem: Can't extract info, the given slot doesn't exist.");
            ms.Reset();
            return ms;
        }
        if(PlayerPrefs.GetInt("slot_" + slot + "_isBeingUsed") == 0) {
            Debug.Log("SaveSystem: Can't extract info, the given slot isn't in use.");
            ms.Reset();
            return ms;
        }
        XmlSerializer serializer = new XmlSerializer(typeof(MySaveData));
       
        using (StringReader reader = new StringReader(PlayerPrefs.GetString("slot_" + slot))) {
            ms = serializer.Deserialize(reader) as MySaveData;
        } 

        return ms;
    }

    public void UpdateSectionTime() {
        int Index = SceneManager.GetActiveScene().buildIndex;

        if(myData.LowestTime[Index] > m_tempTime) {
            myData.LowestTime[Index] = m_tempTime; // this code section saves the ammount of time that the player took to beat a level
        }

        m_tempTime = 0;
    }

    public void ResetSectionTime() {
        m_tempTime = 0;
    }

    public void OnLevelEnter(int level) {
        SetIsInStage(true);
        SetLastStage(level);        
    }

    public void OnLevelExit() {
        SetIsInStage(false);
    }

    void OnApplicationQuit() {
        if(currentSlot != -1) {
            GameObject m_check = GameObject.FindGameObjectWithTag("Checkpoint System");
            if(m_check) {
                m_check.GetComponent<CheckpointSystemBehavior>().RemovePizzaCounters();
            }
            UISaveGame();
        }
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
        m_tempTime += Time.deltaTime;
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

    public void NPCChatted() {
        myData.NPCChat++;
    }
}