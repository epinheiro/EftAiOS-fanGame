using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlienFeedbackGroupHelper : UIHelper
{
    readonly Image leftTop;
    readonly Image leftBottom;
    readonly Image rightTop;
    readonly Image rightBottom;

    ExtendedList<Image> shuffleList;

    public AlienFeedbackGroupHelper(GameObject uiObject) : base(uiObject){
        leftTop = uiObject.transform.Find("TopLeft").GetComponent<Image>();
        leftBottom = uiObject.transform.Find("BottomLeft").GetComponent<Image>();
        rightTop = uiObject.transform.Find("TopRight").GetComponent<Image>();
        rightBottom = uiObject.transform.Find("BottomRight").GetComponent<Image>();

        EnableImage(false);

        shuffleList = new ExtendedList<Image>();
        shuffleList.Add(leftTop);
        shuffleList.Add(leftBottom);
        shuffleList.Add(rightTop);
        shuffleList.Add(rightBottom);
    }

    void EnableImage(bool allImages){
        leftTop.enabled = allImages;
        leftBottom.enabled = allImages;
        rightTop.enabled = allImages;
        rightBottom.enabled = allImages;
    }

    void EnableImage(bool topLeft, bool topRight, bool bottomLeft, bool bottomRight){
        leftTop.enabled = topLeft;
        leftBottom.enabled = topRight;
        rightTop.enabled = bottomLeft;
        rightBottom.enabled = bottomRight;
    }

    public void SetAlienFeedbacks(){
        EnableImage(false);
    }

    public void SetAlienFeedbacks(List<Color> colors){
        EnableImage(false);

        shuffleList.Shuffle();

        for( int i=0; i<colors.Count; i++){
            shuffleList[i].enabled = true;
            shuffleList[i].color = colors[i];
        }
    }
}
