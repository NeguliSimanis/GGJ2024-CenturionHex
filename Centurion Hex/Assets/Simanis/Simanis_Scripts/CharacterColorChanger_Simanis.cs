using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColorChanger_Simanis : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material team0Material;
    public Material team1Material;

    public void ChangeColor(int teamID)
    {
        if (teamID == 0)
            spriteRenderer.material = team0Material;
        else
            spriteRenderer.material = team1Material;
    }
}
