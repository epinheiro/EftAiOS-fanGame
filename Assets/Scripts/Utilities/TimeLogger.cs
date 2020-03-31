using UnityEngine;

public class TimeLogger : Debug
{
    static public void Log(string formatString, params object[] formatArgs){
        string message = string.Format(formatString, formatArgs);
        Debug.Log(string.Format("{0} - {1}", System.DateTime.Now, message));
    }
}
