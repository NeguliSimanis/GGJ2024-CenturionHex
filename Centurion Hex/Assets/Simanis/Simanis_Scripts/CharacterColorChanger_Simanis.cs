using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColorChanger_Simanis : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material team0Material;
    public Material team1Material;
    public Material greyMaterial;

    public void ChangeColor(int teamID)
    {
        if (teamID == 0)
            spriteRenderer.material = team0Material;
        else
            spriteRenderer.material = team1Material;
    }

    /// <summary>
    /// used to mark ally war units as inactive during governor turn and vice versa
    /// </summary>
    /// <param name="color"></param>
    public void ColorGrey(int teamID, bool color = true)
    {
        if (color)
        {
            //Debug.Log("COLORING ME GREY");
            spriteRenderer.color = Color.black;
            spriteRenderer.material = greyMaterial;
        }
        else
        {
            //Debug.Log("COLORING ME MNOT GREY");
            ChangeColor(teamID); spriteRenderer.color = Color.white;
        }
    }
}
