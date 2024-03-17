using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSweeperStarter : MonoBehaviour
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
                if (Singleton<MineSweeperManager>.Instance == null)
                {
                    GetComponent<BasePart>().gameObject.AddComponent<MineSweeperManager>();
                }
                Singleton<MineSweeperManager>.Instance.InitializeGame(10,10,20,(int)transform.position.x,(int)transform.position.y);
            }
        }
    }
}
