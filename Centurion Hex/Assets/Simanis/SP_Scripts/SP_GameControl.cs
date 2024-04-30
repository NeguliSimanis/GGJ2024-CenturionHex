using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum VictoryCondition
{
    kill_all_enemies,
    find_jewel
}

public enum DefeatCondition
{
    lose_all_units,
    pricess_dies
}

public class SP_GameControl : MonoBehaviour
{
    public static SP_GameControl instance;

    [Header("MISSION SETUP")]
    public Mission mission;
    public VictoryCondition victoryCondition;
    public DefeatCondition [] defeatConditions;
    private bool isMissionOver = false;
    private bool isMissionStarted = false;

    [Header("MAP BALANCE")]
    public float slow_tile_chance = 0.23f;
    public bool map_has_random_mines = true;
    public bool map_has_random_gold = false;
    public float tile_has_something_chance = 0.32f;
    public float tile_something_is_landmine_chance = 0.32f;

    [Header("OTHER CONTROLLERS")]
    public SP_HUD_Control hudControl;
    public CustomCursor_Simanis customCursor;
    public SP_RaycastControl raycastControl;
    [HideInInspector] public bool allowRaycastInteract = true;

    [Header("UNITS")]
    public Transform predefinedUnitParent;
    [HideInInspector] public List<SP_Unit> allUnits;
    [HideInInspector] public SP_Unit prevSelectedUnit = null;
    [HideInInspector] public SP_Unit prevSelectedAllyUnit = null;
    public int livingAllies = 0;

    [Header("BUILDINGS")]
    [HideInInspector] public SP_Building prevSelectedBuilding = null;

    [Header("TURN MANAGEMENT")]
    public float enemyTurnDuration = 5f;
    [HideInInspector] public float enemyTurnStartTime;
    [HideInInspector] public float enemyTurnEndTime;
    public bool isAllyTurn = true;

    [Header("UI")]
    public AnnouncementUi_Simanis announcementControl;
    public GameObject endTurnButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SP_MapControl.instance.InitializeMap();
        announcementControl.gameObject.SetActive(true);
        announcementControl.HideAnnouncmentText(0);
        
    }

    public void StartMission()
    {
        isMissionStarted = true;
        InitializePredefinedUnits();
        hudControl.SetTurnInfoText();
        MarkInactiveUnits();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ReloadScene();
        }

        if (!isMissionStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenuScene();
        }

        if (!isMissionStarted && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SP_MissionBriefPanel.instance.ShowPopup(false);
            StartMission();
        }    

        if (!isMissionStarted)
            return;

        if (isMissionOver)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                LoadNextMission();
            }
        }

        if (isAllyTurn && Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectEverything();
        }

        if (isMissionOver && Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenuScene();
        }

        if (!isAllyTurn)
        {
            if (Time.time >= enemyTurnEndTime)
            {
                EndTurn();
            }
        }
    }

    public void LoadNextMission()
    {
        string nextSceneName = SP_MissionDescriptions.GetNextMissionSceneName(mission);
        SceneManager.LoadScene(nextSceneName);
    }

    public void DeselectEverything()
    {
        Debug.Log("deselecting everything");
        allowRaycastInteract = false;
        if (raycastControl.previousRaycast != null)
        {
            Debug.Log("prev raycast is not null");
            raycastControl.previousRaycast.ShowClickObj(false);
            raycastControl.previousRaycast.HighlightThis(false);
            raycastControl.previousRaycast = null;
            //Debug.Log("yes im null " + raycastControl.previousRaycast.gameObject.name);
        }
        if (prevSelectedUnit != null)
        {
            Debug.Log("prev unit is not null");
            prevSelectedUnit.SelectUnit(false);
        }
        if (prevSelectedBuilding != null)
        {
            prevSelectedBuilding.SelectBuilding(false);
        }
        customCursor.SetCursor(false, cursorAction: CursorAction.walk);
        allowRaycastInteract = true;
    }

    private void InitializePredefinedUnits()
    {
        allUnits = new List<SP_Unit>();
        foreach (Transform child in predefinedUnitParent)
        {
            SP_Unit newUnit = child.GetComponent<SP_Unit>();
            allUnits.Add(newUnit);
        }
        foreach (SP_Unit foundUnit in allUnits)
        {
            // dont spawn landmines and other stuff on tiles where units will be placed
            SP_MapControl.instance.GetTile(foundUnit.x, foundUnit.y).allowSpawnStuffOnTile = false;
            foundUnit.MoveUnit(foundUnit.x, foundUnit.y, instant: true);

            if(!foundUnit.isAllyUnit)
            {
                SP_EnemyUnitAI enemyControl = foundUnit.gameObject.AddComponent<SP_EnemyUnitAI>();
                enemyControl.myBehaviour = UnitBehaviour.RandomMoveAttackWhenAdjacent;
                enemyControl.myUnit = foundUnit;
                foundUnit.aiControl = enemyControl;
            }
            foundUnit.InitializeUnit();
        }
        UpdateUnitStats();
    }

    public void EndTurn()
    {
        isAllyTurn = !isAllyTurn;
        hudControl.SetTurnInfoText();
        UpdateUnitStats();
        DeselectEverything();
        if (!isAllyTurn)
        {
            endTurnButton.SetActive(false);
            announcementControl.ShowAnnouncmentText(
                bigAnnounce: "Enemy Turn",
                smallAnnounce: "",
                appearDuration: 0.8f,
                disappearDelay: 1.8f,
                disappearDuration: 1f);
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.enemyTurnSFX, isVoiceLine: true);
            hudControl.ShowTurnDuration(true);
            enemyTurnStartTime = Time.time;
            enemyTurnEndTime = Time.time + enemyTurnDuration;


            DOVirtual.DelayedCall(0.6f, () =>
            {
                MoveNextEnemy();
            });
        }

        // ALLY  TURN
        else
        {
            endTurnButton.SetActive(true);
            if (prevSelectedAllyUnit != null)
                prevSelectedAllyUnit.SelectUnit(true);
            announcementControl.ShowAnnouncmentText(
                bigAnnounce: "Your Turn",
                smallAnnounce: "",
                appearDuration: 0.8f,
                disappearDelay: 1.8f,
                disappearDuration: 1f);
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.yourTurnSFX, isVoiceLine: true);
            hudControl.ShowTurnDuration(false);
        }
    }

    public void MoveNextEnemy()
    {
        if (isAllyTurn)
            return;
        
        foreach(SP_Unit unit in allUnits)
        {
            if (!unit.isAllyUnit && !unit.aiControl.hasCompletedTurn)
            {
                unit.aiControl.DoMove();
                return;
            }
        }

        enemyTurnEndTime -= 0.0005f;
        MoveNextEnemy();
    }

    private void UpdateUnitStats()
    {
        foreach (SP_Unit unit in allUnits)
        {
            unit.ProcessNewTurnStart();

        }
    }

    public void ReloadScene()
    {
        // Get the current active scene's index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    private void MarkInactiveUnits()
    {
        foreach (SP_Unit unit in allUnits)
        {
            if (unit.isAllyUnit)
            {
                if (isAllyTurn)
                {
                    unit.MarkUnitAsSleeping(false);
                }
                else
                {
                    unit.MarkUnitAsSleeping(true);
                }
            }
            else // enemy turn
            {
                if (isAllyTurn)
                {
                    unit.MarkUnitAsSleeping(true);
                }
                else
                {
                    unit.MarkUnitAsSleeping(false);
                }
            }
        }
    }

    public void ProcessUnitDeath()
    {
        int livingAllies = 0;
        int livingEnemies = 0;
        foreach(SP_Unit unit in allUnits)
        {
            if (!unit.isAllyUnit && !unit.myStats.isDead)
            {
                livingEnemies++;
            }
            else if (unit.isAllyUnit && !unit.myStats.isDead)
            {
                livingAllies++;
            }
        }
        if (livingAllies <= 0)
        {
            EndMission(isVictory: false);
            return;
        }
        Debug.Log("living enemies remaining " + livingEnemies);
        if (livingEnemies <= 0)
        {
            if (victoryCondition == VictoryCondition.kill_all_enemies)
                EndMission(isVictory: true);
        }
    }

    public void EndMission(bool isVictory)
    {
        SP_GameOverPopup.instance.InitializePopup(isVictory);
        isMissionOver = true;
    }

    public void ProcessJewelPickup()
    {
        EndMission(isVictory: true);
    }
}
