using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Microsoft.Win32.SafeHandles;

namespace Matcha_Editor.Core.Docking
{
    public class DockingLayoutManager
    {
        public struct StackingProperties
        {
            public bool IsHorizontallyStacked { get; set; }
            public bool IsNewNodeFirst { get; set; }
        }
        public enum DockPosition
        {
            Top, Left, Bottom, Right
        }

        private Dictionary<UIElement, DockingNode> m_RootNodes;
        private Dictionary<DockingNode, UIElement> m_RootContainers;

        private static DockingLayoutManager m_Instance;
        public static DockingLayoutManager Instance
        {
            get
            {
                if (m_Instance != null)
                    return m_Instance;

                m_Instance = new DockingLayoutManager();
                return m_Instance;
            }
        }

        public DockingLayoutManager()
        {
            m_RootNodes = new Dictionary<UIElement, DockingNode>();
            m_RootContainers = new Dictionary<DockingNode, UIElement>();
        }

        public DockingNode AddNewNode(UIElement rootContainer, Point position = new Point())
        {
            DockingNode rootNode;
            m_RootNodes.TryGetValue(rootContainer, out rootNode);

            if (rootNode == null)
            {
                DockingNode newRootNode = new DockingNode(new Rect(rootContainer.RenderSize));
                m_RootNodes.Add(rootContainer, newRootNode);
                m_RootContainers.Add(newRootNode, rootContainer);
                return newRootNode;
            }

            DockingNode newNode = GetPreviewNode(position, rootContainer);
            if (newNode != null)
            {
                DockNode(newNode, newNode.Parent);
                newNode.GetRootNode().RecursiveResize();
            }

            return newNode;
        }

        public DockingNode AddNode(UIElement rootContainer, DockingNode parent = null, DockPosition position = DockPosition.Left)
        {
            if (parent == null)
            {
                DockingNode newRootNode = new DockingNode(new Rect(rootContainer.RenderSize));
                m_RootNodes.Add(rootContainer, newRootNode);
                m_RootContainers.Add(newRootNode, rootContainer);
                return newRootNode;
            }

            DockingNode newNode = GetPreviewNode(parent, position);
            if (newNode != null)
            {
                DockNode(newNode, parent);
                parent.RecursiveResize();
            }

            return newNode;
        }
        
        public void RemoveNode(DockingNode node)
        {
            Debug.Assert(!node.HasChildren());

            if (node.IsAncestor())
            {
                RemoveRootNode(node);
                return;
            }

            if (node.Parent.IsAncestor())
            {
                DockingNode newRootNode = node.GetSibling();
                newRootNode.Rect = node.Parent.Rect;
                SetRootNode(newRootNode, m_RootContainers[node.Parent]);
                newRootNode.Collapse();
                newRootNode.RecursiveResize();
            }
            else
            {
                if (node.IsLeftChild())
                    node.Parent.LeftChild = null;
                else
                    node.Parent.RightChild = null;

                node.GetRootNode().Collapse();
                node.GetRootNode().RecursiveResize();
            }
        }

        public void RemoveRootNode(DockingNode node)
        {
            Debug.Assert(node.IsAncestor());
            Debug.Assert(m_RootNodes.Remove(m_RootContainers[node]));
            Debug.Assert(m_RootContainers.Remove(node));
        }

        public UIElement[] GetRootContainers()
        {
            return m_RootNodes.Keys.ToArray();
        }

        public UIElement GetRootContainer(DockingNode node)
        {
            UIElement container = null;
            m_RootContainers.TryGetValue(node.GetRootNode(), out container);
            return container;
        }

        public bool HasNodes(UIElement rootContainer) => GetRootNode(rootContainer) != null;

        public DockingNode GetRootNode(UIElement rootContainer)
        {
            DockingNode root = null;
            m_RootNodes.TryGetValue(rootContainer, out root);
            return root;
        }

        public DockingNode GetPreviewNode(Point position, UIElement rootContainer)
        {
            DockingNode leafNode = GetLeafNode(position, rootContainer);
            return leafNode?.GetSubzone(position);
        }

        public DockingNode GetPreviewNode(DockingNode node, DockPosition pos)
        {
            switch (pos)
            {
                case DockPosition.Top:
                    return node.GetTopSubzone();
                case DockPosition.Left:
                    return node.GetLeftSubzone();
                case DockPosition.Bottom:
                    return node.GetBottomSubzone();
                case DockPosition.Right:
                    return node.GetRightSubzone();
                default:
                    return null;
            }
        }

        public void Resize(Size newSize, DockingNode node)
        {
            Rect newRect = new Rect(newSize);
            node.Rect = newRect;
            node.RecursiveResize();
        }

        private void SetRootNode(DockingNode node, UIElement rootContainer)
        {
            node.ClearParent();
            DockingNode oldRoot = m_RootNodes[rootContainer];
            Debug.Assert(m_RootContainers.Remove(oldRoot));
            m_RootContainers[node] = rootContainer;
            m_RootNodes[rootContainer] = node;
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
                SetRootNode(newParentNode, m_RootContainers[targetParent]);

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
            }
            else
            {
                siblingNode.Height = newParentNode.Height - node.Height;
                siblingNode.Top += props.IsNewNodeFirst ? node.Height : 0;
            }

            newParentNode.LeftChild = props.IsNewNodeFirst ? node : siblingNode;
            newParentNode.RightChild = props.IsNewNodeFirst ? siblingNode : node;

            if (op != null)
            {
                DockingNode newRoot = op.LeftChild == node ? op.RightChild : op.LeftChild;
                if (op.IsAncestor())
                {
                    SetRootNode(newRoot, m_RootContainers[op]);
                    newRoot.Rect = op.Rect;
                }

                if (op.LeftChild == node)
                    op.LeftChild = null;
                else
                    op.RightChild = null;
            }

            node.GetRootNode().Collapse();
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

        private DockingNode GetLeafNode(Point position, UIElement rootContainer)
        {
            return GetLeafNode(position, m_RootNodes[rootContainer]);
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
