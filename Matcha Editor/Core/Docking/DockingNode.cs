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
            GetRootNode().NotifySplitterPropertiesChanged();
        }
        private void NotifySplitterPropertiesChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SplitterPositionX"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SplitterPositionY"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SplitterWidth"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SplitterHeight"));
            LeftChild?.NotifySplitterPropertiesChanged();
            RightChild?.NotifySplitterPropertiesChanged();
        }

        private Rect m_Rect;
        private double m_Left, m_Top, m_Width, m_Height;

        public const int SplitterThickness = 8;
        public const int MinimumSize = 200;
        public const double SubzoneRatio = 0.3;

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
        
        public double SplitterPositionX
        {
            get { return !HasBothChildren() 
                ? 0
                : LeftChild.IsHorizontallyStacked
                    ? Left + LeftChild.Width - (SplitterThickness / 2)
                    : Left;
            }
        }
        public double SplitterPositionY
        {
            get { return !HasBothChildren() 
                ? 0 
                : LeftChild.IsHorizontallyStacked
                    ? Top
                    : Top + LeftChild.Height - (SplitterThickness / 2);
            }
        }
        public double SplitterWidth
        {
            get { return !HasBothChildren() 
                ? 0 
                : LeftChild.IsHorizontallyStacked 
                    ? SplitterThickness 
                    : Width;
            }
        }
        public double SplitterHeight
        {
            get { return !HasBothChildren() 
                ? 0 
                : LeftChild.IsHorizontallyStacked 
                    ? Height 
                    : SplitterThickness;
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
                {
                    m_LeftChild.Parent = this;
                }
            }
        }
        public DockingNode RightChild
        {
            get { return m_RightChild; }
            set 
            {
                m_RightChild = value;
                if (m_RightChild != null)
                {
                    m_RightChild.Parent = this;
                }
            }
        }
        public DockingNode GetFirstNonNullChild() => LeftChild != null ? LeftChild : RightChild;

        public bool HasChildren() => LeftChild != null || RightChild != null;
        public bool HasBothChildren() => LeftChild != null && RightChild != null;
        public bool HasSingleChildren() => HasChildren() && !HasBothChildren();
        public bool IsAncestor() => Parent == null;
        public bool IsChild() => !IsAncestor();
        public bool IsRightChild() => IsChild() && Parent.RightChild == this;
        public bool IsLeftChild() => IsChild() && Parent.LeftChild == this;
        public bool IsHorizontallyStacked { get; set; }

        public DockingNode(Rect rect = new Rect())
        {
            Rect = rect;
        }

        public void ClearParent()
        {
            if (!IsAncestor())
            {
                if (IsLeftChild())
                    Parent.LeftChild = null;
                else
                    Parent.RightChild = null;
            }
                
            Parent = null;
        }

        public DockingNode GetSibling()
        {
            if (IsAncestor())
                return null;
            if (Parent.HasSingleChildren())
                return null;

            return IsLeftChild() ? Parent.RightChild : Parent.LeftChild;
        }
        public DockingNode GetRootNode()
        {
            if (IsAncestor())
                return this;

            return this.Parent.GetRootNode();
        }

        public Size GetMinimumSize()
        {
            if (!HasChildren())
                return new Size(MinimumSize, MinimumSize);

            Size minimumLeftSize = LeftChild.GetMinimumSize();
            Size minimumRightSize = RightChild.GetMinimumSize();

            if (LeftChild.IsHorizontallyStacked)
                return new Size(minimumLeftSize.Width + minimumRightSize.Width,
                    Math.Max(minimumLeftSize.Height, minimumRightSize.Height));
            else
                return new Size(Math.Max(minimumLeftSize.Width, minimumRightSize.Width),
                    minimumLeftSize.Height + minimumRightSize.Height);
        }

        public bool CanBeOfSize(Size size)
        {
            Size minSize = GetMinimumSize();
            return minSize.Width <= size.Width && minSize.Height <= size.Height;
        }

        public bool RecursiveResize(Rect newRect)
        {
            newRect.X = Math.Ceiling(newRect.X);
            newRect.Y = Math.Ceiling(newRect.Y);
            newRect.Width = Math.Ceiling(newRect.Width);
            newRect.Height = Math.Ceiling(newRect.Height);

            if (!CanBeOfSize(newRect.Size))
                return false;

            Debug.Assert(newRect.Width >= MinimumSize && newRect.Height >= MinimumSize);
            Rect = newRect;

            if (!HasChildren())
                return true;

            Debug.Assert(Parent == null || HasBothChildren(), "Single child node detected during recursive resize. Why was it not collapsed?");
            Debug.Assert(HasSingleChildren() || LeftChild.IsHorizontallyStacked == RightChild.IsHorizontallyStacked);

            Rect newLeftRect, newRightRect;
            GetNewChildRects(newRect, out newLeftRect, out newRightRect);

            Size minLeftSize = LeftChild.GetMinimumSize();
            Size minRightSize = RightChild.GetMinimumSize();

            ShiftSplitter(Math.Max(0, minLeftSize.Width - newLeftRect.Width),
                Math.Max(0, minLeftSize.Height - newLeftRect.Height));

            ShiftSplitter(-Math.Max(0, minRightSize.Width - newRightRect.Width),
                -Math.Max(0, minRightSize.Height - newRightRect.Height));

            GetNewChildRects(newRect, out newLeftRect, out newRightRect);

            Debug.Assert(LeftChild.RecursiveResize(newLeftRect));
            Debug.Assert(RightChild.RecursiveResize(newRightRect));

            return true;
        }

        private void GetNewChildRects(Rect newParentRect, out Rect newLeftRect, out Rect newRightRect)
        {
            newLeftRect = LeftChild.Rect;
            newRightRect = RightChild.Rect;
            newLeftRect.Union(newParentRect.TopLeft);
            newLeftRect.Union(LeftChild.IsHorizontallyStacked ? newParentRect.BottomLeft : newParentRect.TopRight);
            newLeftRect.Intersect(newParentRect);
            newRightRect.Union(newParentRect.BottomRight);
            newRightRect.Union(LeftChild.IsHorizontallyStacked ? newParentRect.TopRight : newParentRect.BottomLeft);
            newRightRect.Intersect(newParentRect);
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

            if (!IsAncestor())
            {
                GetFirstNonNullChild().IsHorizontallyStacked = IsHorizontallyStacked;
                GetFirstNonNullChild().Rect = Rect;

                if (this.IsLeftChild())
                    Parent.LeftChild = this.GetFirstNonNullChild();
                else
                    Parent.RightChild = this.GetFirstNonNullChild();
            }

            this.GetFirstNonNullChild().Collapse();
        }

        public bool ShiftSplitter(double deltaX, double deltaY)
        {
            if (!HasBothChildren())
                return false;

            if (deltaX == 0 && deltaY == 0)
                return true;

            Rect oldLeftRect = LeftChild.Rect;
            Rect oldRightRect = RightChild.Rect;
            Rect leftRectTarget = oldLeftRect;
            Rect rightRectTarget = oldRightRect;

            if (LeftChild.IsHorizontallyStacked)
            {
                leftRectTarget.Width = Math.Max(leftRectTarget.Width + deltaX, 0);
                rightRectTarget.Width = Math.Max(rightRectTarget.Width - deltaX, 0);
                rightRectTarget.Offset(deltaX, 0);
            }
            else
            {
                leftRectTarget.Height = Math.Max(leftRectTarget.Height + deltaY, 0);
                rightRectTarget.Height = Math.Max(rightRectTarget.Height - deltaY, 0);
                rightRectTarget.Offset(0, deltaY);
            }

            bool leftResized = LeftChild.RecursiveResize(leftRectTarget);
            bool rightResized = RightChild.RecursiveResize(rightRectTarget);

            if (leftResized && rightResized)
                return true;

            // Reset and attempt to have parent shift instead
            RightChild.RecursiveResize(oldRightRect);
            LeftChild.RecursiveResize(oldLeftRect);

            bool towardsRight = LeftChild.IsHorizontallyStacked && deltaX > 0 ||
                                !LeftChild.IsHorizontallyStacked && deltaY > 0;
            bool stackmode = LeftChild.IsHorizontallyStacked;
            DockingNode parent = Parent;
            DockingNode previous = this;
            if (towardsRight)
            {
                LeftChild.RecursiveResize(oldLeftRect);

                while (parent != null)
                {
                    if (parent.LeftChild == previous)
                        if (parent.LeftChild.IsHorizontallyStacked == stackmode)
                            if (parent.ShiftSplitter(deltaX, deltaY))
                                return ShiftSplitter(deltaX, deltaY);

                    previous = parent;
                    parent = parent.Parent;
                }
            }
            else
            {
                // Left side is getting smaller. Right side will definitely have passed.
                // Undo right

                while (parent != null)
                {
                    if (parent.RightChild == previous)
                    {
                        if (parent.LeftChild.IsHorizontallyStacked == stackmode)
                        {
                            if (parent.ShiftSplitter(deltaX, deltaY))
                                return ShiftSplitter(deltaX, deltaY);
                        }
                    }
                    previous = parent;
                    parent = parent.Parent;
                }
            }


            // End attempt
            LeftChild.RecursiveResize(oldLeftRect);
            RightChild.RecursiveResize(oldRightRect);
            return false;
        }

        public DockingNode GetSubzone(Point position)
        {
            if (!Rect.Contains(position))
                return null;

            DockingNode[] subzones = {
                GetLeftSubzone(), GetRightSubzone(), GetTopSubzone(), GetBottomSubzone()
            };

            for (int i = 0; i < 4; ++i)
                if (subzones[i].Rect.Contains(position))
                    if (subzones[i].Width >= MinimumSize && subzones[i].Height >= MinimumSize)
                        return subzones[i];

            return null;
        }

        private DockingNode GetLeftSubzone()
        {
            Rect subzone = Rect;
            double idealWidth = Width * SubzoneRatio;
            subzone.Width = Math.Min(Math.Max(idealWidth, MinimumSize), Width / 2);
            return new DockingNode { Rect = subzone, Parent = this };
        }
        private DockingNode GetRightSubzone()
        {
            Rect subzone = Rect;
            double idealWidth = Width * SubzoneRatio;
            subzone.Width = Math.Min(Math.Max(idealWidth, MinimumSize), Width / 2);
            subzone.Offset(Rect.Width - subzone.Width, 0);
            return new DockingNode { Rect = subzone, Parent = this };
        }
        private DockingNode GetTopSubzone()
        {
            Rect subzone = Rect;
            double idealHeight = Height * SubzoneRatio;
            subzone.Height = Math.Min(Math.Max(idealHeight, MinimumSize), Height / 2);
            return new DockingNode { Rect = subzone, Parent = this };
        }
        private DockingNode GetBottomSubzone()
        {
            Rect subzone = Rect;
            double idealHeight = Height * SubzoneRatio;
            subzone.Height = Math.Min(Math.Max(idealHeight, MinimumSize), Height / 2);
            subzone.Offset(0, Rect.Height - subzone.Height);
            return new DockingNode { Rect = subzone, Parent = this };
        }
    }
}
