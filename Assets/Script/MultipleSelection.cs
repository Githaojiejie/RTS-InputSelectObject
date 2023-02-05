using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 功能：单选、多选(按住左边的Ctrl+点击）、框选
/// </summary>
public enum State
{
    OnLeft,
    OnCtrlLeft
}


public class MultipleSelection : MonoBehaviour
{
    //定义一根射线
    private Ray ray;

    //枚举默认的点击状态是左键点击的状态
    private State clickState = State.OnLeft;
    private Vector3 v1, v2;


    //************弹窗信息

    //info面板
    // public GameObject infoPanel;
    //碰撞信息
    private RaycastHit hitInfo;


    //画线的材质
    // public Material mat;
    //鼠标一开始点击的位置
    private Vector2 FirstMouseposition;

    //鼠标松开的位置
    private Vector2 SecondMousePosition;

    //是否画线标志位
    private bool StartRender = false;

    //renderer数组用来保存各个对象身上的组件
    private Renderer[] gameObjects;

    //单选和框选的标志位
    private bool isKuangxuan = false;

    //用来存储每次点击的对象以及对象的颜色
    private Dictionary<GameObject, Color> dict;

    //鼠标点击的对象
    private GameObject clickGameObject;

    // public RectTransform rectTrans;
    //public RectTransform objRectTrans;
    //选择到的对象，用来做显示隐藏
    public static List<GameObject> objlist = new List<GameObject>();

    // BoxCollider[] objCollider;

    //获取renderer中心点所需要的变量
    float xmax = 0;
    float ymax = 0;
    float zmax = 0;
    float xmin = 0;
    float ymin = 0;
    float zmin = 0;

    void Start()
    {
        dict = new Dictionary<GameObject, Color>();
        //找到对象中含有的renderer组件
        gameObjects = FindObjectsOfType<Renderer>();
        //  objCollider = FindObjectsOfType<BoxCollider>();
    }


    void Update()
    {
        //**********************多选
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            objlist.Clear();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                //得到当前点击物体的颜色，并放入字典中
                OnChangeColor();
                objlist.Add(hitInfo.transform.gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            //点击空白处，清空
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider)
                {
                    // infoPanel.gameObject.SetActive(false);
                    RecoverColor();
                    clickState = State.OnCtrlLeft;
                    objlist.Clear();
                }
            }
            else
            {
                // infoPanel.gameObject.SetActive(false);
                RecoverColor();
                clickState = State.OnCtrlLeft;
                objlist.Clear();
            }
        }

        //点击ESC键，清空选中
        if (Input.GetKey(KeyCode.Escape))
        {
            // infoPanel.gameObject.SetActive(false);
            RecoverColor();
            clickState = State.OnCtrlLeft;
            objlist.Clear();
        }

        if (clickState != State.OnLeft)
        {
            //鼠标左键单击的时候
            if (Input.GetMouseButtonDown(0))
            {
                v1 = Input.mousePosition;
                StartRender = true;
                FirstMouseposition = Input.mousePosition;
                //选取对象
                //PickGameObject();
                isKuangxuan = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && isKuangxuan == true)
        {
            objlist.Clear();
            v2 = Input.mousePosition;
            if (Vector3.Distance(v1, v2) < 1)
            {
                StartRender = false;

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //******************单选
                if (Physics.Raycast(ray, out hitInfo))
                {
                    OnChangeColor();
                    objlist.Add(hitInfo.transform.gameObject);
                }
            }

            else
            {
                //******************框选
                StartRender = false;
                ChangeTwoPoint();
                //将线框里的对象都进行变色处理
                PickGameObject();
                //选择完毕后，将两个点归0
                FirstMouseposition = SecondMousePosition = Vector2.zero;
            }

            isKuangxuan = false;
        }

        SecondMousePosition = Input.mousePosition;


        clickState = State.OnLeft;
    }

    /// <summary>
    /// 存储原色，以及变色
    /// </summary>
    void OnChangeColor()
    {
        clickGameObject = hitInfo.transform.gameObject;
        if (dict.ContainsKey(clickGameObject)) return;
        dict.Add(clickGameObject, clickGameObject.GetComponent<Renderer>().material.color);
        if (clickGameObject.GetComponent<Renderer>().material.color == Color.blue)
            clickGameObject.GetComponent<Renderer>().material.color = new Color(0, 0.5f, 1);
        else
            clickGameObject.GetComponent<Renderer>().material.color = Color.blue;
    }

    /// <summary>
    /// 恢复颜色
    /// </summary>
    void RecoverColor()
    {
        //遍历字典让颜色恢复到原来的状态
        if (dict.Count != 0)
        {
            foreach (var kvp in dict)
            {
                kvp.Key.GetComponent<Renderer>().material.color = kvp.Value;
            }

            dict.Clear();
        }
    }

    //OnPostRender在相机渲染完所有物体之后被调用。只有该脚本
    //附于相机并启用时才会调用这个函数。OnPostRender可以是一个协同程序，在函数中调用yield语句即
    void OnPostRender()
    {
        if (StartRender)
        {
            //激活第一个pass,设置材质通道
            // mat.SetPass(0);
            GL.Color(Color.red);
            //辅助函数用来做一个正交投影变换,用来绘制2D图形
            GL.LoadOrtho();
            //开始绘制四边形
            GL.Begin(GL.LINES);
            DrawLine(FirstMouseposition.x, FirstMouseposition.y, SecondMousePosition.x, SecondMousePosition.y);
            GL.End();
        }
    }


    void DrawLine(float x1, float y1, float x2, float y2)
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

    void ChangeTwoPoint()
    {
        //右下角往左上方向进行框选
        if (FirstMouseposition.x > SecondMousePosition.x)
        {
            float position1 = FirstMouseposition.x;
            FirstMouseposition.x = SecondMousePosition.x;
            SecondMousePosition.x = position1;
        }

        //左上角往右下方向进行框选
        if (FirstMouseposition.y > SecondMousePosition.y)
        {
            float position2 = FirstMouseposition.y;
            FirstMouseposition.y = SecondMousePosition.y;
            SecondMousePosition.y = position2;
        }
    }

    //框选对象
    void PickGameObject()
    {
        objlist.Clear();
        foreach (Renderer item in gameObjects)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(GetCenterPoint(item));
            if (position.x >= FirstMouseposition.x & position.x <= SecondMousePosition.x &
                position.y >= FirstMouseposition.y & position.y <= SecondMousePosition.y)
            {
                //if (dict.ContainsKey(clickGameObject)) return;
                dict.Add(item.gameObject, item.material.color);
                item.material.color = Color.blue;
                objlist.Add(item.transform.gameObject);
            }
        }
    }

    //获取renderer组件的中心点
    Vector3 GetCenterPoint(Renderer render)
    {
        Vector3 vmax = render.bounds.max;
        Vector3 vmin = render.bounds.min;
        xmax = vmax.x;
        ymax = vmax.y;
        zmax = vmax.z;
        xmin = vmin.x;
        ymin = vmin.y;
        zmin = vmin.z;
        return (new Vector3((xmax + xmin) * 0.5f, (ymax + ymin) * 0.5f, (zmax + zmin) * 0.5f));
    }
}