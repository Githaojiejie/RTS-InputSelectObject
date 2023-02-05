using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
public class DrawTest : MonoBehaviour {
    /// <summary>
    /// 材质球
    /// </summary>
    public Material mat;
    /// <summary>
    /// 鼠标开始的位置
    /// </summary>
    private Vector2 FirstMousePosition;
    /// <summary>
    /// 鼠标结束的位置
    /// </summary>
    private Vector2 SecondMousePosition;
    private bool StartRender = false;
    private Renderer[] gameobjects;
// Use this for initialization
void Start ()
    {
        gameobjects = FindObjectsOfType<Renderer>();	
}
 
// Update is called once per frame
void Update ()
    {
        //获取鼠标按下
        if(Input.GetMouseButtonDown(0))
        {
            StartRender = true;
            FirstMousePosition = Input.mousePosition;
            PickGameObject();
        }
        //获取鼠标抬起
        if (Input.GetMouseButtonUp(0))
        {
            StartRender = false;
            ChangeTwoPoint();
            PickGameObject();
            FirstMousePosition = SecondMousePosition = Vector2.zero;
        }
        SecondMousePosition = Input.mousePosition;
}

public  Color rectColor;

 
    private void OnPostRender()
    {
        if (StartRender)
        {
            mat.SetPass(0);
            GL.LoadOrtho();
            // GL.LoadPixelMatrix();//设置用屏幕坐标绘图

            GL.Begin(GL.LINES);
            
            GL.Color(rectColor);
            DrawLine(FirstMousePosition.x, FirstMousePosition.y, SecondMousePosition.x, SecondMousePosition.y);
            GL.End(); 
        }
    }
    /// <summary>
    /// 画线
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    private void DrawLine(float x1, float y1, float x2, float y2)
    {
        GL.Vertex(new Vector3(x1 / Screen.width, y1 / Screen.height, 0));
        GL.Vertex(new Vector3(x2 / Screen.width, y1 / Screen.height, 0));
        GL.Vertex(new Vector3(x2 / Screen.width, y1 / Screen.height, 0));
        GL.Vertex(new Vector3(x2 / Screen.width, y2 / Screen.height, 0));
        GL.Vertex(new Vector3(x2 / Screen.width, y2 / Screen.height, 0));
        GL.Vertex(new Vector3(x1 / Screen.width, y2 / Screen.height, 0));
        GL.Vertex(new Vector3(x1 / Screen.width, y2 / Screen.height, 0));
        GL.Vertex(new Vector3(x1 / Screen.width, y1 / Screen.height, 0));
    }
    /// <summary>
    /// 改变两点
    /// </summary>
    private void ChangeTwoPoint()
    {
        if (FirstMousePosition.x > SecondMousePosition.x)
        {
            float position1 = FirstMousePosition.x;
            FirstMousePosition.x = SecondMousePosition.x;
            SecondMousePosition.x = position1;
        }
        if (FirstMousePosition.y > SecondMousePosition.y)
        {
            float position2 = FirstMousePosition.y;
            FirstMousePosition.y = SecondMousePosition.y;
            SecondMousePosition.y = position2;
        }
    }
    /// <summary>
    /// 改变物体颜色
    /// </summary>
    private void PickGameObject()
    {
        //遍历所有的组件
        foreach (Renderer item in gameobjects)
        {//判断位置
            Vector3 position = Camera.main.WorldToScreenPoint(item.transform.position);
            if (position.x >= FirstMousePosition.x & position.x <= SecondMousePosition.x & position.y >= FirstMousePosition.y & position.y <= SecondMousePosition.y)
            {
 
//改变颜色
 
                item.material.color = Color.red;
            }
            else
            {
 
//改变颜色
 
                item.material.color = Color.white;
            }
 
 
        }
    }
}
