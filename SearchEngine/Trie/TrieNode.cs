// Author : Puneet Sharma
// Copyright (C) 2016, Stevens Institute of Technology
// Created : 12/06/2016 21:24

using System.Collections.Generic;

namespace SearchEngine.Trie
{
    internal class TrieNode
    {
        private TrieNode _parent;

        public TrieNode()
        {
            Children = new HashSet<TrieNode>();
        }

        public bool IsRoot { get; set; }

        public TrieNode Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (_parent != null)
                {
                    Prefix = _parent.Prefix + _parent.Data;
                }
            }
        }

        public HashSet<TrieNode> Children { get; }

        public bool IsExternal { get; set; }

        public string Data { get; set; }

        public string Prefix { get; set; }

        public int WordIndex { get; set; }

        public void Insert(string word, int index)
        {
            TrieNode matchNode = null;
            foreach (var child in Children)
            {
                if (child.Data[0] == word[0])
                {
                    matchNode = child;
                    break;
                }
            }

            if (matchNode != null)
            {
                var key = matchNode.Data;
                var i = 1;
                bool keyEnd;
                bool wordEnd;
                while (true)
                {
                    keyEnd = i == key.Length;
                    wordEnd = i == word.Length;
                    if (keyEnd || wordEnd || (key[i] != word[i]))
                    {
                        break;
                    }
                    i++;
                }

                if (!wordEnd && !keyEnd)
                {
                    Children.Remove(matchNode);
                    var prefix = key.Substring(0, i);
                    var keySuffix = key.Substring(i);
                    var wordSuffix = word.Substring(i);
                    var intermediate = new TrieNode();
                    intermediate.Data = prefix;
                    intermediate.Parent = this;
                    Children.Add(intermediate);
                    matchNode.Data = keySuffix;
                    matchNode.Parent = intermediate;
                    intermediate.Children.Add(matchNode);
                    intermediate.Insert(wordSuffix, index);
                }
                else if (keyEnd)
                {
                    var wordSuffix = word.Substring(i);
                    matchNode.Insert(wordSuffix, index);
                }
                else
                {
                    Children.Remove(matchNode);
                    var intermediate = new TrieNode();
                    intermediate.Data = word;
                    intermediate.WordIndex = index;
                    intermediate.IsExternal = true;
                    intermediate.Parent = this;
                    intermediate.Children.Add(matchNode);
                    matchNode.Data = matchNode.Data.Substring(i);
                    matchNode.Parent = intermediate;
                }
            }
            else
            {
                var child = new TrieNode();
                child.Data = word;
                child.Parent = this;
                child.WordIndex = index;
                Children.Add(child);
                child.IsExternal = true;
            }
        }

        public int Search(string word)
        {
            TrieNode matchNode = null;
            foreach (var child in Children)
            {
                if (child.Data[0] == word[0])
                {
                    matchNode = child;
                    break;
                }
               
                //if (word.StartsWith(child.Data))
                //{
                //    matchNode = child;
                //    break;
                //}
            }

            if (matchNode == null)
            {
                return -1;
            }
            if (matchNode.Data == word && matchNode.IsExternal)
            {
                return matchNode.WordIndex;
            }
            var key = matchNode.Data;
            var i = 1;
            bool keyEnd;
            bool wordEnd;
            while (true)
            {
                keyEnd = i == key.Length;
                wordEnd = i == word.Length;
                if (keyEnd || wordEnd || (key[i] != word[i]))
                {
                    break;
                }
                i++;
            }
            if (!keyEnd && !wordEnd)
            {
                return matchNode.Search(word.Substring(i));
            }
            //if (keyEnd)
            //{
            //    return -1;
            //}
            if (!wordEnd)
            {
                return matchNode.Search(word.Substring(i));
            }
            return -1;
           // return matchNode.Search(word.Substring(matchNode.Data.Length));
        }

        public override string ToString()
        {
            return Prefix + " -> " + Data;
        }
    }
}