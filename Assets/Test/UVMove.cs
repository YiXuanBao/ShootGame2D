using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVMove : MonoBehaviour
{
    // Start is called before the first frame update
    Material _mat;
    public Vector2 uvOff;
    public Vector2 till;

    public float moveSpeed = 0.01f;

    private void Awake()
    {
        this._mat = GetComponent<Image>().material;
    }

    void Start()
    {
        uvOff = _mat.GetTextureOffset("_MainTex");
        till = _mat.GetTextureScale("_MainTex");
        Debug.Log(this._mat.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            uvOff.y += moveSpeed;
            //Debug.Log(" GetKey   KeyCode.UpArrow");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            uvOff.y -= moveSpeed;
            //Debug.Log(" GetKey   KeyCode.DownArrow");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            uvOff.x -= moveSpeed;
            //Debug.Log(" GetKey   KeyCode.LeftArrow");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            uvOff.x += moveSpeed;
            //Debug.Log(" GetKey   KeyCode.RightArrow");
        }
        
        this._mat.SetTextureOffset("_MainTex", uvOff);
        this._mat.SetTextureScale("_MainTex", till);
    }
}
