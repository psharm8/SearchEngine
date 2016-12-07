// Author : Puneet Sharma
// Copyright (C) 2016, Stevens Institute of Technology
// Created : 12/06/2016 21:24

namespace SearchEngine.Trie
{
    public class Trie
    {
        private readonly TrieNode _root;

        public Trie()
        {
            _root = new TrieNode {IsRoot = true};
        }

        public void Insert(string word, int index)
        {
            _root.Insert(word, index);
        }

        public int Search(string word)
        {
            var index = -1;
            index = _root.Search(word);
            return index;
        }
    }
}