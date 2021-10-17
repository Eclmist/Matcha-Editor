using Matcha_Editor.MVVM.View;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Matcha_Editor.Core.Docking
{
    public class DockingNode : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Rect m_Rect;
        private double m_Left, m_Top, m_Width, m_Height;

        public Rect Rect { 
            get { return m_Rect; }
            set
            {
                m_Rect = value;
                Left = value.Left;
                Top = value.Top;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public double Left
        {
            get { return m_Left; }
            set
            {
                if (value != m_Left)
                {
                    m_Left = value;
                    m_Rect.X = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double Top
        {
            get { return m_Top; }
            set
            {
                if (value != m_Top)
                {
                    m_Top = value;
                    m_Rect.Y = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double Width
        {
            get { return m_Width; }
            set
            {
                if (value != m_Width)
                {
                    m_Width = value;
                    m_Rect.Width = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double Height
        {
            get { return m_Height; }
            set
            {
                if (value != m_Height)
                {
                    m_Height = value;
                    m_Rect.Height = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DockingNode m_LeftChild, m_RightChild;

        public DockingNode Parent { get; private set; }

        public DockingNode LeftChild
        {
            get { return m_LeftChild; }
            set 
            {
                m_LeftChild = value;
                if (m_LeftChild != null)
                    m_LeftChild.Parent = this;
            }
        }

        public DockingNode RightChild
        {
            get { return m_RightChild; }
            set 
            {
                m_RightChild = value;
                if (m_RightChild != null)
                    m_RightChild.Parent = this;
            }
        }

        public bool IsHorizontallyStacked { get; set; }

        public const double m_SubzoneRatio = 0.3;

        public DockingNode(Rect rect = new Rect())
        {
            Rect = rect;
        }

        public bool HasChildren() => LeftChild != null || RightChild != null;
        
        public bool HasBothChildren() => LeftChild != null && RightChild != null;

        // This should only be true when the node is right about to be collapsed.
        public bool HasSingleChildren() => HasChildren() || HasBothChildren();

        public bool IsAncestor() => Parent == null;

        public bool IsChild(DockingNode node) => node == LeftChild || node == RightChild;

        public bool IsRightChild(DockingNode child) => IsChild(child) && child == RightChild;

        public bool IsLeftChild(DockingNode child) => IsChild(child) && child == LeftChild;

        public DockingNode GetFirstChild() => LeftChild != null ? LeftChild : RightChild;

        public void ClearParent()
        {
            if (Parent != null)
            {
                if (Parent.IsLeftChild(this))
                    Parent.LeftChild = null;
                else
                    Parent.RightChild = null;
            }
                
            Parent = null;
        }

        public DockingNode GetSibling()
        {
            if (Parent == null)
                return null;
            if (Parent.HasSingleChildren())
                return null;

            return Parent.LeftChild == this ? Parent.RightChild : Parent.LeftChild;
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

            if (!IsInPaddingZone(position))
                return null;
            if (shortestDistance == distanceToLeft) 
                return GetLeftSubzone();
            else if (shortestDistance == distanceToRight) 
                return GetRightSubzone();
            else if (shortestDistance == distanceToTop) 
                return GetTopSubzone();
            else
                return GetBottomSubzone();
        }

        public void RecursiveResize(Rect newRect)
        {
            Rect = newRect;

            if (!HasChildren())
                return;

            Debug.Assert(Parent == null || HasBothChildren(), "Single child node detected during recursive resize. Why was it not collapsed?");
            Debug.Assert(HasSingleChildren() || LeftChild.IsHorizontallyStacked == RightChild.IsHorizontallyStacked);

            Rect newLeftRect = newRect;
            Rect newRightRect = newRect;

            if (LeftChild.IsHorizontallyStacked)
            {
                double totalWidth = LeftChild.Rect.Width + (RightChild == null ? 0 : RightChild.Rect.Width);
                newLeftRect.Width = (int)(LeftChild.Rect.Width / totalWidth * newRect.Width);
                newRightRect.Width = (int)newRect.Width - newLeftRect.Width;
                newRightRect.Offset(newLeftRect.Width, 0);
            }
            else
            {
                double totalHeight = LeftChild.Rect.Height + (RightChild == null ? 0 : RightChild.Rect.Height);
                newLeftRect.Height = (int)(LeftChild.Rect.Height / totalHeight * newRect.Height);
                newRightRect.Height = (int)newRect.Height - newLeftRect.Height;
                newRightRect.Offset(0, newLeftRect.Height);
            }

            LeftChild.RecursiveResize(newLeftRect);
            RightChild?.RecursiveResize(newRightRect);
        }

        public void Collapse()
        {
            if (!HasChildren())
                return;

            if (HasBothChildren())
            {
                LeftChild.Collapse();
                RightChild.Collapse();
                return;
            }

            if (Parent != null)
            {
                GetFirstChild().IsHorizontallyStacked = IsHorizontallyStacked;

                if (Parent.IsLeftChild(this))
                    Parent.LeftChild = GetFirstChild();
                else
                    Parent.RightChild = GetFirstChild();
            }

            GetFirstChild().Collapse();
        }

        private DockingNode GetLeftSubzone()
        {
            Rect subzone = Rect;
            subzone.Width *= m_SubzoneRatio;
            return new DockingNode { Rect = subzone, Parent = this };
        }

        private DockingNode GetRightSubzone()
        {
            Rect subzone = Rect;
            subzone.Width *= m_SubzoneRatio;
            subzone.Offset(Rect.Width - subzone.Width, 0);
            return new DockingNode { Rect = subzone, Parent = this };
        }

        private DockingNode GetTopSubzone()
        {
            Rect subzone = Rect;
            subzone.Height *= m_SubzoneRatio;
            return new DockingNode { Rect = subzone, Parent = this };
        }

        private DockingNode GetBottomSubzone()
        {
            Rect subzone = Rect;
            subzone.Height *= m_SubzoneRatio;
            subzone.Offset(0, Rect.Height - subzone.Height);
            return new DockingNode { Rect = subzone, Parent = this };
        }
        
        private bool IsInPaddingZone(Point position)
        {
            bool inLeft = position.X - Left < m_SubzoneRatio * Width;
            bool inRight = position.X - Left > (1 - m_SubzoneRatio) * Width;
            bool inTop = position.Y - Top < m_SubzoneRatio * Height;
            bool inBottom = position.Y - Top > (1 - m_SubzoneRatio) * Height;

            return inLeft || inRight || inTop || inBottom;
        }

    }
}
