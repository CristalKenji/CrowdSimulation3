using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScene : MonoBehaviour
{
    // Start is called before the first frame update

    public bool KeepSceneViewActive;

    void Start()
    {
        if (this.KeepSceneViewActive && Application.isEditor)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
