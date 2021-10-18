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

            Assert.IsFalse(node.IsChild(null));
            Assert.IsFalse(node.IsChild(new DockingNode()));
            Assert.IsTrue(node.IsChild(leftChild));
            Assert.IsTrue(node.IsChild(rightChild));
        }

        [TestMethod]
        public void CanQueryLeftOrRightChild()
        {
            DockingNode node = new DockingNode();
            DockingNode leftChild = new DockingNode();
            DockingNode rightChild = new DockingNode();
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            Assert.IsTrue(node.IsLeftChild(leftChild));
            Assert.IsFalse(node.IsLeftChild(rightChild));
            Assert.IsFalse(node.IsLeftChild(new DockingNode()));
            Assert.IsFalse(node.IsLeftChild(null));
            Assert.IsFalse(node.IsLeftChild(node));

            Assert.IsTrue(node.IsRightChild(rightChild));
            Assert.IsFalse(node.IsRightChild(leftChild));
            Assert.IsFalse(node.IsRightChild(new DockingNode()));
            Assert.IsFalse(node.IsRightChild(null));
            Assert.IsFalse(node.IsRightChild(node));
        }

        [TestMethod]
        public void CanQuerySubzone()
        {
            double widthHeight = DockingNode.m_SubzoneRatio * 100;
            double padding = DockingNode.m_SubzoneRatio * widthHeight;

            DockingNode node = new DockingNode(new Rect(0, 0, widthHeight, widthHeight));
            DockingNode subzone;

            for (int x = 0; x < widthHeight; ++x)
            {
                for (int y = 0; y < widthHeight; ++y)
                {
                    subzone = node.GetSubzone(new Point(x, y));
                    double distanceToTop = y;
                    double distanceToLeft = x;
                    double distanceToBottom = widthHeight- y;
                    double distanceToRight = widthHeight- x;
                    double shortestDistance = Math.Min(distanceToTop, Math.Min(distanceToLeft, Math.Min(distanceToBottom, distanceToRight)));

                    if (shortestDistance > padding)
                    {
                        Assert.IsNull(subzone);
                        continue;
                    }

                    if (shortestDistance == distanceToLeft)
                    {
                        Assert.IsTrue(subzone.Rect.TopLeft == new Point(0,0));
                        Assert.IsTrue(subzone.Rect.BottomRight == new Point(padding, widthHeight));
                    }
                    else if (shortestDistance == distanceToRight)
                    {
                        Assert.IsTrue(subzone.Rect.TopLeft == new Point(widthHeight - padding, 0));
                        Assert.IsTrue(subzone.Rect.BottomRight == new Point(widthHeight, widthHeight));
                    }
                    else if (shortestDistance == distanceToTop)
                    {
                        Assert.IsTrue(subzone.Rect.TopLeft == new Point(0 ,0));
                        Assert.IsTrue(subzone.Rect.BottomRight == new Point(widthHeight, padding));
                    }
                    else if (shortestDistance == distanceToBottom)
                    {
                        Assert.IsTrue(subzone.Rect.TopLeft == new Point(0, widthHeight - padding));
                        Assert.IsTrue(subzone.Rect.BottomRight == new Point(widthHeight, widthHeight));
                    }
                }
            }
        }

        [TestMethod]
        public void CanBeResized()
        {
            DockingNode node = new DockingNode(new Rect(0, 0, 500, 500));
            DockingNode leftChild = new DockingNode { Rect = new Rect(0, 0, 100, 500), IsHorizontallyStacked = true };
            DockingNode rightChild = new DockingNode { Rect = new Rect(100, 0, 400, 500), IsHorizontallyStacked = true };
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            node.RecursiveResize(new Rect(0, 0, 100, 100));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 20, 100));
            Assert.AreEqual(rightChild.Rect, new Rect(20, 0, 80, 100));

            node.RecursiveResize(new Rect(0, 0, 100, 200));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 20, 200));
            Assert.AreEqual(rightChild.Rect, new Rect(20, 0, 80, 200));

            node = new DockingNode(new Rect(0, 0, 500, 500));
            leftChild = new DockingNode { Rect = new Rect(0, 0, 500, 100), IsHorizontallyStacked = false };
            rightChild = new DockingNode { Rect = new Rect(0, 100, 500, 400), IsHorizontallyStacked = false };
            node.LeftChild = leftChild;
            node.RightChild = rightChild;

            node.RecursiveResize(new Rect(0, 0, 100, 100));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 100, 20));
            Assert.AreEqual(rightChild.Rect, new Rect(0, 20, 100, 80));

            node.RecursiveResize(new Rect(0, 0, 100, 200));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 100, 40));
            Assert.AreEqual(rightChild.Rect, new Rect(0, 40, 100, 160));

            node.RecursiveResize(new Rect(0, 0, 200, 200));
            Assert.AreEqual(leftChild.Rect, new Rect(0, 0, 200, 40));
            Assert.AreEqual(rightChild.Rect, new Rect(0, 40, 200, 160));
        }
    }
}
