using Matcha_Editor.Core.RenderSurface;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Matcha_Editor.MVVM.View
{
    public partial class RenderSurfaceView : UserControl, IDisposable
    {
        private RenderSurfaceHost m_Host = null;
        private bool m_DisposedValue;

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

        protected virtual void Dispose(bool disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                    m_Host.Dispose();

                m_DisposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
