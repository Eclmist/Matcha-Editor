using Matcha_Editor.Core.Docking;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingContainerView : UserControl
    {
        private DockingController m_DockingController;

        public DockingContainerView()
        {
            InitializeComponent();

            DockingPanelView mvp = new DockingPanelView
            {
                Width = 130,
                MinWidth = 50,
                Height = 130,
                MinHeight = 40
            };

            mvp.Tab1.Title.Content = "DockableTab";

            Canvas.SetLeft(mvp, 20);
            Canvas.SetTop(mvp, 20);

            LayoutRoot.Children.Add(mvp);
        }

        private void AddDefaultPanels()
        {

        }
    }
}
