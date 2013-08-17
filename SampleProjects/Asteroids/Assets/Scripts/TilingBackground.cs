﻿using UnityEngine;
using System.Collections;

public class TilingBackground : MonoBehaviour 
{
	public float Speed;

    Vector2 offset;

    void Update()
    {
        offset.y += Speed * Time.deltaTime;
        renderer.material.mainTextureOffset = offset;
	}
}
