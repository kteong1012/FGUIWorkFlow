using MyUnityProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FGUIManager.Instance.Open<UILoginPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
