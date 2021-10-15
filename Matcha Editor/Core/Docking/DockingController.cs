using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Matcha_Editor.Core.Docking
{
    public class DockingController
    {
        private DockingNode m_RootNode;

        public DockingController(Size size)
        {
            m_RootNode = new DockingNode{ 
                Rect = new Rect(size)
            };
        }

        public DockingNode GetDeepestNode(Point position)
        {
            return GetDeepestNode(position, m_RootNode);
        }

        private DockingNode GetDeepestNode(Point position, DockingNode currentNode)
        {
            if (currentNode.LeftChild == null)
            {
                Debug.Assert(currentNode.RightChild == null);
                return currentNode;
            }

            if (currentNode.Rect.Contains(position))
                return GetDeepestNode(position, currentNode.LeftChild);

            return GetDeepestNode(position, currentNode.RightChild);
        }

        private void Dock(DockingNode node, Rect newRect)
        {
            Debug.Assert(!node.HasChildren());
            Debug.Assert(node.Rect.Contains(newRect));
            Debug.Assert(node.Rect.Width == newRect.Width || node.Rect.Height == newRect.Height);

            if (node.Rect.Width == newRect.Width)
            {
                Debug.Assert(node.Rect.Top == newRect.Top || node.Rect.Bottom == newRect.Bottom);
                Debug.Assert(node.Rect.Left == newRect.Left && node.Rect.Right == newRect.Right);

                if (node.Rect.Top == newRect.Top)
                {
                    node.LeftChild = new DockingNode { Rect = newRect, Parent = node };
                    node.RightChild = new DockingNode { Rect = new Rect(newRect.BottomLeft, node.Rect.BottomRight), Parent = node };
                }
                else
                {
                    node.LeftChild = new DockingNode { Rect = new Rect(node.Rect.TopLeft, newRect.TopRight), Parent = node };
                    node.RightChild = new DockingNode { Rect = newRect, Parent = node };
                }
            }
            else
            {
                Debug.Assert(node.Rect.Left == newRect.Left || node.Rect.Right == newRect.Right);
                Debug.Assert(node.Rect.Top == newRect.Top && node.Rect.Bottom == newRect.Bottom);

                if (node.Rect.Left == newRect.Left)
                {
                    node.LeftChild = new DockingNode { Rect = newRect, Parent = node };
                    node.RightChild = new DockingNode { Rect = new Rect(newRect.TopRight, node.Rect.BottomRight), Parent = node};
                }
                else
                {
                    node.LeftChild = new DockingNode { Rect = new Rect(node.Rect.TopLeft, newRect.BottomLeft), Parent = node };
                    node.RightChild = new DockingNode { Rect = newRect, Parent = node};
                }
            }
        }

        private void Separate(DockingNode child)
        {
            Debug.Assert(!child.HasChildren());
            DockingNode parent = child.Parent;
            DockingNode sibling = parent.LeftChild == child ? parent.RightChild : parent.LeftChild;

            parent.Rect = sibling.Rect;
            parent.LeftChild = null;
            parent.RightChild = null;
        }
    }
}
