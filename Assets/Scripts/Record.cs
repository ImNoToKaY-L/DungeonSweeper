using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record 
{
    public static String path = Application.persistentDataPath + "/Record.txt";
    public int stepTaken;
    public int scanUsed;
    public int interceptionTriggered;

}
