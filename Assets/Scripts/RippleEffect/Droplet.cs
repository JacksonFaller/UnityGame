using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Droplet
{
    Vector2 position;
    float time;

    public Droplet()
    {
        time = 1000;
    }

    public void Reset(Vector2 pos)
    {
        position = pos;
        time = 0;
    }

    public void Update()
    {
        time += Time.deltaTime * 2;
    }

    public Vector4 MakeShaderParameter(float aspect)
    {
        return new Vector4(position.x * aspect, position.y, time, 0);
    }
}
