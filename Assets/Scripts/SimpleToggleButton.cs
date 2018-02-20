using UnityEngine;

using System.Collections;

public class SimpleToggleButton : MonoBehaviour

{


    bool pressed = true;

    void OnGUI()

    {
        pressed = GUILayout.Toggle(pressed, "Toggle me !", "Button");
        
    }

}