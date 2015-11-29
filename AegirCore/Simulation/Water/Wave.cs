﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegirCore.Simulation.Water
{
    public class Wave
    {
        public float Len;
        public float Freq;  // 2*PI / wavelength
        public float Amp;   // amplitude
        public float Speed;
        public float Phase; // speed * 2*PI / wavelength
        public float Angle;
        public Vector2d Dir;
        public override string ToString()
        {
            return "L: " + Len.ToString("0.00") +
                    " A: " + Amp.ToString("0.00") +
                    " S: " + Speed.ToString("0.00") +
                    " D: " + Angle.ToString("0.00");
        }
    }
}
