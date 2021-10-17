using Matcha_Editor.MVVM.View;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Matcha_Editor.Core.Docking
{
    public class DockingNode
    {
        public Rect Rect { get; set; }
        public DockingNode Parent { get; set; }
        public DockingNode LeftChild { get; set; }
        public DockingNode RightChild { get; set; }
        public DockingPanelView AttachedPanel { get; set; }
        public bool IsHorizontallyStacked { get; set; }

        private const double m_SubzoneRatio = 0.3;

        public bool HasChildren() => LeftChild != null || RightChild != null;
        
        public bool HasBothChildren() => LeftChild != null && RightChild != null;

        // This should only be true when the node is right about to be collapsed.
        public bool HasSingleChildren() => HasChildren() || HasBothChildren();

        public bool IsAncestor() => Parent == null;

        public bool HasUIAttached() => AttachedPanel != null;

        public DockingNode OtherChild(DockingNode child) => LeftChild == child ? RightChild : LeftChild;

        public void ReplaceChild(DockingNode oldChild, DockingNode newChild)
        {
            Debug.Assert(oldChild == LeftChild || oldChild == RightChild);

            if (oldChild == LeftChild)
                LeftChild = newChild;
            else
                RightChild = newChild;

            if (newChild != null)
                newChild.Parent = this;
        }

        public DockingNode RecursivelyFindNode(DockingPanelView targetView)
        {
            if (AttachedPanel == targetView)
                return this;

            if (!HasChildren())
                return null;

            DockingNode node = LeftChild.RecursivelyFindNode(targetView);
            if (node != null)
                return node;
            else 
                return RightChild?.RecursivelyFindNode(targetView);
        }

        public DockingNode GetSubzone(Point position)
        {
            if (!Rect.Contains(position))
                return null;

            double distanceToTop = position.Y - Rect.Top;
            double distanceToLeft = position.X - Rect.Left;
            double distanceToBottom = Rect.Bottom - position.Y;
            double distanceToRight = Rect.Right - position.X;
            double shortestDistance = Math.Min(distanceToTop, Math.Min(distanceToLeft, Math.Min(distanceToBottom, distanceToRight)));

            if (shortestDistance > Math.Min(Rect.Width, Rect.Height) * m_SubzoneRatio)
                return null;

            Rect subzone = Rect;

            if (shortestDistance == distanceToLeft)
            {
                subzone.Width *= m_SubzoneRatio;
            }
            else if (shortestDistance == distanceToRight)
            {
                subzone.Width *= m_SubzoneRatio;
                subzone.Offset(Rect.Width - subzone.Width, 0);
            }
            else if (shortestDistance == distanceToTop)
            {
                subzone.Height *= m_SubzoneRatio;
            }
            else if (shortestDistance == distanceToBottom)
            {
                subzone.Height *= m_SubzoneRatio;
                subzone.Offset(0, Rect.Height - subzone.Height);
            }

            return new DockingNode { Rect = subzone, Parent = this };
        }

        public void Collapse()
        {
            Debug.Assert(!HasChildren());
            Debug.Assert(!IsAncestor());

            if (Parent.IsAncestor())
                return; // Tree is now empty

            DockingNode remainingChild = Parent.LeftChild != null ? Parent.LeftChild : Parent.RightChild;
            remainingChild.RecursiveResize(Parent.Rect);
            remainingChild.IsHorizontallyStacked = Parent.IsHorizontallyStacked;
            Parent.Parent.ReplaceChild(Parent, remainingChild);
        }

        private void ResizeAttachedUIPanel()
        {
            if (AttachedPanel != null)
            {
                Canvas.SetLeft(AttachedPanel, Rect.Left);
                Canvas.SetTop(AttachedPanel, Rect.Top);
                AttachedPanel.Width = Rect.Width;
                AttachedPanel.Height = Rect.Height;
            }
        }

        public void RecursiveResize(Rect newRect)
        {
            Rect oldRect = Rect;
            Rect = newRect;

            ResizeAttachedUIPanel();

            if (!HasChildren())
                return;

            Debug.Assert(Parent == null || HasBothChildren(), "Single child node detected during recursive resize. Why was it not collapsed?");
            Debug.Assert(HasSingleChildren() || LeftChild.IsHorizontallyStacked == RightChild.IsHorizontallyStacked);

            Rect newLeftRect = newRect;
            Rect newRightRect = newRect;

            if (LeftChild.IsHorizontallyStacked)
            {
                double totalWidth = LeftChild.Rect.Width + (RightChild == null ? 0 : RightChild.Rect.Width);
                // TODO: Force this assert when ugly base case is removed
                //Debug.Assert(totalWidth == LeftChild.Parent.Rect.Width);
                newLeftRect.Width = (int)(LeftChild.Rect.Width / totalWidth * newRect.Width);
                newRightRect.Width = (int)newRect.Width - newLeftRect.Width;
                newRightRect.Offset(newLeftRect.Width, 0);
            }
            else
            {
                double totalHeight = LeftChild.Rect.Height + (RightChild == null ? 0 : RightChild.Rect.Height);
                //Debug.Assert(totalHeight == LeftChild.Parent.Rect.Height);
                newLeftRect.Height = (int)(LeftChild.Rect.Height / totalHeight * newRect.Height);
                newRightRect.Height = (int)newRect.Height - newLeftRect.Height;
                newRightRect.Offset(0, newLeftRect.Height);
            }

            LeftChild.RecursiveResize(newLeftRect);
            RightChild?.RecursiveResize(newRightRect);
        }
    }
}
