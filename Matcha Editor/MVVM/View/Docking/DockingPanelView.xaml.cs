using Matcha_Editor.Core.Docking;
using System.Windows.Controls;
using System.Windows.Data;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingPanelView : UserControl
    {
        public DockingNode DockingNode { get; set; }

        public DockingPanelView(DockingNode dockingNode)
        {
            InitializeComponent();
            DockingNode = dockingNode;
            Tab.ParentPanel = this;

            Binding canvasTopBinding = new Binding("Top");
            Binding canvasLeftBinding = new Binding("Left");
            Binding widthBinding = new Binding("Width");
            Binding heightBinding = new Binding("Height");

            canvasTopBinding.Source = DockingNode;
            canvasLeftBinding.Source = DockingNode;
            widthBinding.Source = DockingNode;
            heightBinding.Source = DockingNode;

            SetBinding(Canvas.TopProperty, canvasTopBinding);
            SetBinding(Canvas.LeftProperty, canvasLeftBinding);
            SetBinding(WidthProperty, widthBinding);
            SetBinding(HeightProperty, heightBinding);
        }
    }
}
