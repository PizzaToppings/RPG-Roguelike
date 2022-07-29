using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration 
{
    Vector3[] vertices;

    int xSize;
    int zSize;

    public void Generate(int xSize, int zSize)
    {
        this.xSize = xSize;
        this.xSize = xSize;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }
    }
}
