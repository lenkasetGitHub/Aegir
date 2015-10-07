﻿using AegirCore.Entity;
using AegirCore.Project.Event;
using AegirCore.Scene;
using AegirCore.Vessel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegirCore.Project
{
    public class ProjectContext
    {
        private ProjectData activeProject;
        public ProjectData ActiveProject
        {
            get 
            {
                return activeProject;
            }
            private set
            {
                if(value != activeProject)
                {
                    activeProject = value;
                    TriggerActiveProject(value);
                }
            }
        }
        public ProjectContext()
        {
            
        }
        /// <summary>
        /// Opens a new project from a file and sets it as the active one
        /// </summary>
        /// <param name="filePath">The path to the .proj file</param>
        public void OpenProject(string filePath)
        {
            if(File.Exists(filePath))
            {
                ProjectLoadEventArgs args = new ProjectLoadEventArgs(filePath);
                TriggerProjectLoadFailure(args);
            }
        }
        public void OpenProject(ProjectData project)
        {
            ActiveProject = project;
        }

        public ProjectData CreateNewProject()
        {
            SceneGraph scene = new SceneGraph();
            scene.RootNodes.Add(new Water());
            var vessel = new Entity.Vessel();
            vessel.Children.Add(new GNSSReceiver());
            vessel.Children.Add(new GNSSReceiver());
            scene.RootNodes.Add(vessel);
            //scene.RootNodes.Add(new Water());
            VesselConfiguration vesselConf = new VesselConfiguration();
            return new ProjectData(scene, vesselConf, "New Simulation");
        }

        private void TriggerProjectLoadSuccess(ProjectLoadEventArgs e)
        {
            ProjectLoadEventHandler evt = ProjectLoaded;
            if(evt!=null)
            {
                evt(e);
            }
        }
        private void TriggerProjectLoadFailure(ProjectLoadEventArgs e)
        {
            ProjectLoadEventHandler evt = ProjectLoadFailure;
            if (evt != null)
            {
                evt(e);
            }
        }
        private void TriggerActiveProject(ProjectData project)
        {
            ProjectActivateEventHandler evt = ProjectActivated;
            if(evt!=null)
            {
                evt(new ProjectActivateEventArgs(project));
            }
        }

        public delegate void ProjectLoadEventHandler(ProjectLoadEventArgs e);
        public delegate void ProjectActivateEventHandler(ProjectActivateEventArgs e);

        public event ProjectActivateEventHandler ProjectActivated;
        public event ProjectLoadEventHandler ProjectLoaded;
        public event ProjectLoadEventHandler ProjectLoadFailure;

    }
}
