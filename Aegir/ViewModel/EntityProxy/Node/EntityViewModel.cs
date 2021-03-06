﻿using Aegir.Mvvm;
using Aegir.Rendering;
using Aegir.View.PropertyEditor.CustomEditor;
using Aegir.ViewModel.EntityProxy.Behaviour.Output;
using Aegir.ViewModel.EntityProxy.Simulation;
using Aegir.ViewModel.EntityProxy.Vessel;
using Aegir.Windows;
using AegirLib.Behaviour;
using AegirLib.Behaviour.World;
using AegirLib.Scene;
using GalaSoft.MvvmLight.Command;
using HelixToolkit.Wpf;
using PropertyTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Media3D;
using TinyMessenger;
using ViewPropertyGrid.PropertyGrid;
using ViewPropertyGrid.PropertyGrid.Component;

namespace Aegir.ViewModel.EntityProxy.Node
{
    public class EntityViewModel : SceneNodeViewModelBase,
                                ITransformableVisual,
                                IComponentContainer,
                                IDragSource,
                                IDropTarget,
                                INameable
    {

        protected Entity entityData;
        private Transform transform;
        private List<BehaviourViewModel> componentProxies;
        private Transform3D visualTransform;


        public Entity EntitySource
        {
            get { return entityData; }
        }

        [DisplayName("Name")]
        [Category("General")]
        public string Name
        {
            get { return entityData.Name; }
            set
            {
                if (value != entityData.Name)
                {
                    entityData.Name = value;
                    RaisePropertyChanged();
                }
            }
        }


        public RelayCommand RemoveEntityCommand { get; set; }
        public RelayCommand<string> AddEntityCommand { get; set; }
        public IScenegraphAddRemoveHandler AddRemoveHandler { get; set; }

        //public Point3D Position
        //{
        //    get
        //    {
        //        return new Point3D(transform.LocalPosition.X, transform.LocalPosition.Y, transform.LocalPosition.Z);
        //    }
        //}

        //public Quaternion Rotation
        //{
        //    get
        //    {
        //        return new Quaternion(transform.LocalRotation.X, transform.LocalRotation.Y, transform.LocalRotation.Z, transform.LocalRotation.W);
        //    }
        //}

        public bool IsDraggable
        {
            get
            {
                return true;
            }
        }

        public Transform3D VisualTransform
        {
            get
            {
                return TransformHelper.LibTransformToWPFTransform(transform.LocalPosition,transform.LocalRotation);
            }

            set
            {
                if(visualTransform!=value)
                {
                    visualTransform = value;
                    transform.LocalPosition = TransformHelper.Transform3DToPosition(value);
                    transform.LocalRotation = TransformHelper.Transform3DToQuaternion(value);
                }
            }
        }

        /// <summary>
        /// Creates a new proxy entity
        /// </summary>
        /// <param name="entityData">The entity to proxy</param>
        public EntityViewModel(Entity entityData, IScenegraphAddRemoveHandler addRemoveHandler)
        {
            this.AddRemoveHandler = addRemoveHandler;
            this.entityData = entityData;
            this.componentProxies = new List<BehaviourViewModel>();
            //All entities should have a transform behaviour
            transform = entityData.GetComponent<Transform>();

            AddEntityCommand = new RelayCommand<string>(AddEntity);
            RemoveEntityCommand = new RelayCommand(DoRemoveEntity);

            CreateBehaviourProxies(entityData.Components);
        }

        private void CreateBehaviourProxies(IEnumerable<BehaviourComponent> behaviourComponents)
        {
            foreach (BehaviourComponent component in behaviourComponents)
            {
                BehaviourViewModel vm = BehaviourViewModelFactory.GetViewModelProxy(component);
                if (vm != null)
                {
                    componentProxies.Add(vm);
                }
            }
        }

        public T GetEntityComponent<T>()
            where T : BehaviourComponent
        {
            return entityData.GetComponent<T>();
        }

        private void DoRemoveEntity()
        {
            AddRemoveHandler?.Remove(this);
            Debug.WriteLine("Removing Entity");
        }

        private void AddEntity(string type)
        {
            AddRemoveHandler?.Add(type);
            Debug.WriteLine("Adding Entity: " + type);
        }

        public override string ToString()
        {
            return "EntityViewModelProxy For: " + entityData.Name;
        }

        public void ApplyTransform(Transform3D targetTransform)
        {
            Quaternion rotation = targetTransform.ToQuaternion();
            Point3D position = targetTransform.ToPoint3D();
            transform.LocalPosition = position.ToLibVector();
            transform.LocalRotation = rotation.ToLibQuaternion();
        }

        internal void Invalidate()
        {
            foreach (BehaviourViewModel behaviourVM in componentProxies)
            {
                behaviourVM.Invalidate();
            }
        }

        //public InspectableProperty[] GetProperties()
        //{
        //    List<InspectableProperty> properties = new List<InspectableProperty>();

        //    //Add Entity properties
        //    properties.Add(new InspectableProperty(this, GetType().GetProperty(nameof(Name))));
        //    properties.Add(new InspectableProperty(this, GetType().GetProperty(nameof(IsEnabled))));

        //    foreach (BehaviourViewModel behaviour in componentProxies)
        //    {
        //        properties.AddRange(behaviour.GetInspectableComponent());
        //    }

        //    return properties.ToArray();
        //}

        public void Detach()
        {
        }

        public bool CanDrop(IDragSource node, DropPosition dropPosition, DragDropEffect effect)
        {
            return true;
        }

        public void Drop(IEnumerable<IDragSource> items, DropPosition dropPosition, DragDropEffect effect, PropertyTools.DragDropKeyStates initialKeyStates)
        {
        }

        public ComponentDescriptor[] GetAvailableComponents()
        {
            return ComponentDescriptorCache.GetDescriptors(new Type[] {
                typeof(VesselDimentionsViewModel),
                typeof(WaterSimulationViewModel),
                typeof(FileOutputViewModel),
                typeof(UdpOutputViewModel),
                typeof(TCPOutputViewModel),
            });
        }

        public IInspectableComponent[] GetInspectableComponents()
        {
            List<IInspectableComponent> properties = new List<IInspectableComponent>();

            //IInspectableComponent generalComponent = new
            ////Add Entity properties
            //properties.Add(new InspectableProperty(this, GetType().GetProperty(nameof(Name))));
            //properties.Add(new InspectableProperty(this, GetType().GetProperty(nameof(IsEnabled))));

            foreach (BehaviourViewModel behaviour in componentProxies)
            {

                properties.Add(behaviour);
            }
            return properties.ToArray();
        }

        public void AddComponent(ComponentDescriptor componentDescriptor)
        {
            Type vmType = componentDescriptor.ComponentType;
            Type componentType = BehaviourViewModelFactory.GetBehaviourFromViewModelProxy(vmType);
            BehaviourComponent component = BehaviourFactory.CreateFromType(componentType, entityData);
            entityData.AddComponent(component);
            //Create a viewmodel proxy for our new component
            BehaviourViewModel vm = BehaviourViewModelFactory.GetViewModelProxy(component);
            ComponentAdded?.Invoke(vm);
        }

        public void ComponentRemoved(IInspectableComponent component)
        {
            throw new NotImplementedException();
        }

        public event ComponentAddedHandler ComponentAdded;
        //public void TriggerTransformChanged()
        //{
        //    TransformationChangedHandler transformEvent = TransformationChanged;
        //    if (transformEvent != null && Notify)
        //    {
        //        transformEvent();
        //    }
        //}
    }
}