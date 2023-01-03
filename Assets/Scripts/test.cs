using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	
    public QTEEvent q;
    public QTEManager qm;
    
    
    // Start is called before the first frame update
    void Start()
    {
        qm.startEvent(q);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !qm.isEventStarted)
        {
            qm.startEvent(q);
        }
    }
}
