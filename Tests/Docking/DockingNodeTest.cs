using Matcha_Editor.Core.Docking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;

namespace Tests
{
    [TestClass]
    public class DockingNodeTest
    {
        [TestMethod]
        public void CanCreateRootNode()
        {
            DockingNode node = new DockingNode();
            Assert.IsTrue(node.IsAncestor());
            Assert.IsTrue(node.Rect == new Rect());

            node = new DockingNode(new Rect(100, 200, 300, 400));
            Assert.IsTrue(node.Rect == new Rect(100, 200, 300, 400));
        }

        [TestMethod]
        public void CanQueryNumChildren()
        {
            DockingNode node = new DockingNode();
            Assert.IsFalse(node.HasChildren());
            Assert.IsFalse(node.HasBothChildren());
            Assert.IsFalse(node.HasSingleChildren());

            node.LeftChild = new DockingNode();
            Assert.IsTrue(node.HasChildren());
            Assert.IsTrue(node.HasSingleChildren());
            Assert.IsFalse(node.HasBothChildren());
            
            node.RightChild = new DockingNode();
            Assert.IsTrue(node.HasChildren());
            Assert.IsTrue(node.HasSingleChildren());
            Assert.IsTrue(node.HasBothChildren());

            node.LeftChild = null;
            Assert.IsTrue(node.HasChildren());
            Assert.IsTrue(node.HasSingleChildren());
            Assert.IsFalse(node.HasBothChildren());
        }

        [TestMethod]
        public void CanAttachUI()
        {
            DockingNode node = new DockingNode();
            Assert.IsFalse(node.HasUIAttached());

            node.AttachedPanel = new Matcha_Editor.MVVM.View.DockingPanelView();
            Assert.IsTrue(node.HasUIAttached());
        }
    }
}
