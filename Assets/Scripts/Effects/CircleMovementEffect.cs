using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from http://answers.unity.com/answers/596698/view.html
public class CircleMovementEffect : MonoBehaviour
{
    [Range(-360,360)] public float beginningAngle = 0;
    [Range(0, 60)] public float timeToCompleteCircle = 5;
    [Range(0, 10)] public float circleRadius = 5;

    float speed;

    Vector3 initialPosition;

    void Start(){
        initialPosition = transform.localPosition;
        speed = (2*Mathf.PI)/timeToCompleteCircle;
    }

    void Update()
    {
        beginningAngle += speed*Time.deltaTime; //if you want to switch direction, use -= instead of +=
        float x = Mathf.Cos(beginningAngle)*circleRadius;
        float y = Mathf.Sin(beginningAngle)*circleRadius;
        transform.localPosition = initialPosition + new Vector3(x, y);
    }
}
