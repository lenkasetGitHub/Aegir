﻿using Aegir.Messages.ObjectTree;
using Aegir.Messages.Project;
using Aegir.Messages.Simulation;
using AegirCore.Scene;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegir.ViewModel.NodeProxy
{
    public class ScenegraphViewModelProxy : ViewModelBase
    {
        /// <summary>
        /// The scenegraph source we are wrapping
        /// </summary>
        private SceneGraph sceneSource;

        private NodeViewModelProxy selectedItem;
        /// <summary>
        /// Command to be executed when selected item has changed
        /// </summary>
        public RelayCommand<NodeViewModelProxy> SelectItemChanged { get; private set; }
        /// <summary>
        /// Command to be executed when an item is wanted to be removed from the graph
        /// </summary>
        public RelayCommand<NodeViewModelProxy> RemoveItemCommand { get; private set; }
        /// <summary>
        /// Command to be executed when an item is wanted to be moved
        /// </summary>
        public RelayCommand<NodeViewModelProxy> MoveItemCommand { get; private set; }
        /// <summary>
        /// A Graph of node viewmodel proxies composed from our scene source
        /// </summary>
        public ObservableCollection<NodeViewModelProxy> Items { get; set; }

        public NodeViewModelProxy SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    UpdateSelectedItem(value);
                    RaisePropertyChanged();
                }
            }
        }
        /// <summary>
        /// Creates a new Scenegraph View Model
        /// </summary>
        public ScenegraphViewModelProxy()
        {
            SelectItemChanged = new RelayCommand<NodeViewModelProxy>(c => SelectedItem = c);
            RemoveItemCommand = new RelayCommand<NodeViewModelProxy>(RemoveItem);
            MoveItemCommand = new RelayCommand<NodeViewModelProxy>(MoveTo);

            MessengerInstance.Register<ProjectActivated>(this, ProjectChanged);
            MessengerInstance.Register<InvalidateEntities>(this, OnInvalidateEntitiesMessage);
            MessengerInstance.Register<ProjectActivated>(this, OnProjectActivated);

        }
        /// <summary>
        /// Updates the currently active selected item in the graph
        /// </summary>
        /// <param name="newItem"></param>
        public void UpdateSelectedItem(NodeViewModelProxy newItem)
        {
            SelectedNodeChanged.Send(newItem);
        }
        private void ProjectChanged(ProjectActivated projectMessage)
        {
            RebuildScenegraphNodes(projectMessage.Project.Scene.RootNodes);
        }

        private void RemoveItem(NodeViewModelProxy item)
        {
            Debug.WriteLine("Removing Item" + item);
        }
        private void MoveTo(NodeViewModelProxy item)
        {
            Debug.WriteLine("Moving Item" + item);
        }
        /// <summary>
        /// Message handler for invalidate entities message
        /// </summary>
        /// <param name="message"></param>
        public void OnInvalidateEntitiesMessage(InvalidateEntities message)
        {
            TriggerInvalidateChildren();
        }
        /// <summary>
        /// message Handler for project loaded/activated message
        /// </summary>
        /// <param name="message"></param>
        public void OnProjectActivated(ProjectActivated message)
        {
            if(sceneSource!=null)
            {
                sceneSource.GraphChanged -= SceneSource_GraphChanged;
            }
            sceneSource = message.Project.Scene;
            sceneSource.GraphChanged += SceneSource_GraphChanged;
            //Build Scenegraph
            RebuildScenegraphNodes(sceneSource.RootNodes);
        }
        /// <summary>
        /// Event Handler for the scenegraph changed event on our scene source
        /// </summary>
        private void SceneSource_GraphChanged()
        {
            //At the moment we rebuild the graph if it's changed
            Items.Clear();
            RebuildScenegraphNodes(sceneSource.RootNodes);
            TriggerScenegraphChanged();
        }
        /// <summary>
        /// Rebuilds our graph of proxy objects to resemble the one in our scene source
        /// </summary>
        /// <param name="nodes"></param>
        private void RebuildScenegraphNodes(IEnumerable<Node> nodes)
        {
            foreach(Node n in nodes)
            {
                NodeViewModelProxy nodeProxy = new NodeViewModelProxy(n);
                Items.Add(nodeProxy);

                RebuildScenegraphNodes(n.Children);
            }
        }
        /// <summary>
        /// Triggers our invalidate event, letting listeners update their props based on new viewmodel changes
        /// </summary>
        public void TriggerInvalidateChildren()
        {
            InvalidateChildrenHandler InvalidateEvent = InvalidateChildren;
            if (InvalidateEvent != null)
            {
                InvalidateEvent();
            }
        }
        /// <summary>
        /// Triggers a scenegraph changed event when we have rebuilt the proxy graph
        /// </summary>
        public void TriggerScenegraphChanged()
        {
            ScenegraphChangedHandler ChangeEvent = ScenegraphChanged;
            if (ChangeEvent != null)
            {
                ChangeEvent();
            }
        }

        public delegate void InvalidateChildrenHandler();
        public event InvalidateChildrenHandler InvalidateChildren;

        public delegate void ScenegraphChangedHandler();
        public event ScenegraphChangedHandler ScenegraphChanged;
    }
}