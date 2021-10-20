using Matcha_Editor.Core.Docking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System;

namespace Tests.Docking
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
            Assert.IsFalse(node.HasSingleChildren());
            Assert.IsTrue(node.HasBothChildren());

            node.LeftChild = null;
            Assert.IsTrue(node.HasChildren());
            Assert.IsTrue(node.HasSingleChildren());
            Assert.IsFalse(node.HasBothChildren());
        }

        [TestMethod]
        public void CanTestIfIsChild()
        {
            DockingNode node = new DockingNode();
            DockingNode leftChild = new DockingNode();
            DockingNode rightChild = new DockingNode();
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            Assert.IsFalse((new DockingNode()).IsChild());
            Assert.IsTrue(leftChild.IsChild());
            Assert.IsTrue(rightChild.IsChild());
            Assert.IsFalse(node.IsChild());
        }

        [TestMethod]
        public void CanQueryLeftOrRightChild()
        {
            DockingNode node = new DockingNode();
            DockingNode leftChild = new DockingNode();
            DockingNode rightChild = new DockingNode();
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            Assert.IsTrue(leftChild.IsLeftChild());
            Assert.IsFalse(rightChild.IsLeftChild());
            Assert.IsFalse((new DockingNode()).IsLeftChild());
            Assert.IsFalse(node.IsLeftChild());

            Assert.IsTrue(rightChild.IsRightChild());
            Assert.IsFalse(leftChild.IsRightChild());
            Assert.IsFalse((new DockingNode()).IsRightChild());
            Assert.IsFalse(node.IsRightChild());
        }

        [TestMethod]
        public void CanBeResized()
        {
            DockingNode node = new DockingNode(new Rect(0, 0, 5000, 5000));
            DockingNode leftChild = new DockingNode { Rect = new Rect(0, 0, 1000, 5000), IsHorizontallyStacked = true };
            DockingNode rightChild = new DockingNode { Rect = new Rect(100, 0, 4000, 5000), IsHorizontallyStacked = true };
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            node.RecursiveResize(new Rect(0, 0, 1000, 1000));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 200, 1000));
            Assert.AreEqual(rightChild.Rect, new Rect(200, 0, 800, 1000));

            node.RecursiveResize(new Rect(0, 0, 1000, 2000));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 200, 2000));
            Assert.AreEqual(rightChild.Rect, new Rect(200, 0, 800, 2000));

            node = new DockingNode(new Rect(0, 0, 5000, 5000));
            leftChild = new DockingNode { Rect = new Rect(0, 0, 5000, 1000), IsHorizontallyStacked = false };
            rightChild = new DockingNode { Rect = new Rect(0, 1000, 5000, 4000), IsHorizontallyStacked = false };
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            node.RecursiveResize(new Rect(0, 0, 1000, 1000));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 1000, 200));
            Assert.AreEqual(rightChild.Rect, new Rect(0, 200, 1000, 800));

            node.RecursiveResize(new Rect(0, 0, 1000, 2000));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 1000, 400));
            Assert.AreEqual(rightChild.Rect, new Rect(0, 400, 1000, 1600));

            node.RecursiveResize(new Rect(0, 0, 2000, 2000));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 2000, 400));
            Assert.AreEqual(rightChild.Rect, new Rect(0, 400, 2000, 1600));
        }

        [TestMethod]
        public void CanGetMinSize()
        {
            DockingNode a = new DockingNode(new Rect(0, 0, 800, 600));
            DockingNode b = new DockingNode(new Rect(0, 0, 400, 600));
            DockingNode c = new DockingNode(new Rect(0, 0, 400, 300));
            DockingNode d = new DockingNode(new Rect(0, 300, 400, 300));
            DockingNode e = new DockingNode(new Rect(0, 300, 200, 300));
            DockingNode f = new DockingNode(new Rect(200, 300, 300, 300 ));
            DockingNode g = new DockingNode(new Rect(400, 0, 400, 600));

            a.LeftChild = b;
            a.RightChild = g;
            b.LeftChild = c;
            b.RightChild = d;
            d.LeftChild = e;
            d.RightChild = f;

            b.IsHorizontallyStacked = true;
            g.IsHorizontallyStacked = b.IsHorizontallyStacked;

            c.IsHorizontallyStacked = false;
            d.IsHorizontallyStacked = c.IsHorizontallyStacked;

            e.IsHorizontallyStacked = true;
            f.IsHorizontallyStacked = e.IsHorizontallyStacked;

            Assert.AreEqual(a.GetMinimumSize(), new Size(600, 400));
        }

        [TestMethod]
        public void CanTestMinSize()
        {
            DockingNode a = new DockingNode(new Rect(0, 0, 800, 600));
            DockingNode b = new DockingNode(new Rect(0, 0, 400, 600));
            DockingNode c = new DockingNode(new Rect(0, 0, 400, 300));
            DockingNode d = new DockingNode(new Rect(0, 300, 400, 300));
            DockingNode e = new DockingNode(new Rect(0, 300, 200, 300));
            DockingNode f = new DockingNode(new Rect(200, 300, 300, 300 ));
            DockingNode g = new DockingNode(new Rect(400, 0, 400, 600));

            a.LeftChild = b;
            a.RightChild = g;
            b.LeftChild = c;
            b.RightChild = d;
            d.LeftChild = e;
            d.RightChild = f;

            b.IsHorizontallyStacked = true;
            g.IsHorizontallyStacked = b.IsHorizontallyStacked;

            c.IsHorizontallyStacked = false;
            d.IsHorizontallyStacked = c.IsHorizontallyStacked;

            e.IsHorizontallyStacked = true;
            f.IsHorizontallyStacked = e.IsHorizontallyStacked;

            Assert.IsTrue(a.CanBeOfSize(new Size(600, 400))); 
            Assert.IsFalse(a.CanBeOfSize(new Size(599, 400))); 
            Assert.IsFalse(a.CanBeOfSize(new Size(600, 399))); 
            Assert.IsFalse(a.CanBeOfSize(new Size(0, 0))); 
            Assert.IsTrue(a.CanBeOfSize(new Size(601, 400))); 
            Assert.IsTrue(a.CanBeOfSize(new Size(600, 401))); 
        }
    }
}
