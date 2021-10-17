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

        private void DockNode(DockingNode newNode)
        {
            Debug.Assert(!newNode.HasChildren());
            Debug.Assert(newNode.Parent != null);
            Debug.Assert(!newNode.Parent.HasChildren());

            if (newNode.Parent.IsAncestor()) // Attaching first node to root node, special case
            {
                newNode.Parent.LeftChild = newNode;
                newNode.Rect = newNode.Parent.Rect;
                return;
            }

            DockingNode newSibling = new DockingNode();
            newSibling.Parent = newNode.Parent;
            newSibling.AttachedPanel = newNode.Parent.AttachedPanel;
            newNode.Parent.AttachedPanel = null;

            bool isHorizontallyStacked = newNode.Rect.Width < newNode.Parent.Rect.Width;
            bool newNodeOnTopOrLeft = 
                (!isHorizontallyStacked && newNode.Rect.Top == newNode.Parent.Rect.Top) ||
                (isHorizontallyStacked && newNode.Rect.Left == newNode.Parent.Rect.Left);
            newSibling.IsHorizontallyStacked = isHorizontallyStacked;
            newNode.IsHorizontallyStacked = isHorizontallyStacked;

            Rect newNodeRect = newNode.Parent.Rect;
            Rect newSiblingRect = newNodeRect;

            if (isHorizontallyStacked)
            {
                newNodeRect.Width /= 2;
                newSiblingRect.Width /= 2;

                newNodeRect.X += newNodeOnTopOrLeft ? 0 : newSiblingRect.Width;
                newSiblingRect.X += newNodeOnTopOrLeft ? newNodeRect.Width : 0;
            }
            else
            {
                newNodeRect.Height /= 2;
                newSiblingRect.Height /= 2;

                newNodeRect.Y += newNodeOnTopOrLeft ? 0 : newSiblingRect.Height;
                newSiblingRect.Y += newNodeOnTopOrLeft ? newNodeRect.Height : 0;
            }

            newNode.Rect = newNodeRect;
            newSibling.Rect = newSiblingRect;
            newNode.Parent.LeftChild = newNodeOnTopOrLeft ? newNode : newSibling;
            newNode.Parent.RightChild = newNodeOnTopOrLeft ? newSibling : newNode;
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
