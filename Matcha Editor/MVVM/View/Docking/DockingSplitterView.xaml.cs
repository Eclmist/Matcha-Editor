using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Matcha_Editor.Core.Docking;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingSplitterView : UserControl
    {

        public DockingNode DockingNode { get; set; }

        public DockingSplitterView(DockingNode node)
        {
            InitializeComponent();
            DockingNode = node;

            Binding canvasTopBinding = new Binding("SplitterPositionY");
            Binding canvasLeftBinding = new Binding("SplitterPositionX");
            Binding widthBinding = new Binding("SplitterWidth");
            Binding heightBinding = new Binding("SplitterHeight");

            canvasTopBinding.Source = DockingNode;
            canvasLeftBinding.Source = DockingNode;
            widthBinding.Source = DockingNode;
            heightBinding.Source = DockingNode;

            SetBinding(Canvas.TopProperty, canvasTopBinding);
            SetBinding(Canvas.LeftProperty, canvasLeftBinding);
            SetBinding(WidthProperty, widthBinding);
            SetBinding(HeightProperty, heightBinding);

            MouseEnter += Splitter_MouseEnter;
            MouseLeave += Splitter_MouseLeave;
            PreviewMouseDown += Splitter_MouseDown;
            PreviewMouseUp += Splitter_MouseUp;
            PreviewMouseMove += Splitter_MouseMove;
        }

        private bool m_IsMouseDown;
        private Point m_MouseDownPos;
        private bool disposedValue;

        private void Splitter_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = DockingNode.LeftChild.IsHorizontallyStacked ? Cursors.SizeWE : Cursors.SizeNS;
        }

        private void Splitter_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Splitter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_IsMouseDown = true;
            m_MouseDownPos = e.GetPosition(sender as IInputElement);
            CaptureMouse();
        }

        private void Splitter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_IsMouseDown = false;
            ReleaseMouseCapture();
        }

        private void Splitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_IsMouseDown)
                return;

            Point newPosition = e.GetPosition(sender as IInputElement);

            if ((newPosition - m_MouseDownPos).Length <= 0)
                return;

            double deltaX = !DockingNode.LeftChild.IsHorizontallyStacked ? 0 : newPosition.X - m_MouseDownPos.X;
            double deltaY = DockingNode.LeftChild.IsHorizontallyStacked ? 0 : newPosition.Y - m_MouseDownPos.Y;

            Rect oldLeftRect = DockingNode.LeftChild.Rect;
            Rect oldRightRect = DockingNode.RightChild.Rect;
            Rect leftRectTarget = oldLeftRect;
            Rect rightRectTarget = oldRightRect;

            leftRectTarget.Width = Math.Max(leftRectTarget.Width + deltaX, 0);
            leftRectTarget.Height = Math.Max(leftRectTarget.Height + deltaY, 0);

            rightRectTarget.Width = Math.Max(rightRectTarget.Width - deltaX, 0);
            rightRectTarget.Height = Math.Max(rightRectTarget.Height - deltaY, 0);
            rightRectTarget.Offset(deltaX, deltaY);

            if (DockingNode.LeftChild.RecursiveResize(leftRectTarget))
                if (DockingNode.RightChild.RecursiveResize(rightRectTarget))
                    return;

            // Revert to old rects because resize failed (one side was too small)
            DockingNode.LeftChild.RecursiveResize(oldLeftRect);
            DockingNode.RightChild.RecursiveResize(oldRightRect);
        }
    }
}
