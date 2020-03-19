using System;
using Models;
using UnityEngine;

public class CreateMap : MonoBehaviour
{
    private Map map;

    // Start is called before the first frame update
    private void Start()
    {
        map = new Map(3, 3, 3, -1.5f * (float) Math.Sqrt(3), 0, -1.5f);
        Debug.Log("Map created");
    }

    // Update is called once per frame
    private void Update()
    {
    }
}