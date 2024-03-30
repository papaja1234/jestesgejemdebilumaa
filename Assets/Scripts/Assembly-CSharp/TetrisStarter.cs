using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisStarter : MonoBehaviour
{
    private bool init = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            if (GetComponent<BasePart>().contraption.IsRunning)
            {
                init = true;
                if (Singleton<TetrisManager>.Instance == null)
                {
                    GetComponent<BasePart>().gameObject.AddComponent<TetrisManager>();
                }
                Singleton<TetrisManager>.Instance.Init(this.transform.position);
            }
        }
    }
}
