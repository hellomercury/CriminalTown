using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NightButton : MonoBehaviour
{
    public Image map;
    Color defaultMapColor;
    public GameObject nightAnimation;

    private float animationTime = 2;
    private float animationTimer = 0;


    public void OnNightButtonClick()
    {
       Night.Instance.TryToStartNight();
    }

//    IEnumerator NightAnimation()
//    {
//        nightAnimation.SetActive(true);
//
//        animationTimer = 0;
//        defaultMapColor = map.color;
//        while (animationTimer < 1)
//        {
//            animationTimer += Time.deltaTime / animationTime;
//            map.color = Color.Lerp(defaultMapColor, Color.black, animationTimer);
//            yield return null;
//        }
//
//        nightAnimation.SetActive(false);
//        yield return new WaitForSeconds(eventTime);
//
//        StartCoroutine(NightEvents());
//        yield break;
//    }



//    IEnumerator DayAnimation()
//    {
//        nightAnimation.SetActive(true);
//
//        animationTimer = 0;
//        while (animationTimer < 1)
//        {
//            animationTimer += Time.deltaTime / animationTime;
//            map.color = Color.Lerp(Color.black, defaultMapColor, animationTimer);
//            yield return null;
//        }
//
//        nightAnimation.SetActive(false);
//
//        yield return new WaitForSeconds(eventTime);
//
//
//        FinishNight();
//        yield break;
//    }





}
