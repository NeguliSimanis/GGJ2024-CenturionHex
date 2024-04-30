using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SP_JewelAnimation : MonoBehaviour
{

    public static SP_JewelAnimation instance;

    public GameObject jewelAnimPrefab;
    public float animationDistance = 190f;
    public float moveAnimDuration = 3f;
    public Ease moveEase;

    public float noFadeDuration = 2f;
    

    private void Awake()
    {
        instance = this;
    }

    public void ShowJewelAnimation(GameObject position, bool show = true)
    {
        if (show)
        {
            GameObject newAnim = Instantiate(jewelAnimPrefab, this.transform);
            newAnim.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, position.transform.TransformPoint(Vector3.zero));

            Image jewelImage = newAnim.GetComponent<Image>();
            jewelImage.color = new Color(1, 1, 1, 0);
            jewelImage.DOFade(1, 0.2f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(noFadeDuration, () =>
                 {
                     jewelImage.DOFade(0, 0.2f).OnComplete(() =>
                     {

                     });
                 });
            });

            Vector3 targetPos= newAnim.transform.position;
            targetPos.y += animationDistance;
            newAnim.transform.DOMove(targetPos, moveAnimDuration).SetEase(moveEase).OnComplete(()=> {
                Destroy(newAnim);
            
            });
        }
        
    }

    
}
