using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager_Simanis : MonoBehaviour
{
    public float moveSpeed = 2f;

    public CenturionGame centurionGame;
    public TileSpawner_Simanis spawner;

    private void Start()
    {
        centurionGame = GetComponent<CenturionGame>();
        spawner = GetComponent<TileSpawner_Simanis>();
    }

    public void MoveCharacter()
    {
        Debug.Log("LOOKIN FOR MOVE CHAACTER");
        Character charToMove = centurionGame.lastCharacterMoved;
        foreach(CharacterVisual_Simanis charVisual in spawner.allCharacters)
        {
            if (charVisual.character == charToMove)
            {
                charVisual.MoveCharacter(moveSpeed);
            }
        }
    }
}
