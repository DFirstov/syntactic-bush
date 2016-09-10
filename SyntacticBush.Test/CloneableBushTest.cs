using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyntacticBush.Test
{
    [TestClass]
    public class CloneableBushTest
    {
        [TestMethod]
        public void CloneTest()
        {
            var bush = new Bush<string>();
            bush.AddRoot("man");
            bush.AddRoot("woman").AddLeftChild("and");

            var clone = Bush<string>.Clone(bush);
            Assert.IsFalse(ReferenceEquals(bush, clone));

            bush.First(node => node.Value == "man").Value = "guy";
            bush.First(node => node.Value == "woman").Value = "girl";

            Assert.AreEqual("guy and girl", bush.ToString());
            Assert.AreEqual("man and woman", clone.ToString());
        }

        [TestMethod]
        public void AddRootTest()
        {
            var bush = new Bush<string>();
            bush.AddRoot("man");

            var node = new Node<string>("woman");
            node.AddLeftChild("and");

            var added = bush.AddRoot(node);
            Assert.IsFalse(ReferenceEquals(node, added));

            node.Value = "girl";
            Assert.AreEqual("man and woman", bush.ToString());
        }

        [TestMethod]
        public void InsertRootTest()
        {
            var bush = new Bush<string>();
            bush.AddRoot("man");
            bush.AddRoot("child").AddLeftChild("and");

            Assert.AreEqual("man and child", bush.ToString());

            var woman = new Node<string>("woman,");
            var inserted = bush.InsertRoot(node => node.Value == "man", woman);
            Assert.AreEqual(bush.FakeRoot, inserted.Parent);
            Assert.IsFalse(ReferenceEquals(woman, inserted));

            bush.First(node => node.Value == "man").Value = "man,";
            Assert.AreEqual("man, woman, and child", bush.ToString());
        }
    }
}
