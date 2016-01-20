﻿using AegirCore.Mesh.Loader;
using AegirCore.Simulation.Boyancy;
using AegirCore.Simulation.Water;
using System.IO;

namespace AegirCore.Behaviour.Simulation
{
    public class FloatingMesh : BehaviourComponent
    {
        private WaterCell water;
        private SimulationMesh mesh;

        private string hullModelPath;
        private float mass;

        public float Mass
        {
            get { return mass; }
            set
            {
                if (mass != value)
                {
                    mass = value;
                    mesh.Mass = value;
                }
            }
        }

        public FloatingMesh(WaterCell waterCell)
        {
            water = waterCell;
            mesh = new SimulationMesh(waterCell);
        }

        public string HullModelPath
        {
            get { return hullModelPath; }
            set
            {
                if (value != hullModelPath)
                {
                    hullModelPath = value;
                    ReloadHullModel(value);
                }
            }
        }

        private bool isValidHull;

        public bool IsHullModelValid
        {
            get { return isValidHull; }
            private set { isValidHull = value; }
        }

        public void ReloadHullModel(string newPath)
        {
            bool hullValid = false;
            if (File.Exists(newPath))
            {
                ObjModel hullModel = new ObjModel();
                hullModel.LoadObj(newPath);
                hullValid = hullModel.IsValid;
                if (hullValid)
                {
                    mesh.ToCompute = true;

                    //Create MeshData
                    mesh.Model = hullModel.GetMesh();
                }
            }

            IsHullModelValid = hullValid;
        }
    }
}