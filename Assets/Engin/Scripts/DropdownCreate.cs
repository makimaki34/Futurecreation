﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const_e;

public class DropdownCreate : MonoBehaviour
{
    public Dropdown m_dropdown;
    // Start is called before the first frame update
    void Start()
    {
        m_dropdown.ClearOptions();

        List<string> list = new List<string>();
        for(int i=1; i <= Engin_Const.line_size; i++)
        {
            list.Add("" + i);
        }
        m_dropdown.AddOptions(list);
        m_dropdown.value = 0;
    }
}
