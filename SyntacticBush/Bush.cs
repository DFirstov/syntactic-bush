using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticBush
{
    /// <summary>
    /// Represents one syntactic bush.
    /// </summary>
    /// <typeparam name="T">A type of stored values.</typeparam>
    public class Bush<T> : IEnumerable<Node<T>>
    {
        /// <summary>
        /// A node with default value, the parent of all roots of the bush.
        /// </summary>
        internal readonly Node<T> FakeRoot;

        /// <summary>
        /// Returns a read-only list of root nodes of the bush.
        /// </summary>
        public IReadOnlyList<Node<T>> Roots => FakeRoot.Children;

        /// <summary>
        /// Creates a new empty bush.
        /// </summary>
        public Bush()
        {
            FakeRoot = new Node<T>(default(T));
        }

        /// <summary>
        /// Creates a new bush with the specified fake root.
        /// </summary>
        /// <param name="fakeRoot">Fake root of a new bush.</param>
        private Bush(Node<T> fakeRoot)
        {
            FakeRoot = fakeRoot;
        }

        /// <summary>
        /// Clones the whole bush.
        /// </summary>
        /// <typeparam name="TCloneable">A type of stored values. Should be ICloneable.</typeparam>
        /// <param name="bush">The bush for cloning.</param>
        /// <returns>A clone of the bush.</returns>
        public static Bush<T> Clone<TCloneable>(Bush<TCloneable> bush) where TCloneable : T, ICloneable
            => new Bush<T>(Node<T>.Clone(bush.FakeRoot));

        /// <summary>
        /// Creates a new root node and adds it to the end of the root list.
        /// </summary>
        /// <param name="value">A value of the new root.</param>
        /// <returns>A reference to the created root.</returns>
        public Node<T> AddRoot(T value) => FakeRoot.AddLeftChild(value);

        /// <summary>
        /// Clones the given node and adds it to the end of the root list.
        /// </summary>
        /// <typeparam name="TCloneable">A type of stored values. Should be ICloneable.</typeparam>
        /// <param name="root">The node that should be cloned and added as root.</param>
        /// <returns>A reference to the clone.</returns>
        public Node<T> AddRoot<TCloneable>(Node<TCloneable> root) where TCloneable : T, ICloneable
            => FakeRoot.AddLeftChild(root);

        /// <summary>
        /// Removes all nodes matching the predicate.
        /// </summary>
        /// <param name="predicate">A predicate for the node removal.</param>
        /// <returns>Amount of deleted nodes.</returns>
        public int RemoveAll(Predicate<Node<T>> predicate) => FakeRoot.RemoveChildren(predicate);

        /// <summary>
        /// Creates a new root and inserts it before or after the node matching the predicate.
        /// </summary>
        /// <param name="neighbor">A predicate for the neighbor search.</param>
        /// <param name="value">A value of the new root.</param>
        /// <param name="before">A flag showing where the root should be placed: before or after the neighbor.</param>
        /// <returns>A reference to the created root.</returns>
        public Node<T> InsertRoot(Predicate<Node<T>> neighbor, T value, bool before = false)
            => FakeRoot.InsertChild(neighbor, value, before);

        /// <summary>
        /// Clones the node and inserts the clone to the root list.
        /// </summary>
        /// <typeparam name="TCloneable">A type of the stored value. Should be ICloneable.</typeparam>
        /// <param name="neighbor">A predicate for the neighbor search.</param>
        /// <param name="root">The node that should be cloned and inserted to the root list.</param>
        /// <param name="before">A flag showing where the root should be placed: before of after the neigbor.</param>
        /// <returns>A reference to the inserted root.</returns>
        public Node<T> InsertRoot<TCloneable>(Predicate<Node<T>> neighbor, Node<TCloneable> root, bool before = false)
            where TCloneable : T, ICloneable
            => FakeRoot.InsertChild(neighbor, root, before);

        /// <summary>
        /// Returns a string representation of the bush.
        /// </summary>
        /// <returns>A string representation, where nodes are divided by space.</returns>
        public override string ToString() => ToString(" ");

        /// <summary>
        /// Returns a string representation of the bush, where nodes are divided by the given separator.
        /// </summary>
        /// <param name="separator">The string that should be placed between nodes in a string representation.</param>
        /// <returns>A string representation.</returns>
        public string ToString(string separator) => FakeRoot.ToString(separator);

        /// <summary>
        /// Compares two bushes.
        /// </summary>
        /// <param name="other">The second bush.</param>
        /// <returns>True, if the bushes are equal.</returns>
        public bool Equals(Bush<T> other)
            => FakeRoot.Equals(other?.FakeRoot);

        /// <summary>
        /// Compares two bushes ignoring order of their children.
        /// </summary>
        /// <param name="other">The second bush.</param>
        /// <returns>True, if the bushes are equal.</returns>
        public bool EqualsIgnoreOrder(Bush<T> other)
            => FakeRoot.EqualsIgnoreOrder(other?.FakeRoot);

        /// <summary>
        /// Returns the enumerator of the bush.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Node<T>> GetEnumerator() => Roots.SelectMany(root => root.Enumerate()).GetEnumerator();

        /// <summary>
        /// Returns the enumerator of the bush.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
