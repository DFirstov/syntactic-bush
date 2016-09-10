using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyntacticBush.Test
{
    [TestClass]
    public class CloneableNodeTest
    {
        [TestMethod]
        public void CloneTest()
        {
            var woman1 = new Node<string>("woman");
            var a1 = woman1.AddLeftChild("a");
            var in1 = woman1.AddRightChild("in");
            var red = in1.AddRightChild("red");

            var woman2 = Node<string>.Clone(woman1);

            a1.Value = "the";
            woman1.Value = "men";
            red.Value = "black";

            Assert.AreEqual("the men in black", woman1.ToString());
            Assert.AreEqual("a woman in red", woman2.ToString());

            try
            {
                Node<string>.Clone((Node<string>) null);
                Assert.Fail("The exception wasn’t thrown.");
            }
            catch (ArgumentNullException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("The thrown exception was incorrect.");
            }
        }

        [TestMethod]
        public void AddChildTest()
        {
            var woman1 = new Node<string>("woman");
            var a1 = woman1.AddLeftChild("a");
            var in1 = woman1.AddRightChild("in");
            in1.AddRightChild("red");

            var woman2 = new Node<string>("woman");
            var a2 = woman2.AddLeftChild(a1);
            var in2 = woman2.AddRightChild(in1);

            Assert.IsFalse(ReferenceEquals(a1, a2));
            Assert.IsFalse(ReferenceEquals(in1, in2));
        }

        [TestMethod]
        public void InsertChildTest()
        {
            var woman = new Node<string>("woman");
            woman.AddLeftChild("a");
            var @in = woman.AddRightChild("in");
            @in.AddRightChild("red");

            var black = new Node<string>("black");
            black.AddRightChild("and");

            var blackClone = @in.InsertChild(node => node.Value == "red", black, true);
            Assert.IsFalse(ReferenceEquals(blackClone, black));
            Assert.AreEqual(@in, blackClone.Parent);
            Assert.AreEqual(false, blackClone.IsLeft);

            black.Value = "white";
            Assert.AreEqual("a woman in black and red", woman.ToString());

            Assert.IsNull(woman.InsertChild(node => false, new Node<string>("something")));
        }
    }
}
