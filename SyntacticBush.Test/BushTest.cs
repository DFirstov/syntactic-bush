using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyntacticBush.Test
{
    [TestClass]
    public class BushTest
    {
        [TestMethod]
        public void AddRootTest()
        {
            var bush = new Bush<Word>();
            var man = bush.AddRoot(new Word("man"));
            var woman = bush.AddRoot(new Word("woman"));
            woman.AddLeftChild(new Word("and"));

            Assert.IsTrue(bush.Roots.Contains(man));
            Assert.IsTrue(bush.Roots.Contains(woman));

            Assert.AreEqual(bush.FakeRoot, man.Parent);
            Assert.AreEqual(bush.FakeRoot, woman.Parent);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var bush = new Bush<Word>();
            bush.AddRoot(new Word("man"));
            bush.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            Assert.AreEqual("man, and, woman", bush.ToString(", "));
            Assert.AreEqual("man and woman", bush.ToString());
        }

        [TestMethod]
        public void RemoveAllTest()
        {
            var bush = new Bush<Word>();
            bush.AddRoot(new Word("man"));
            bush.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            Assert.AreEqual(0, bush.RemoveAll(node => node.Value.Text == "child"));

            Assert.AreEqual(1, bush.RemoveAll(node => node.Value.Text == "and"));
            Assert.AreEqual("man, woman", bush.ToString(", "));

            Assert.AreEqual(2, bush.RemoveAll(node => node.Value.Text.Contains("man")));
            Assert.AreEqual(string.Empty, bush.ToString());
        }

        [TestMethod]
        public void InsertRootTest()
        {
            var bush = new Bush<Word>();
            bush.AddRoot(new Word("man"));
            bush.AddRoot(new Word("child")).AddLeftChild(new Word("and"));

            Assert.AreEqual("man and child", bush.ToString());

            var inserted = bush.InsertRoot(node => node.Value.Text == "man", new Word("woman,"));
            Assert.AreEqual(bush.FakeRoot, inserted.Parent);

            bush.First(node => node.Value.Text == "man").Value.Text = "man,";
            Assert.AreEqual("man, woman, and child", bush.ToString());
        }

        [TestMethod]
        public void EqualsTest()
        {
            var bush1 = new Bush<Word>();
            bush1.AddRoot(new Word("man"));
            bush1.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            var bush2 = new Bush<Word>();
            bush2.AddRoot(new Word("man"));
            bush2.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            var bush3 = new Bush<Word>();
            bush3.AddRoot(new Word("woman"));
            bush3.AddRoot(new Word("man")).AddLeftChild(new Word("and"));

            Assert.IsTrue(bush1.Equals(bush2));
            Assert.IsFalse(bush1.Equals(bush3));
            Assert.IsFalse(bush1.Equals(null));
        }

        [TestMethod]
        public void EqualsIgnoreOrder()
        {
            var bush1 = new Bush<Word>();
            bush1.AddRoot(new Word("man,"));
            bush1.AddRoot(new Word("child,"));
            bush1.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            var bush2 = new Bush<Word>();
            bush2.AddRoot(new Word("man,"));
            bush2.AddRoot(new Word("child,"));
            bush2.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            var bush3 = new Bush<Word>();
            bush3.AddRoot(new Word("child,"));
            bush3.AddRoot(new Word("man,"));
            bush3.AddRoot(new Word("woman")).AddLeftChild(new Word("and"));

            var bush4 = new Bush<Word>();
            bush4.AddRoot(new Word("man"));
            bush4.AddRoot(new Word("child,"));
            bush4.AddRoot(new Word("woman")).AddLeftChild(new Word("&"));

            Assert.IsTrue(bush1.EqualsIgnoreOrder(bush2));
            Assert.IsTrue(bush1.EqualsIgnoreOrder(bush3));
            Assert.IsFalse(bush1.EqualsIgnoreOrder(bush4));
            Assert.IsFalse(bush1.EqualsIgnoreOrder(null));
        }

        [TestMethod]
        public void LinqTest()
        {
            var bush = new Bush<string>();

            var sang = bush.AddRoot("sang");
            var mother = sang.AddLeftChild("mother");
            mother.AddLeftChild("my");
            var song = sang.AddRightChild("song,");
            song.AddLeftChild("a");

            var played = bush.AddRoot("played");
            played.AddLeftChild("and");
            var father = played.AddLeftChild("father");
            father.AddLeftChild("my");
            var guitar = played.AddRightChild("guitar");
            guitar.AddLeftChild("a");

            var wordsStartingWithA = bush
                .Where(node => node.Value.StartsWith("a"))
                .Select(node => node.Value);
            CollectionAssert.AreEqual(new[] { "a", "and", "a" }, wordsStartingWithA.ToList());

            var myNodes = bush.Where(node => node.Value.Equals("my"));
            foreach (var myNode in myNodes) myNode.Value = "her";
            Assert.AreEqual("her mother sang a song, and her father played a guitar", bush.ToString(" "));

            var playedNode = bush.First(node => node.Value == "played");
            playedNode.First().Value = "(" + playedNode.First().Value;
            playedNode.Last().Value = playedNode.Last().Value + ")";
            Assert.AreEqual("her mother sang a song, (and her father played a guitar)", bush.ToString(" "));
        }
    }
}
