using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

[CreateAssetMenu (fileName = "SoundSettings", menuName = "SoundSettings")]
public class SoundSettings: ScriptableObject {   
    [Header("SoundTracks")]

    [FMODUnity.EventRef]
    public string TitleScreen;
    [FMODUnity.EventRef]
    public string Hub;
    [FMODUnity.EventRef]
    public string University;
    [FMODUnity.EventRef]
    public string Arts;
    [FMODUnity.EventRef]
    public string Chemistry;
    [FMODUnity.EventRef]
    public string Philosophy;
    [FMODUnity.EventRef]
    public string ShibasTheme;

    [Space(5)]
    [Header("UI")]

    [FMODUnity.EventRef]
    public string Botton_click;
    [FMODUnity.EventRef]
    public string Cutscene_transition;

    [Space(5)]
    [Header("SFX")]

    [FMODUnity.EventRef]
    public string Player_walk;
    [FMODUnity.EventRef]
    public string Player_jump;
    [FMODUnity.EventRef]
    public string Player_walljump;
    [FMODUnity.EventRef]
    public string Player_death;
    [FMODUnity.EventRef]
    public string Player_reachGround;
    [FMODUnity.EventRef]
    public string Rock_fall;
    [FMODUnity.EventRef]
    public string Platform_break;
    [FMODUnity.EventRef]
    public string Collectable_continuous;
    [FMODUnity.EventRef]
    public string Collectable_pickup;
}