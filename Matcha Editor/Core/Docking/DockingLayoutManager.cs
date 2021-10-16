using Matcha_Editor.MVVM.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Matcha_Editor.Core.Docking
{
    public class DockingLayoutManager
    {
        private DockingNode m_RootNode;

        public DockingLayoutManager(Size size)
        {
            m_RootNode = new DockingNode{ 
                Rect = new Rect(size)
            };
        }

        public void AddNewNode(DockingPanelView panel, Point position = new Point())
        {
            DockingNode newNode = GetPreviewNode(position);
            newNode.AttachedPanel = panel;
            DockNode(newNode);
            RefreshLayout();
        }

        public void RemoveNode(DockingNode node)
        {
            Debug.Assert(!node.HasChildren());
            Debug.Assert(!node.IsAncestor());
            Debug.Assert(node.HasUIAttached());

            if (node.Parent.IsAncestor())
                node.Parent.LeftChild = null;
            else
                node.Parent.ReplaceChild(node, null);

            node.Collapse();
            RefreshLayout();
        }

        public DockingNode FindNode(DockingPanelView panel)
        {
            return m_RootNode.RecursivelyFindNode(panel);
        }

        public DockingNode GetPreviewNode(Point position)
        {
            DockingNode leafNode = GetLeafNode(position);
            return leafNode?.GetSubzone(position);
        }

        public void Resize(Size newSize)
        {
            Rect newRect = new Rect(newSize);
            m_RootNode.Rect = newRect;
            RefreshLayout();
        }

        private void RefreshLayout()
        {
            m_RootNode.RecursiveResize(m_RootNode.Rect);
        }

        private void DockNode(DockingNode nodeToDock)
        {
            Debug.Assert(!nodeToDock.HasChildren());
            Debug.Assert(nodeToDock.Parent != null);
            Debug.Assert(!nodeToDock.Parent.HasChildren());

            if (nodeToDock.Parent.IsAncestor()) // Attaching first node to root node, special case
            {
                nodeToDock.Parent.LeftChild = nodeToDock;
                nodeToDock.Rect = nodeToDock.Parent.Rect;
                return;
            }

            DockingNode newSibling = new DockingNode();
            newSibling.Rect = nodeToDock.Parent.Rect;
            newSibling.Parent = nodeToDock.Parent;
            newSibling.AttachedPanel = nodeToDock.Parent.AttachedPanel;
            nodeToDock.Parent.AttachedPanel = null;
            Debug.Assert((newSibling.Rect.Width == nodeToDock.Rect.Width) || (newSibling.Rect.Height == nodeToDock.Rect.Height));

            bool newNodeOnTopOrLeft = true;

            if (newSibling.Rect.Width == nodeToDock.Rect.Width)
            {
                newSibling.IsHorizontallyStacked = false;
                nodeToDock.IsHorizontallyStacked = false;
                newSibling.Rect.Inflate(0, -nodeToDock.Rect.Height);
                if (newSibling.Rect.Top >= nodeToDock.Rect.Top)
                    newSibling.Rect.Offset(0, nodeToDock.Rect.Width);
                else
                    newNodeOnTopOrLeft = false;
            }
            else
            {
                newSibling.IsHorizontallyStacked = true;
                nodeToDock.IsHorizontallyStacked = true;
                newSibling.Rect.Inflate(-nodeToDock.Rect.Width, 0);
                if (newSibling.Rect.Left >= nodeToDock.Rect.Left)
                    newSibling.Rect.Offset(nodeToDock.Rect.Width, 0);
                else
                    newNodeOnTopOrLeft = false;
            }

            nodeToDock.Parent.LeftChild = newNodeOnTopOrLeft ? nodeToDock : newSibling;
            nodeToDock.Parent.RightChild = newNodeOnTopOrLeft ? newSibling : nodeToDock;
        }

        public void UndockNode(DockingNode node)
        {
            Debug.Assert(!node.HasChildren());

            if (node.Parent.IsAncestor())
            {
                node.Parent.LeftChild = null;
                return;
            }

            node.Parent.Parent.ReplaceChild(node.Parent, node.Parent.OtherChild(node));
        }

        private DockingNode GetLeafNode(Point position)
        {
            return GetLeafNode(position, m_RootNode);
        }

        private DockingNode GetLeafNode(Point position, DockingNode currentNode)
        {
            if (currentNode == null)
                return null;

            if (currentNode.LeftChild == null)
            {
                Debug.Assert(currentNode.RightChild == null);
                return currentNode;
            }

            if (currentNode.LeftChild.Rect.Contains(position))
                return GetLeafNode(position, currentNode.LeftChild);
            else 
                return GetLeafNode(position, currentNode.RightChild);
        }
    }
}
