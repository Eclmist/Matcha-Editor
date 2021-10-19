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
        public struct StackingProperties
        {
            public bool IsHorizontallyStacked { get; set; }
            public bool IsNewNodeFirst { get; set; }
        }

        private DockingNode m_RootNode;

        public Size Size
        {
            get { return m_Size; }
            set
            {
                m_Size = value;
                RefreshTreeSize();
            }
        } 

        private Size m_Size;

        public DockingLayoutManager(Size size)
        {
            Size = size;
        }

        public DockingNode AddNewNode(Point position = new Point())
        {
            if (m_RootNode == null)
            {
                m_RootNode = new DockingNode(new Rect(Size));
                return m_RootNode;
            }

            DockingNode newNode = GetPreviewNode(position);
            DockNode(newNode, newNode.Parent);
            RefreshTreeSize();
            return newNode;
        }
        
        public void MoveNode(DockingNode originalNode, DockingNode previewNode)
        {
            originalNode.Rect = previewNode.Rect;
            DockNode(originalNode, previewNode.Parent);
            RefreshTreeSize();
        }

        public void RemoveNode(DockingNode node)
        {
            Debug.Assert(!node.HasChildren());

            if (node.IsAncestor())
            {
                m_RootNode = null;
                return;
            }

            if (node.Parent.IsAncestor())
            {
                SetRootNode(node.GetSibling());
            }
            else
            {
                if (node.IsLeftChild())
                    node.Parent.LeftChild = null;
                else
                    node.Parent.RightChild = null;
            }

            m_RootNode.Collapse();
            RefreshTreeSize();
        }

        public DockingNode GetPreviewNode(Point position)
        {
            DockingNode leafNode = GetLeafNode(position);
            return leafNode?.GetSubzone(position);
        }

        public void Resize(Size newSize)
        {
            Size = newSize;

            if (m_RootNode == null)
                return;

            Rect newRect = new Rect(newSize);
            m_RootNode.Rect = newRect;
            RefreshTreeSize();
        }

        private void RefreshTreeSize()
        {
            m_RootNode?.RecursiveResize(new Rect(Size));
        }

        private void SetRootNode(DockingNode node)
        {
            m_RootNode = node;
            m_RootNode?.ClearParent();
        }

        private void DockNode(DockingNode node, DockingNode targetParent)
        {
            Debug.Assert(!node.HasChildren());
            Debug.Assert(targetParent != null);
            Debug.Assert(!targetParent.HasChildren());

            DockingNode newParentNode = new DockingNode();
            newParentNode.IsHorizontallyStacked = targetParent.IsHorizontallyStacked;
            newParentNode.Rect = targetParent.Rect;

            if (targetParent.IsAncestor())
                SetRootNode(newParentNode);

            if (targetParent.IsLeftChild())
                targetParent.Parent.LeftChild = newParentNode;
            else if (targetParent.IsRightChild())
                targetParent.Parent.RightChild = newParentNode;

            DockingNode op = node.Parent;
            DockingNode siblingNode = targetParent;
            StackingProperties props = ComputeStackingProperties(node, newParentNode);
            siblingNode.IsHorizontallyStacked = props.IsHorizontallyStacked;
            node.IsHorizontallyStacked = props.IsHorizontallyStacked;

            if (props.IsHorizontallyStacked)
            {
                siblingNode.Width = newParentNode.Width - node.Width;
                siblingNode.Left += props.IsNewNodeFirst ? node.Width : 0;
                node.Left += props.IsNewNodeFirst ? 0 : siblingNode.Left;

            }
            else
            {
                siblingNode.Height = newParentNode.Height - node.Height;
                siblingNode.Top += props.IsNewNodeFirst ? node.Height : 0;
                node.Top += props.IsNewNodeFirst ? 0 : siblingNode.Top;
            }

            newParentNode.LeftChild = props.IsNewNodeFirst ? node : siblingNode;
            newParentNode.RightChild = props.IsNewNodeFirst ? siblingNode : node;

            if (op.IsAncestor())
                SetRootNode(op.LeftChild == node ? op.RightChild : op.LeftChild);

            if (op.LeftChild == node)
                op.LeftChild = null;
            else
                op.RightChild = null;

            m_RootNode.Collapse();
        }

        private StackingProperties ComputeStackingProperties(DockingNode node, DockingNode parent)
        {
            bool isHorizontallyStacked = node.Rect.Width < parent.Rect.Width;
            bool newNodeFirst =
                (!isHorizontallyStacked && node.Rect.Top == parent.Rect.Top) ||
                (isHorizontallyStacked && node.Rect.Left == parent.Rect.Left);

            return new StackingProperties
            {
                IsHorizontallyStacked = isHorizontallyStacked,
                IsNewNodeFirst = newNodeFirst
            };
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
