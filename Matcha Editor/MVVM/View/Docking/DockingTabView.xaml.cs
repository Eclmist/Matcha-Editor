using Matcha_Editor.Core.Docking;
using System.Windows.Controls;
using System.Windows.Media;

namespace Matcha_Editor.MVVM.View
{
    public partial class DockingTabView : UserControl
    {
        public DockingPanelView ParentPanel { get; set; }
        public string TitleText { get; set; }
        public DockingPanelTab Tab { get; set; }

        private SolidColorBrush activeBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x29));
        private SolidColorBrush inactiveBrush = new SolidColorBrush(Color.FromRgb(0x1e, 0x1e, 0x21));

        public DockingTabView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public DockingTabView(DockingPanelTab tab)
        {
            InitializeComponent();
            DataContext = this;

            tab.TabView = this;
            PreviewMouseUp += (s, e) => tab.Select();
        }

        public void ToggleVisibility(bool visible)
        {
            this.Opacity = visible ? 1 : 0;
        }

        public void SetActiveStyle(bool active)
        {
            Main.Background = active ? activeBrush : inactiveBrush;
        }
    }
}
