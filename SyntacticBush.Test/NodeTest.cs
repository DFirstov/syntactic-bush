using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyntacticBush.Test
{
    [TestClass]
    public class NodeTest
    {
        [TestMethod]
        public void AddChildTest()
        {
            var woman = new Node<Word>(new Word("woman"));
            var a = woman.AddLeftChild(new Word("a"));
            var @in = woman.AddRightChild(new Word("in"));
            var red = @in.AddRightChild(new Word("red"));

            Assert.AreEqual("woman", woman.Value.Text);
            Assert.AreEqual(null, woman.Parent);
            Assert.AreEqual(null, woman.IsLeft);
            Assert.AreEqual(2, woman.Children.Count);
            Assert.IsTrue(woman.Children.Contains(a));
            Assert.IsTrue(woman.Children.Contains(@in));
            Assert.IsTrue(woman.LeftChildren.Contains(a)    && !woman.LeftChildren.Contains(@in));
            Assert.IsTrue(woman.RightChildren.Contains(@in) && !woman.RightChildren.Contains(a));

            Assert.AreEqual("a", a.Value.Text);
            Assert.AreEqual(woman, a.Parent);
            Assert.AreEqual(true, a.IsLeft);
            Assert.AreEqual(0, a.Children.Count);

            Assert.AreEqual("in", @in.Value.Text);
            Assert.AreEqual(woman, @in.Parent);
            Assert.AreEqual(false, @in.IsLeft);
            Assert.AreEqual(1, @in.Children.Count);
            Assert.IsTrue(@in.Children.Contains(red));
            Assert.IsTrue(@in.RightChildren.Contains(red) && !@in.LeftChildren.Contains(red));

            Assert.AreEqual("red", red.Value.Text);
            Assert.AreEqual(@in, red.Parent);
            Assert.AreEqual(false, red.IsLeft);
            Assert.AreEqual(0, red.Children.Count);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var woman = new Node<Word>(new Word("woman"));
            woman.AddLeftChild(new Word("a"));
            woman.AddRightChild(new Word("in")).AddRightChild(new Word("red"));

            Assert.AreEqual("a, woman, in, red", woman.ToString(", "));
            Assert.AreEqual("a woman in red", woman.ToString());
        }

        [TestMethod]
        public void InsertChildTest()
        {
            var women = new Node<Word>(new Word("women"));
            women.AddLeftChild(new Word("three"));
            var @in = women.AddRightChild(new Word("in"));
            @in.AddRightChild(new Word("red,"));
            @in.AddRightChild(new Word("green,"));
            @in.AddRightChild(new Word("and")).AddRightChild(new Word("blue"));

            Assert.AreEqual("three women in red, green, and blue", women.ToString());

            var and = women.InsertChild(node => node.Value.Text == "three", new Word("and"));
            and.AddRightChild(new Word("two"));
            Assert.AreEqual(women, and.Parent);
            Assert.AreEqual(true, and.IsLeft);

            @in.InsertChild(node => node.Value.Text == "green,", new Word("black,"), true);
            @in.InsertChild(node => node.Value.Text == "green,", new Word("white,"));

            Assert.AreEqual("three and two women in red, black, green, white, and blue", women.ToString());

            Assert.IsNull(women.InsertChild(node => false, new Word("something")));
        }

        [TestMethod]
        public void RemoveChildTest()
        {
            var woman = new Node<Word>(new Word("woman"));
            var a = woman.AddLeftChild(new Word("a"));
            var @in = woman.AddRightChild(new Word("in"));
            @in.AddRightChild(new Word("red"));

            Assert.AreEqual("a woman in red", woman.ToString());
            
            var man = new Node<Word>(new Word("man"));
            man.Remove(); // check for absence of exceptions when the parent is null

            a.Remove();
            Assert.AreEqual("woman in red", woman.ToString());

            @in.Remove();
            Assert.AreEqual("woman", woman.ToString());
        }

        [TestMethod]
        public void RemoveChildrenTest()
        {
            var woman = new Node<Word>(new Word("woman"));
            woman.AddLeftChild(new Word("a"));
            woman.AddRightChild(new Word("in")).AddRightChild(new Word("red"));

            Assert.AreEqual("a woman in red", woman.ToString());

            Assert.AreEqual(1, woman.RemoveChildren(child => child.Value.Text.Length > 2));
            Assert.AreEqual("a woman in", woman.ToString());

            Assert.AreEqual(2, woman.RemoveChildren(child => true));
            Assert.AreEqual("woman", woman.ToString());
        }

        [TestMethod]
        public void LinqTest()
        {
            var woman = new Node<Word>(new Word("woman"));
            woman.AddLeftChild(new Word("a"));
            woman.AddRightChild(new Word("in")).AddRightChild(new Word("red"));

            var @in = woman.First(node => node.Value.Text == "in");
            var left = @in.First();
            var right = @in.Last();
            left.Value.Text = $"({left.Value}";
            right.Value.Text = $"{right.Value})";

            Assert.AreEqual("a woman (in red)", woman.ToString());
        }

        [TestMethod]
        public void EqualsTest()
        {
            var woman1 = new Node<Word>(new Word("woman"));
            woman1.AddLeftChild(new Word("a"));
            woman1.AddRightChild(new Word("in")).AddRightChild(new Word("red"));

            Assert.IsTrue(woman1.Equals(woman1));

            var woman2 = new Node<Word>(new Word("woman"));
            woman2.AddLeftChild(new Word("a"));
            woman2.AddRightChild(new Word("in")).AddRightChild(new Word("red"));

            var woman3 = new Node<Word>(new Word("woman"));
            woman3.AddLeftChild(new Word("a"));
            woman3.AddRightChild(new Word("in")).AddRightChild(new Word("blue"));

            var woman4 = new Node<Word>(new Word("woman"));
            woman4.AddLeftChild(null);

            var woman5 = new Node<Word>(null);

            var woman6 = new Node<Word>(new Word("woman"));
            woman6.AddLeftChild(new Word("a"));
            var @in = woman6.AddRightChild(new Word("in"));
            @in.AddRightChild(new Word("red"));
            @in.AddRightChild(new Word("and")).AddRightChild(new Word("blue"));

            Assert.IsTrue(woman1.Equals(woman2));
            Assert.IsTrue(woman2.Equals(woman1));
            Assert.AreNotEqual(woman1, woman2);

            Assert.IsFalse(woman1.Equals(woman3));
            Assert.IsFalse(woman3.Equals(woman1));
            Assert.IsFalse(woman1.Equals(woman4));
            Assert.IsFalse(woman4.Equals(woman1));
            Assert.IsFalse(woman1.Equals(woman5));
            Assert.IsFalse(woman5.Equals(woman1));
            Assert.IsFalse(woman1.Equals(woman6));
            Assert.IsFalse(woman6.Equals(woman1));

            Assert.IsFalse(woman1.Equals(null));
        }

        [TestMethod]
        public void EqualsIgnoreOrder()
        {
            var woman1 = new Node<Word>(new Word("woman"));
            woman1.AddLeftChild(new Word("a"));
            var in1 = woman1.AddLeftChild(new Word("in"));
            in1.AddRightChild(new Word("red"));
            in1.AddRightChild(new Word("green"));
            in1.AddRightChild(new Word("and")).AddRightChild(new Word("blue"));

            Assert.IsTrue(woman1.EqualsIgnoreOrder(woman1));

            var woman2 = new Node<Word>(new Word("woman"));
            woman2.AddLeftChild(new Word("a"));
            var in2 = woman2.AddLeftChild(new Word("in"));
            in2.AddRightChild(new Word("green"));
            in2.AddRightChild(new Word("red"));
            in2.AddRightChild(new Word("and")).AddRightChild(new Word("blue"));

            var woman3 = new Node<Word>(new Word("woman"));
            woman3.AddLeftChild(new Word("a"));
            var in3 = woman3.AddLeftChild(new Word("in"));
            in3.AddRightChild(new Word("red"));
            in3.AddRightChild(new Word("blue"));
            in3.AddRightChild(new Word("and")).AddRightChild(new Word("green"));

            Assert.IsTrue(woman1.EqualsIgnoreOrder(woman2));
            Assert.IsFalse(woman1.EqualsIgnoreOrder(woman3)); // another structure of the sentence
        }
    }
}
