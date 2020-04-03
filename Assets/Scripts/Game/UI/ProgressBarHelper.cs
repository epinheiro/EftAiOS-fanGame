using UnityEngine;
using UnityEngine.UI;

public class ProgressBarHelper : UIHelper
{
    readonly Slider slider;

    public ProgressBarHelper(GameObject uiObject) : base(uiObject){
        slider = uiObject.GetComponent<Slider>();
    }

    public void SetValues(int currentValue, int? maxValue = null){
        slider.value = currentValue;
        if(maxValue.HasValue) slider.maxValue = (int) maxValue;
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;

        slider.value = 0;
        slider.maxValue = 39;
    }
}