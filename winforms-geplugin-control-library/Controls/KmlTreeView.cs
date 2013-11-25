﻿// <copyright file="KMLTreeView.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-12-08</date>
// <summary>This file is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    /// <summary>
    /// The KmlTreeView provides a quick and easy way to display kml content.
    /// It intergratres with the <see cref="GEWebBrowser"/> allowing a user to
    /// easily 'fly-to' and toggle the visiblity of features in the plugin.
    /// </summary>
    /// <remarks>
    /// The control supports virtual loading of content using the <see cref="KmlTreeViewNode"/> class.
    /// The KmlTreeView makes use of stacks and background workers to keep the control
    /// fast and responsive when dealing with large numbers of nodes. The control also supports
    /// tri-view checkboxes to show partial selection within a container.
    /// </remarks>
    public sealed partial class KmlTreeView : TreeView, IGEControls
    {
        #region Private fields

        /// <summary>
        /// The current browser
        /// </summary>
        private GEWebBrowser browser = null;

        /// <summary>
        /// The custom image list for tri-state check-box support
        /// </summary>
        private ImageList triStateImageList = null;

        /// <summary>
        /// Vaule indicating whether check-boxes are visible.
        /// </summary>
        private bool checkBoxesVisible = false;

        /// <summary>
        /// Vaule indicating whether to prevent the check-boxes check event.
        /// </summary>
        private bool preventChecking = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        public KmlTreeView()
            : base()
        {
            this.InitializeComponent();
            this.BuildTriStateImageList();
        }

        #region Public properties

        #region Control Properties

        /// <summary>
        /// Gets or sets a value indicating whether the treeview should display checkboxes or not.
        /// Default True
        /// </summary>
        [Category("Control Options")]
        [Description("Gets or sets a value indicating whether the treeview should display checkboxes or not. Default True")]
        [DefaultValue(false)]
        public new bool CheckBoxes
        {
            get
            {
                return this.checkBoxesVisible;
            }

            set
            {
                this.checkBoxesVisible = value;
                base.CheckBoxes = this.checkBoxesVisible;
                this.StateImageList = this.checkBoxesVisible ? this.triStateImageList : null;
            }
        }

        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a KmlTreeViewNode from an kml feature 
        /// </summary>
        /// <param name="feature">The kml feature to base the node on</param>
        /// <returns>A KmlTreeViewNode based on the feature</returns>
        public KmlTreeViewNode CreateNode(dynamic feature)
        {
            // create the node from the feature
            KmlTreeViewNode treeNode = new KmlTreeViewNode(feature);

            // if the node is a networlink, or
            // a document or folder with children
            if (treeNode.ApiType == ApiType.KmlNetworkLink ||
                ((treeNode.ApiType == ApiType.KmlDocument ||
                treeNode.ApiType == ApiType.KmlFolder) &&
                KmlHelpers.HasChildNodes(feature)))
            {
                // add a place holder 
                treeNode.Nodes.Add(new KmlTreeViewNode());
            }

            return treeNode;
        }

        /// <summary>
        ///  Returns the index of the first occurrence of a tree node with the specified object ID.
        ///  As the node key is automatically set from the kmlObject ID the IDs should always correspond and be unique.
        /// </summary>
        /// <param name="id">The API object id</param>
        /// <returns>The treenode for the object from the given ID (or an empty treenode if the ID isn't found)</returns>
        public KmlTreeViewNode GetNodeById(string id)
        {
            TreeNode[] nodes = this.Nodes.Find(id, true);
            KmlTreeViewNode node = new KmlTreeViewNode();

            if (nodes.Length == 1)
            {
                node = nodes[0] as KmlTreeViewNode;
            }

            return node;
        }

        /// <summary>
        /// Load a kml object into the treeview
        /// </summary>
        /// <param name="feature">The kml object to parse</param>
        public void ParseKmlObject(dynamic feature)
        {
            this.Nodes.Add(this.CreateNode(feature));
            this.Refresh();
        }

        /// <summary>
        /// Load a collection of kml objects into the treeview
        /// </summary>
        /// <param name="features">The kml objects to parse</param>
        public void ParseKmlObject(dynamic[] features)
        {
            foreach (dynamic feature in features)
            {
                this.ParseKmlObject(feature);
            }
        }

        /// <summary>
        /// Refreshes and updates the layout of the control
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.BeginUpdate();

            if (!this.CheckBoxes)
            {
                return;
            }

            base.CheckBoxes = false;

            Stack<KmlTreeViewNode> stack =
                new Stack<KmlTreeViewNode>(this.Nodes.Count);

            foreach (KmlTreeViewNode node in this.Nodes)
            {
                // Refresh internal properties of each node
                // http://code.google.com/p/winforms-geplugin-control-library/issues/detail?id=66
                node.Refresh();
                stack.Push(node);
            }

            while (stack.Count > 0)
            {
                KmlTreeViewNode node = stack.Pop();

                if (node.StateImageIndex == -1)
                {
                    node.StateImageIndex = node.Checked ? 1 : 0;
                }

                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    stack.Push(node.Nodes[i] as KmlTreeViewNode);
                }
            }

            this.EndUpdate();
        }

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="instance">The GEWebBrowser instance</param>
        public void SetBrowserInstance(GEWebBrowser instance)
        {
            this.browser = instance;
  
            if (!GEHelpers.IsGE(this.browser.Plugin))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.Nodes.Clear();
            this.Enabled = true;
            this.CheckBoxes = true;

            this.browser.PluginReady += (o, e) =>
            {
                this.Enabled = true;
            };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raised after a KmlTreeView id checked
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);

            if (this.preventChecking)
            {
                return;
            }

            this.OnNodeMouseClick(new TreeNodeMouseClickEventArgs(e.Node, MouseButtons.None, 0, 0, 0));
        }

        /// <summary>
        /// Raised after a KmlTreeViewNode is expanded
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);

            KmlTreeViewNode eventNode = e.Node as KmlTreeViewNode;
            eventNode.SetStyle();

            // If there is a place-holder node 
            if (eventNode.Nodes.ContainsKey(ApiType.None.ToString()))
            {
                if (eventNode.IsLoading == true)
                {
                    return;
                }

                // animate the place holder
                int index = eventNode.Nodes.IndexOfKey(ApiType.None.ToString());
                ((KmlTreeViewNode)eventNode.Nodes[index]).Animate();

                // set up the background worker
                // ...using named delegates to stop code clutter here
                BackgroundWorker nodeBuilder = new BackgroundWorker();
                nodeBuilder.DoWork += new DoWorkEventHandler(this.NodeBuilder_DoWork);
                nodeBuilder.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.NodeBuilder_RunWorkerCompleted);
                nodeBuilder.RunWorkerAsync(eventNode);
            }
            else
            {
                // There is no place-holder node...
                // so just set the check state of the children
                this.SetChildStateImageIndex(eventNode);
            }
        }

        /// <summary>
        /// Raised when the KmlTreeView layout changes
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            this.Refresh();
        }

        /// <summary>
        /// Raised when a KmlTreeView node is double clicked
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseDoubleClick(e);

            // check if the node needs updating...
            // ideally this should be event driven rather than via user interaction
            // but as yet the api does not expose networklink events...
            KmlTreeViewNode node =
                UpdateCheck(e.Node as KmlTreeViewNode, this.browser);
            node.Refresh(); 

            if (node.IsLoading)
            {
                return;
            }

            switch (node.ApiType)
            {
                case ApiType.KmlPlacemark:
                case ApiType.KmlFolder:
                case ApiType.KmlDocument:
                case ApiType.KmlGroundOverlay:
                    {
                        GEHelpers.OpenFeatureBalloon(this.browser.Plugin, node.ApiObject, setBalloon: node.Checked);
                    }

                    break;

                case ApiType.KmlTour:
                case ApiType.KmlPhotoOverlay:
                    {
                        GEHelpers.ToggleMediaPlayer(this.browser.Plugin, node.ApiObject, node.Checked);
                    }

                    return;   // exit here as the media player handles the view update

                case ApiType.None:
                    return;
                default:
                    break;
            }

            GEHelpers.FlyToObject(this.browser.Plugin, node.ApiObject);
        }

        /// <summary>
        /// Raised when the mouse is clicked on the KmlTreeView
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            Stack<KmlTreeViewNode> nodes;
            KmlTreeViewNode treeNode;
            bool state;

            base.OnNodeMouseClick(e);
            this.preventChecking = true;

            int space = this.ImageList == null ? 0 : 18;

            if ((e.X > e.Node.Bounds.Left - space ||
                e.X < e.Node.Bounds.Left - (space + 16)) &&
                e.Button != MouseButtons.None)
            {
                return;
            }

            treeNode = e.Node as KmlTreeViewNode;
            if (e.Button == MouseButtons.Left)
            {
                // toggle the check state
                treeNode.Checked = !treeNode.Checked;
                treeNode.ApiObjectVisible = treeNode.Checked;

                if (!treeNode.Checked)
                {
                    // Turn off the media player for the node if it was unchecked 
                    // For example, unchecking a Tour object in the tree exits the tour player
                    GEHelpers.ToggleMediaPlayer(this.browser.Plugin, treeNode.ApiObject, false);
                }
            }

            treeNode.StateImageIndex = treeNode.Checked ? 1 : treeNode.StateImageIndex;
            
            this.OnAfterCheck(new TreeViewEventArgs(treeNode, TreeViewAction.ByMouse));

            nodes = new Stack<KmlTreeViewNode>(treeNode.Nodes.Count);
            nodes.Push(treeNode);

            do
            {
                treeNode = nodes.Pop();
                treeNode.Checked = e.Node.Checked;
                treeNode.ApiObjectVisible = treeNode.Checked;
                for (int i = 0; i < treeNode.Nodes.Count; i++)
                {
                    nodes.Push(treeNode.Nodes[i] as KmlTreeViewNode);
                }
            }
            while (nodes.Count > 0);

            state = false;
            treeNode = e.Node as KmlTreeViewNode;

            while (treeNode.Parent != null)
            {
                foreach (KmlTreeViewNode child in treeNode.Parent.Nodes)
                {
                    state |= child.Checked != treeNode.Checked | child.StateImageIndex == 2;
                }

                int index = (int)Convert.ToUInt32(treeNode.Checked);
                treeNode.Parent.Checked = state || (index > 0);
           
                if (state)
                {
                    ((KmlTreeViewNode)treeNode.Parent).ApiObjectVisible = state;
                    treeNode.Parent.StateImageIndex = 2;
                }
                else
                {
                    treeNode.Parent.StateImageIndex = index;
                }

                treeNode = treeNode.Parent as KmlTreeViewNode;
            }

            this.preventChecking = false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Generates an ID for a feature loaded from a remote reasource 
        /// </summary>
        /// <param name="url">the base url</param>
        /// <param name="id">the object id</param>
        /// <returns>the generated id</returns>
        private static string GenerateId(string url, string id)
        {
            string hashed = string.IsNullOrEmpty(id) ? string.Empty : url + "#" + id;
            return string.IsNullOrEmpty(url) ? id : hashed;
        }

        /// <summary>
        /// Checks if a given element in the treeview needs updating
        /// </summary>
        /// <param name="node">The node to check for</param>
        /// <param name="browser">The browser instance to check in</param>
        /// <returns>a tree view node based on the new data.</returns>
        private static KmlTreeViewNode UpdateCheck(KmlTreeViewNode node, GEWebBrowser browser)
        {
            dynamic liveObject = null;

            try
            {
                liveObject = browser.Plugin.getElementByUrl(node.Name);
            }
            catch (COMException)
            {
            }

            if (liveObject != null)
            {
                KmlTreeViewNode newNode = new KmlTreeViewNode(liveObject);
                newNode.Name = node.Name;
                newNode.BaseUrl = node.BaseUrl; 

                if (node.Parent != null)
                {
                    // update the tree
                    node.Parent.Nodes.Insert(node.Index, newNode);
                    node.Parent.Nodes.Remove(node);
                }

                return newNode;
            }

            return node;
        }

        /// <summary>
        /// Builds the tri-state image list for use with the control
        /// </summary>
        private void BuildTriStateImageList()
        {
            this.triStateImageList = new ImageList();

            CheckBoxState state = CheckBoxState.UncheckedNormal;

            for (int i = 0; i < 3; i++)
            {
                Bitmap bitmap = new Bitmap(16, 16);
                Graphics graphics = Graphics.FromImage(bitmap);

                switch (i)
                {
                    case 0:
                        state = CheckBoxState.UncheckedNormal;
                        break;

                    case 1:
                        state = CheckBoxState.CheckedNormal;
                        break;

                    case 2:
                        state = CheckBoxState.MixedNormal;
                        break;
                }

                CheckBoxRenderer.DrawCheckBox(graphics, new Point(2, 2), state);
                graphics.Save();

                this.triStateImageList.Images.Add(bitmap);
            }
        }

        /// <summary>
        /// Sets the StateImageIndex of each child node of the given <paramref name="node">parent tree node</paramref>
        /// </summary>
        /// <param name="node">The parent tree node</param>
        private void SetChildStateImageIndex(KmlTreeViewNode node)
        {
            foreach (KmlTreeViewNode child in node.Nodes)
            {
                if (child.StateImageIndex == -1)
                {
                    child.StateImageIndex = child.Checked ? 1 : 0;
                }
            }

            this.Refresh();
        }

        /// <summary>
        /// Asynchronous method for building a list of nodes
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NodeBuilder_DoWork(object sender, DoWorkEventArgs e)
        {
            // node passed in from RunWorkerAsync
            KmlTreeViewNode baseNode = e.Argument as KmlTreeViewNode;
            baseNode.IsLoading = true;

            // create a stack to hold the children we will create
            Stack<KmlTreeViewNode> children = new Stack<KmlTreeViewNode>();
            bool linkFailed = false;

            if (baseNode.ApiType == ApiType.KmlNetworkLink)
            {
                // fetch the networklink data 
                string url = KmlHelpers.GetUrl(baseNode.ApiObject).ToString();
                dynamic data = null;
                bool rebuild = false;

                // can't use the new FetchAndParse with archive files...
                if (url.EndsWith("kmz", StringComparison.OrdinalIgnoreCase) || 
                    url.EndsWith("dae", StringComparison.OrdinalIgnoreCase))
                {
                    data = this.browser.FetchKmlSynchronous(url);
                }
                else
                {
                    data = this.browser.FetchAndParse(url);
                    rebuild = true;
                }

                if (data != null)
                {
                    KmlTreeViewNode link = this.CreateNode(data);

                    if (rebuild)
                    {
                        // The kmlobjects are created via parsekml so they need their id's need to be rebuilt
                        // so that the Url is still present (e.g. http://foo.com/#id vs #id)
                        // the baseurl of the node is set so that any child nodes can also have there id's rebuilt
                        // when they are created.
                        link.Name = GenerateId(url, data.getId());
                        link.BaseUrl = url; 
                    }

                    // create a new treenode from the data and push it on to the stack
                    children.Push(link);
                }
                else
                {
                    // no data, so push a new place holder node in and set the loadError flag
                    baseNode.IsLoading = false;
                    children.Push(new KmlTreeViewNode());
                    linkFailed = true;
                }
            }
            else
            {
                // the feature must be a KmlFolder or a KmlDocument (KmlContainer)
                // ...so get the child nodes from it
                dynamic kmlChildNodes = KmlHelpers.GetChildNodes(baseNode.ApiObject);

                if (kmlChildNodes != null)
                {
                    int count = kmlChildNodes.getLength();
                    for (int i = 0; i < count; i++)
                    {
                        // create a new KmlTreeViewNode from each feature in the KmlContainer 
                        // and push it on to the stack. 
                        try
                        {
                            children.Push(this.CreateNode(kmlChildNodes.item(i)));
                        }
                        catch (COMException)
                        {
                            children.Clear();
                            return;
                        }
                    }
                }
            }

            // pass the base node, child stack and error flag as the result.
            e.Result = new object[] { baseNode, children, linkFailed };
        }

        /// <summary>
        /// Adds children to a KmlTreeViewNode.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NodeBuilder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                return;
            }

            // the result object from NodeBuilder_DoWork
            object[] result = e.Result as object[];
            KmlTreeViewNode baseNode = result[0] as KmlTreeViewNode;
            Stack<KmlTreeViewNode> children = result[1] as Stack<KmlTreeViewNode>;
            bool linkFailed = Convert.ToBoolean(result[2], CultureInfo.InvariantCulture);

            // clear the node of all children
            baseNode.Nodes.Clear();

            // add the children to the node 
            // (is just a new placeholder if there was error loading a networklink)
            while (children.Count > 0)
            {
                KmlTreeViewNode child = children.Pop();
                baseNode.Nodes.Add(child);

                // If the parent has a BaseUrl then we need to use this to generate the 
                // correct ids for the child, we also set the baseurl on the child for its children ...
                // we also do an update check as it is possible the node needs to be updated...
                string url = ((KmlTreeViewNode)child.Parent).BaseUrl;

                if (!string.IsNullOrEmpty(url) && child.ApiObject != null)
                {
                    child.Name = GenerateId(url, child.ApiObject.getId());
                    child.BaseUrl = url; 
                    UpdateCheck(child, this.browser);
                }
            }

            if (linkFailed)
            {
                baseNode.Collapse();
                baseNode.ImageKey = baseNode.SelectedImageKey = "linkFolderClosedDisconected";
            }
            else
            {
                this.SetChildStateImageIndex(baseNode);
            }
        }

        #endregion
    }
}