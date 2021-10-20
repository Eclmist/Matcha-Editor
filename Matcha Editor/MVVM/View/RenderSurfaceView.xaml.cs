using Matcha_Editor.Core.RenderSurface;
using System.Windows;
using System.Windows.Controls;

namespace Matcha_Editor.MVVM.View
{
    public partial class RenderSurfaceView : UserControl
    {
        private RenderSurfaceHost m_Host = null;

        public RenderSurfaceView()
        {
            InitializeComponent();
            Loaded += OnRenderSurfaceViewLoaded;
        }
        private void OnRenderSurfaceViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnRenderSurfaceViewLoaded;
            m_Host = new RenderSurfaceHost();
            Content = m_Host;
        }
    }
}
