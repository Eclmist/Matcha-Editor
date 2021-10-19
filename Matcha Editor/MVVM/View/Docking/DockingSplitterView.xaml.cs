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

            double deltaX = newPosition.X - m_MouseDownPos.X;
            double deltaY = newPosition.Y - m_MouseDownPos.Y;


            if (DockingNode.LeftChild.IsHorizontallyStacked)
            {
                double newLeftWidth = DockingNode.LeftChild.Width;
                double newRightWidth = DockingNode.RightChild.Width;

                newLeftWidth += deltaX;
                newRightWidth -= deltaX;

                DockingNode.LeftChild.Width = newLeftWidth;
                DockingNode.RightChild.Width = newRightWidth;

                DockingNode.RecursiveResize(DockingNode.Rect);
            }
            else
            {
                double newLeftWidth = DockingNode.LeftChild.Height;
                double newRightWidth = DockingNode.RightChild.Height;

                newLeftWidth += deltaY;
                newRightWidth -= deltaY;

                DockingNode.LeftChild.Height = newLeftWidth;
                DockingNode.RightChild.Height = newRightWidth;

                DockingNode.RecursiveResize(DockingNode.Rect);
            }
        }
    }
}
