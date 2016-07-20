﻿using AegirCore.Persistence;
using AegirCore.Persistence.Data;
using AegirCore.Persistence.Persisters;
using AegirCore.Project;
using AegirCore.Scene;
using AegirCore.Simulation;
using System;
using System.Xml.Linq;
using TinyMessenger;

namespace AegirCore
{
    public class ApplicationContext
    {
        public SimulationEngine Engine { get; private set; }
        public PersistenceHandler SaveLoadHandler { get; private set; }
        public ITinyMessengerHub MessageHub { get; private set; }
        public SceneGraph Scene { get; private set; }
        public ApplicationContext()
        {
            Scene = new SceneGraph();

            SaveLoadHandler = new PersistenceHandler();
            //Adding specific persisters
            SaveLoadHandler.AddPersister(new ScenePersister() { Graph = Scene });
            //Set up Engine
            Engine = new SimulationEngine(Scene);
        }

    }
}