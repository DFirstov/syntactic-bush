using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticBush
{
    /// <summary>
    /// Represents one node of a syntactic bush.
    /// </summary>
    /// <typeparam name="T">A type of a value that is stored in each node.</typeparam>
    public class Node<T> : IEnumerable<Node<T>>
    {
        /// <summary>
        /// The value stored in this node.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// A reference to the parent of this node.
        /// </summary>
        internal Node<T> Parent { get; }

        /// <summary>
        /// If true, the word represented with this node was located before the parent in an original sentence.
        /// If false, the word was after the parent.
        /// If null, this is a root node (has no parents).
        /// </summary>
        internal bool? IsLeft { get; private set; }

        /// <summary>
        /// A list of child nodes.
        /// </summary>
        private readonly List<Node<T>> _children;

        /// <summary>
        /// A list of all direct children of this node. Cannot be changed directly.
        /// </summary>
        public IReadOnlyList<Node<T>> Children => _children;

        /// <summary>
        /// An enumerable of all direct children which were located before this node in an original sentence.
        /// </summary>
        public IEnumerable<Node<T>> LeftChildren => _children.Where(child => child.IsLeft == true);

        /// <summary>
        /// An enumerable of all direct children which were located after this node in an original sentence.
        /// </summary>
        public IEnumerable<Node<T>> RightChildren => _children.Where(child => child.IsLeft == false);

        /// <summary>
        /// Creates a new node. This constructor is not to be used by uesers of the library.
        /// </summary>
        /// <param name="value">A value that should be contained in the node.</param>
        /// <param name="parent">A parent of the node.</param>
        /// <param name="isLeft">A flag showing if the node was located before the parent 
        /// in an original sentence.</param>
        internal Node(T value, Node<T> parent = null, bool? isLeft = null)
        {
            Value = value;
            Parent = parent;
            _children = new List<Node<T>>();
            IsLeft = isLeft;
        }

        /// <summary>
        /// Creates a clone of a node and all its descendants.
        /// </summary>
        /// <typeparam name="TCloneable">A type of the stored value. It should be ICloneable.</typeparam>
        /// <param name="node">A node for cloning.</param>
        /// <returns>The clone. It will be like a root — without a parent 
        /// and with the left/right flag set to null.</returns>
        public static Node<T> Clone<TCloneable>(Node<TCloneable> node) where TCloneable : T, ICloneable
            => Clone(node, null);

        /// <summary>
        /// Creates a clone of the node and all its ancestors.
        /// </summary>
        /// <typeparam name="TCloneable">A type of the stored value. It should be ICloneable.</typeparam>
        /// <param name="node">A node for cloning.</param>
        /// <param name="parent">A reference to parent for the clone.</param>
        /// <param name="isLeft">If true, the clone will be left relatively to the parent, if false — right, 
        /// if null — the parent also should be null.</param>
        /// <returns>A reference to the created clone.</returns>
        private static Node<T> Clone<TCloneable>(Node<TCloneable> node, Node<T> parent, bool? isLeft = null)
            where TCloneable : T, ICloneable
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var valueClone = node.Value?.Clone();
            var clone = new Node<T>((T)valueClone, parent, isLeft);

            foreach (var child in node.LeftChildren)  clone.AddLeftChild(child);
            foreach (var child in node.RightChildren) clone.AddRightChild(child);

            return clone;
        }

        /// <summary>
        /// Adds a new left child with a specified value to this node.
        /// </summary>
        /// <param name="value">The value of the new child.</param>
        /// <returns>A reference to the created child.</returns>
        public Node<T> AddLeftChild(T value) => AddChild(new Node<T>(value, this, true));

        /// <summary>
        /// Adds a new right child with a specified value to this node.
        /// </summary>
        /// <param name="value">The value of the new child.</param>
        /// <returns>A reference to the created child.</returns>
        public Node<T> AddRightChild(T value) => AddChild(new Node<T>(value, this, false));

        /// <summary>
        /// Clones an existing node and adds the clone as a left child.
        /// </summary>
        /// <typeparam name="TCloneable">A type of the stored value. It should be ICloneable.</typeparam>
        /// <param name="child">The node that will be cloned, and clone will be added as a child.</param>
        /// <returns>Reference to the added clone.</returns>
        public Node<T> AddLeftChild<TCloneable>(Node<TCloneable> child) where TCloneable : T, ICloneable
            => AddChild(Clone(child, this, true));

        /// <summary>
        /// Clones an existing node and adds the clone as a right child.
        /// </summary>
        /// <typeparam name="TCloneable">A type of the stored value. It should be ICloneable.</typeparam>
        /// <param name="child">The node that will be cloned, and clone will be added as a child.</param>
        /// <returns>Reference to the added clone.</returns>
        public Node<T> AddRightChild<TCloneable>(Node<TCloneable> child) where TCloneable : T, ICloneable
            => AddChild(Clone(child, this, false));

        /// <summary>
        /// Adds a child to the child list.
        /// </summary>
        /// <param name="node">The node that will be added as a child.</param>
        /// <returns>A reference to the added node.</returns>
        private Node<T> AddChild(Node<T> node)
        {
            _children.Add(node);
            return node;
        }

        /// <summary>
        /// Removes this node from the bush.
        /// </summary>
        public void Remove() => Parent?._children.Remove(this);

        /// <summary>
        /// Removes children matching the predicate.
        /// </summary>
        /// <param name="predicate">If the predicate for the node is true, the node will be removed.</param>
        /// <returns>A number of removed children.</returns>
        public int RemoveChildren(Predicate<Node<T>> predicate)
            => _children.RemoveAll(predicate) +
               _children.Sum(child => child.RemoveChildren(predicate));

        /// <summary>
        /// Creates a new child node and inserts it after or before the node selected by the predicate.
        /// </summary>
        /// <param name="neighbor">A predicate for selecting left or right neighbor of the new child.</param>
        /// <param name="value">A value of the new child.</param>
        /// <param name="before">True — insert the node before the neighbor, false — after.</param>
        /// <returns>A reference to the inserted node or null, if the neighbor wasn’t found.</returns>
        public Node<T> InsertChild(Predicate<Node<T>> neighbor, T value, bool before = false)
            => InsertChild(neighbor, new Node<T>(value, this), before);

        /// <summary>
        /// Clones the node and inserts the clone as a new child of this node.
        /// </summary>
        /// <typeparam name="TCloneable">A type of the stored value in the node. Should be ICloneable.</typeparam>
        /// <param name="neighbor">A predicate for selecting left or right neighbor of the new child.</param>
        /// <param name="child">The node, which clone should be inserted as a child.</param>
        /// <param name="before">True — insert the node before the neighbor, false — after.</param>
        /// <returns>A reference on the inserted clone or null, if the neighbor wasn’t found.</returns>
        public Node<T> InsertChild<TCloneable>(Predicate<Node<T>> neighbor, Node<TCloneable> child, bool before = false)
            where TCloneable : T, ICloneable
            => InsertChild(neighbor, Clone(child, this), before);

        /// <summary>
        /// Inserts a child.
        /// </summary>
        /// <param name="neighbor">A predicate for selecting left or right neighbor of the new child.</param>
        /// <param name="child">A node for inserting.</param>
        /// <param name="before">True — insert the node before the neighbor, false — after.</param>
        /// <returns>A reference to the inserted child or null, if the neighbor wasn’t found.</returns>
        private Node<T> InsertChild(Predicate<Node<T>> neighbor, Node<T> child, bool before)
        {
            var neighborIndex = _children.FindIndex(neighbor);
            if (neighborIndex == -1) return null;

            child.IsLeft = _children[neighborIndex].IsLeft;

            if (before) _children.Insert(neighborIndex,     child);
            else        _children.Insert(neighborIndex + 1, child);

            return child;
        }

        /// <summary>
        /// Builds a string representation of the subtree with a root in this node.
        /// The values of each node will be separated by space.
        /// </summary>
        /// <returns>A string representation of the subtree.</returns>
        public override string ToString() => ToString(" ");

        /// <summary>
        /// Builds a string representation of the subtree with a root in this node.
        /// The values of each node will be separated by the given separator.
        /// </summary>
        /// <param name="separator">A string that will be inserted between values of the nodes.</param>
        /// <returns>A string representation of the subtree.</returns>
        public string ToString(string separator)
        {
            var strings = this
                .Where(node => node.Value != null)
                .Select(node => node.Value.ToString());
            return string.Join(separator, strings);
        }
        
        /// <summary>
        /// Compares two nodes.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True, if nodes are equal.</returns>
        public bool Equals(Node<T> other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (!EqualityComparer<T>.Default.Equals(Value, other.Value)) return false;

            using (var enumerator1 = Children.GetEnumerator())
            using (var enumerator2 = other.Children.GetEnumerator())
            {
                bool firstHasChildren;
                bool secondHasChildren;
                while ((firstHasChildren = enumerator1.MoveNext()) 
                    & (secondHasChildren = enumerator2.MoveNext()))
                    if (!enumerator1.Current.Equals(enumerator2.Current)) return false;

                return !firstHasChildren && !secondHasChildren;
            }
        }

        /// <summary>
        /// Compares two nodes ignoring order of their child lists.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True, if nodes are equal.</returns>
        public bool EqualsIgnoreOrder(Node<T> other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (!EqualityComparer<T>.Default.Equals(Value, other.Value)) return false;

            return Children
                .Select(child => other.Children.Any(child.EqualsIgnoreOrder))
                .All(childrenAreEqual => childrenAreEqual);
        }

        /// <summary>
        /// Creates enumerable from the subtree with a root in this node. The nodes in the enumeration 
        /// go in the order from the most left to the most right node (like in an original sentence).
        /// </summary>
        /// <returns>Enumeration of the node.</returns>
        internal IEnumerable<Node<T>> Enumerate()
        {
            var left = Enumerate(LeftChildren);
            foreach (var l in left) yield return l;

            yield return this;

            var right = Enumerate(RightChildren);
            foreach (var r in right) yield return r;
        }

        /// <summary>
        /// Enumerates the given list of nodes.
        /// </summary>
        /// <param name="nodes">List of nodes.</param>
        /// <returns>Enumeration of all the nodes.</returns>
        private static IEnumerable<Node<T>> Enumerate(IEnumerable<Node<T>> nodes)
            => nodes.SelectMany(node => node.Enumerate());

        /// <summary>
        /// Returns the enumerator of the subtree with a root in this node.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Node<T>> GetEnumerator() => Enumerate().GetEnumerator();

        /// <summary>
        /// Returns the enumerator of the subtree with a root in this node.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
