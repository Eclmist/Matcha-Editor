using Matcha_Editor.Core;

namespace Matcha_Editor.MVVM.View
{
    public partial class ViewportView : ViewBase
    {
        public ViewportView()
        {
            InitializeComponent();
            EditorWindowManager.Instance.RegisterWindow(this);
        }
    }
}
