using UnityEngine;

public class CameraController
{
    public static Camera _cameraComponent;
    public static Camera CameraComponent
    {
        get
        {
            if(_cameraComponent == null)
                _cameraComponent = Camera.main.GetComponent<Camera>();
            return _cameraComponent;
        }
    }

    static public Color GetBackgroundColor(){
        return CameraComponent.backgroundColor;
    }

    static public void ChangeCameraBackgroundColor(Color newColor){
        CameraComponent.backgroundColor = newColor;
    }

    static public void ChangeCameraBackgroundColor(float r, float g, float b, float a){
        ChangeCameraBackgroundColor(new Color(r,g,b,a));
    }

    static public System.Collections.IEnumerator BackgroundChangeCoroutine(Color start, Color finish){
        int steps = 100;

        float rStep = (finish.r - start.r)/steps;
        float gStep = (finish.g - start.g)/steps;
        float bStep = (finish.b - start.b)/steps;

        Color current = start;

        for( int i=0 ; i<(steps-1) ; i++){
            start.r += rStep;
            start.g += gStep;
            start.b += bStep;

            CameraController.ChangeCameraBackgroundColor(start);

            yield return new WaitForSeconds(.01f);
        }

        CameraController.ChangeCameraBackgroundColor(finish);
    }
}
