using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Linq;

namespace Matcha_Editor.Core
{
    public class EditorWindowManager
    {
        public static EditorWindowManager m_Instance;
        public static EditorWindowManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new EditorWindowManager();

                return m_Instance;
            }
        }

        private IList<UserControl> m_RegisteredWindows;

        private EditorWindowManager()
        {
            m_RegisteredWindows = new List<UserControl>();
        }

        public void RegisterWindow(UserControl window)
        {
            m_RegisteredWindows.Add(window);
        }

        public void DeregisterWindow(UserControl window)
        {
            m_RegisteredWindows.Remove(window);
        }

        public IList<UserControl> GetWindowsOfType<T>()
        {
            return m_RegisteredWindows.Where(x => x is T).ToList<UserControl>();
        }
    }
}
