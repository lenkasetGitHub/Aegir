﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AegirCore.Simulation.Mesh
{
    public struct TriangleWetted
    {
        //Translated these comments from french, does not mean i understand the fields :P
        public int I0;              // Indices
        public int I1;
        public int I2;
        public Color color;         // Color visible in Debug Mode
        public bool bNoChange;      // Is this a triangle created by intersection with water or an original
        public float Depth;         // Depth
        public float fArea;         // triangle area
        public Vector3d vNormal;     // Normal for the triangle when the vertexes are enumarated in an counter clockwise order

        public Vector3d vPressure;   // Pressure force vector
        public float fPressure;     // pressure force standard
        public Vector3d pCPressure;  // Point of pressure
    }
}