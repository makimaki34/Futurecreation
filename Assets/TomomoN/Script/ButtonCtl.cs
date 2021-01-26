﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCtl : MonoBehaviour
{
    GameObject Hand = null;

    // Start is called before the first frame update
    void Start()
    {
        Hand = GameObject.Find("HandTexture");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerStay2D(Collider2D t)
    {
        if (t.gameObject.name == "HandTexture")
        {
            Hand.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
    void OnTriggerExit2D(Collider2D t)
    {
        if (t.gameObject.name == "HandTexture")
        {
            Hand.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
    }
}
