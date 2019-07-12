using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IslandMapsManager : MonoBehaviour {
    public enum EPlayerAnimation {
        Front,
        Back,
        Right,
        Left
    }

    public enum EMovementState {
        Idle,
        Moving
    }

    [Header("Configuration")]
    public Transform playerCharacter;
    public IslandData[] islands;
    public IslandPath[] islandPaths;

    [Header("User Interface")]
    public TextMeshProUGUI levelName;

    private int m_previousIslandIndex;
    private int m_currentIslandIndex = 0;
    private EMovementState m_currentMovementState;
    private Animator m_playerAnimator;

    // Animations
    private const string FRONT_ANIMATION = "Front";
    private const string BACK_ANIMATION = "Back";
    private const string SIDE_ANIMATION = "Side";

    // each island should have => name, position, first time or not, finished or not

    private void Awake() {
        m_playerAnimator = playerCharacter.GetComponentInChildren<Animator>();
    }

    private void Start() {
        m_currentMovementState = EMovementState.Idle;
    }

    private void Update() {
        if(m_currentMovementState == EMovementState.Moving) {
            return;
        }

        // Testing Movements...
        if(Input.GetKeyDown(KeyCode.D)) {
            IslandData from = islands[m_currentIslandIndex];
            m_currentIslandIndex = Mathf.Min(m_currentIslandIndex + 1, islands.Length - 1);
            IslandData to = islands[m_currentIslandIndex];

            MovePlayer(from, to);
        } else if(Input.GetKeyDown(KeyCode.A)) {
            IslandData from = islands[m_currentIslandIndex];
            m_currentIslandIndex = Mathf.Max(0, m_currentIslandIndex - 1);
            IslandData to = islands[m_currentIslandIndex];

            MovePlayer(from, to);
        }

        m_previousIslandIndex = m_currentIslandIndex;
    }

    private void MovePlayer(IslandData _from, IslandData _to) {
        // Updating Level Name on UI
        levelName.text = _to.islandName;

        m_currentMovementState = EMovementState.Moving;
        IslandPath[] desiredPaths = islandPaths.Where((path) => {
            return (path.origin == _from.island && path.destination == _to.island);
        }).ToArray();

        if(desiredPaths.Length == 0) {
            m_currentMovementState = EMovementState.Idle;
            m_currentIslandIndex = m_previousIslandIndex;
            return;
        }

        IslandPath desiredPath = desiredPaths.First();

        Debug.Log($"Desired Path: {desiredPath}");

        List<Transform> pathToFollow = desiredPath.path.ToList();
        pathToFollow.Add(_to.playerPositionOnIsland);

        PlayPlayerAnimation(desiredPath.playerAnimation);

        StartCoroutine(MovePlayerOnMarkersRoutine(pathToFollow.ToArray()));
    }

    private void PlayPlayerAnimation(EPlayerAnimation _animation) {
        switch(_animation) {
            case EPlayerAnimation.Front:
                m_playerAnimator.Play(FRONT_ANIMATION);
                break;
            case EPlayerAnimation.Back:
                m_playerAnimator.Play(BACK_ANIMATION);
                break;
            case EPlayerAnimation.Right:
                m_playerAnimator.Play(SIDE_ANIMATION);
                playerCharacter.localScale = new Vector3(Mathf.Abs(playerCharacter.localScale.x), playerCharacter.localScale.y, playerCharacter.localScale.z);
                break;
            case EPlayerAnimation.Left:
                m_playerAnimator.Play(SIDE_ANIMATION);
                playerCharacter.localScale = new Vector3(-Mathf.Abs(playerCharacter.localScale.x), playerCharacter.localScale.y, playerCharacter.localScale.z);
                break;
        }
    }

    private IEnumerator MovePlayerOnMarkersRoutine(Transform[] _markers) {
        float DELAY_TIME = 0.5f;

        foreach(Transform destination in _markers) {
            Vector2 playerOrigin = playerCharacter.position;
            Vector2 playerDestination = destination.position;

            for(float i = 0f; i < DELAY_TIME; i += Time.deltaTime) {
                float t = Mathf.Clamp01(i / DELAY_TIME);
                playerCharacter.position = Vector2.Lerp(playerOrigin, playerDestination, t);
                yield return null;
            }

            playerCharacter.position = playerDestination;
        }

        PlayPlayerAnimation(EPlayerAnimation.Front);
        m_currentMovementState = EMovementState.Idle;
    }
}
