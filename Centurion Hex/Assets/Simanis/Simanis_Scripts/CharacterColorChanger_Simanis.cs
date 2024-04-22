using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterColorChanger_Simanis : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material team0Material;
    public Material team1Material;
    public Material greyMaterial;

    public void ChangeColor(int teamID)
    {
        if (teamID == 0)
        {
            Debug.Log("coloring me yellow");
            spriteRenderer.material = team0Material;
        }
        else
        {
            Debug.Log("coloring me grey");
            spriteRenderer.material = team1Material;
        }
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

    public void FadeToTransparent(float fadeDuration)
    {
        Color transparent = new Color(1, 1, 1, 0);
        Color currColor = spriteRenderer.color;
        float startAlpha = currColor.a;
        DOTween.To(() => startAlpha, x => startAlpha = x, 0, fadeDuration)
            .OnUpdate(()=>
            {
               // Debug.Log("fading to transparent  " + startAlpha);
                currColor.a = startAlpha;
                spriteRenderer.color = currColor;
            }).
            OnComplete(()=>
            {
                currColor.a = 0f;
                spriteRenderer.color = currColor;
            });
    }

}
