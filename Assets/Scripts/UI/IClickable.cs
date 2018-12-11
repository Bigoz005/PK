﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IClickable  
{
    Image MyIcon
    {
        get;
        set;
    }

    int MyCount
    {
        get;
    }

    Text MyStackText
    {
        get;
    }
}
