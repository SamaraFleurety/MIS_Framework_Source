﻿// Copyright (c) 2013 Pēteris Ņikiforovs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections;
using System.Collections.Generic;

//ac自动机
namespace AK_DLL.AhoCorasick
{
    /// <summary>
    /// Trie that will find and return strings found in a text.
    /// </summary>
    public class Trie : Trie<string>
    {
        /// <summary>
        /// Adds a string.
        /// </summary>
        /// <param name="s">The string to add.</param>
        public void Add(string s)
        {
            Add(s, s);
        }

        /// <summary>
        /// Adds multiple strings.
        /// </summary>
        /// <param name="strings">The strings to add.</param>
        public void Add(IEnumerable<string> strings)
        {
            foreach (string s in strings)
            {
                Add(s);
            }
        }
    }

    /// <summary>
    /// Trie that will find strings in a text and return values of type <typeparamref name="T"/>
    /// for each string found.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class Trie<TValue> : Trie<char, TValue>
    {
    }

    /// <summary>
    /// Trie that will find strings or phrases and return values of type <typeparamref name="T"/>
    /// for each string or phrase found.
    /// </summary>
    /// <remarks>
    /// <typeparamref name="T"/> will typically be a char for finding strings
    /// or a string for finding phrases or whole words.
    /// </remarks>
    /// <typeparam name="T">The type of a letter in a word.</typeparam>
    /// <typeparam name="TValue">The type of the value that will be returned when the word is found.</typeparam>
    public class Trie<T, TValue>
    {
        /// <summary>
        /// Root of the trie. It has no value and no parent.
        /// </summary>
        private readonly Node<T, TValue> root = new Node<T, TValue>();

        /// <summary>
        /// Adds a word to the tree.
        /// </summary>
        /// <remarks>
        /// A word consists of letters. A node is built for each letter.
        /// If the letter type is char, then the word will be a string, since it consists of letters.
        /// But a letter could also be a string which means that a node will be added
        /// for each word and so the word is actually a phrase.
        /// </remarks>
        /// <param name="word">The word that will be searched.</param>
        /// <param name="value">The value that will be returned when the word is found.</param>
        public void Add(IEnumerable<T> word, TValue value)
        {
            // start at the root
            var node = root;

            // build a branch for the word, one letter at a time
            // if a letter node doesn't exist, add it
            foreach (T c in word)
            {
                var child = node[c];

                if (child == null)
                    child = node[c] = new Node<T, TValue>(c, node);

                node = child;
            }

            // mark the end of the branch
            // by adding a value that will be returned when this word is found in a text
            node.Values.Add(value);
        }


        /// <summary>
        /// Constructs fail or fall links.
        /// </summary>
        public void Build()
        {
            // construction is done using breadth-first-search
            var queue = new Queue<Node<T, TValue>>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                // visit children
                foreach (var child in node)
                    queue.Enqueue(child);

                // fail link of root is root
                if (node == root)
                {
                    root.Fail = root;
                    continue;
                }

                var fail = node.Parent.Fail;

                while (fail[node.Word] == null && fail != root)
                    fail = fail.Fail;

                node.Fail = fail[node.Word] ?? root;
                if (node.Fail == node)
                    node.Fail = root;
            }
        }

        /// <summary>
        /// Finds all added words in a text.
        /// </summary>
        /// <param name="text">The text to search in.</param>
        /// <returns>The values that were added for the found words.</returns>
        public IEnumerable<TValue> Find(IEnumerable<T> text)
        {
            var node = root;

            foreach (T c in text)
            {
                while (node[c] == null && node != root)
                    node = node.Fail;

                node = node[c] ?? root;

                for (var t = node; t != root; t = t.Fail)
                {
                    foreach (TValue value in t.Values)
                        yield return value;
                }
            }
        }

        /// <summary>
        /// Node in a trie.
        /// </summary>
        /// <typeparam name="TNode">The same as the parent type.</typeparam>
        /// <typeparam name="TNodeValue">The same as the parent value type.</typeparam>
        private class Node<TNode, TNodeValue> : IEnumerable<Node<TNode, TNodeValue>>
        {
            private readonly TNode word;
            private readonly Node<TNode, TNodeValue> parent;
            private readonly Dictionary<TNode, Node<TNode, TNodeValue>> children = new Dictionary<TNode, Node<TNode, TNodeValue>>();
            private readonly List<TNodeValue> values = new List<TNodeValue>();

            /// <summary>
            /// Constructor for the root node.
            /// </summary>
            public Node()
            {
            }

            /// <summary>
            /// Constructor for a node with a word
            /// </summary>
            /// <param name="word"></param>
            /// <param name="parent"></param>
            public Node(TNode word, Node<TNode, TNodeValue> parent)
            {
                this.word = word;
                this.parent = parent;
            }

            /// <summary>
            /// Word (or letter) for this node.
            /// </summary>
            public TNode Word
            {
                get { return word; }
            }

            /// <summary>
            /// Parent node.
            /// </summary>
            public Node<TNode, TNodeValue> Parent
            {
                get { return parent; }
            }

            /// <summary>
            /// Fail or fall node.
            /// </summary>
            public Node<TNode, TNodeValue> Fail
            {
                get;
                set;
            }

            /// <summary>
            /// Children for this node.
            /// </summary>
            /// <param name="c">Child word.</param>
            /// <returns>Child node.</returns>
            public Node<TNode, TNodeValue> this[TNode c]
            {
                get { return children.ContainsKey(c) ? children[c] : null; }
                set { children[c] = value; }
            }

            /// <summary>
            /// Values for words that end at this node.
            /// </summary>
            public List<TNodeValue> Values
            {
                get { return values; }
            }

            /// <inherit/>
            public IEnumerator<Node<TNode, TNodeValue>> GetEnumerator()
            {
                return children.Values.GetEnumerator();
            }

            /// <inherit/>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <inherit/>
            public override string ToString()
            {
                return Word.ToString();
            }
        }
    }
}
