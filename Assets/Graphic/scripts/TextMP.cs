﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextMP : MonoBehaviour
{
    private TextMeshPro text;
    public bool vertical;        //縦書きか
    private Vector3[] vec;
    private bool once;
    public int cnt;
    // Start is called before the first frame update
    void Start()
    {
       text = GetComponent<TextMeshPro>();


        //縦書き
        if(vertical)         
        {
            Quaternion rot = text.rectTransform.rotation;
            Vector3 pos = text.rectTransform.position;
            pos.y -= 3.0f;
            rot.z = -1;
            rot.x = 0;
            rot.y = 0;
            text.rectTransform.rotation = rot;
            text.rectTransform.position = pos;
            once = true;
            cnt = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //縦書き用
       if (once)
       {
           if (vertical)
           {
               for (int i = 0; i < text.textInfo.characterCount; i++)
               {
                    var charaInfo = text.textInfo.characterInfo[i];

                    if(!charaInfo.isVisible)
                    {
                        continue;
                    }

                    int materialIndex = charaInfo.materialReferenceIndex;
                    int vertexIndex = charaInfo.vertexIndex;

                    Vector3[] destVertices = text.textInfo.meshInfo[materialIndex].vertices;

                    float angle = 180 * Mathf.Sin(Time.time/100);

                    Vector3 rotateCenterVertex = Vector3.Lerp(destVertices[vertexIndex + 1], destVertices[vertexIndex + 2],0.5f);

                    destVertices[vertexIndex + 0] += -rotateCenterVertex;
                    destVertices[vertexIndex + 1] += -rotateCenterVertex;
                    destVertices[vertexIndex + 2] += -rotateCenterVertex;
                    destVertices[vertexIndex + 3] += -rotateCenterVertex;

                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90), Vector3.one);
                    destVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destVertices[vertexIndex + 0]);
                    destVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destVertices[vertexIndex + 1]);
                    destVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destVertices[vertexIndex + 2]);
                    destVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destVertices[vertexIndex + 3]);

                    destVertices[vertexIndex + 0] +=  rotateCenterVertex;
                    destVertices[vertexIndex + 1] +=  rotateCenterVertex;
                    destVertices[vertexIndex + 2] +=  rotateCenterVertex;
                    destVertices[vertexIndex + 3] +=  rotateCenterVertex;
                    matrix = Matrix4x4.identity;
                }
       
           }
       
           for (int i = 0; i < text.textInfo.meshInfo.Length; i++)
           {
               text.textInfo.meshInfo[i].mesh.vertices = text.textInfo.meshInfo[i].vertices;
               text.UpdateGeometry(text.textInfo.meshInfo[i].mesh, i);
           }
            
        }
        cnt++;
        if(cnt>1)
        {
            once = false;
            cnt = 0;
        }
    }
}
