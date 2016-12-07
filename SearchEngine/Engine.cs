// Author : Puneet Sharma
// Copyright (C) 2016, Stevens Institute of Technology
// Created : 12/06/2016 16:21

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace SearchEngine
{
    public class Engine
    {
        private static readonly string[] Terminals =
        {
            " ", ".", "?", "!", ",", ";", ":", "-", "(", ")", "[", "]", "{", "}", "'",
            "—", "\"", "\n", "\r", "\t"
        };

        private readonly List<string> _ignoreTags;
        private List<HashSet<string>> _occurenceList;
        private HashSet<string> _stopWords;
        private Trie.Trie _trie;

        public Engine()
        {
            _ignoreTags = new List<string>();
            _ignoreTags.Add("script");
            _ignoreTags.Add("meta");
        }

        public void Process()
        {
            _occurenceList = InitializeList();
        }

        private List<HashSet<string>> InitializeList()
        {
            var wordsFile = ConfigurationManager.AppSettings["words"];
            var stopWordsFile = ConfigurationManager.AppSettings["stopWords"];
            var info = new FileInfo(wordsFile);
            var fileLength = info.Length + 2;
            _stopWords = new HashSet<string>(File.ReadAllLines(stopWordsFile).Select(s => s.Trim()));

            var words = File.ReadLines(wordsFile).Select(s => s.Trim());
            var list = new List<HashSet<string>>();
            _trie = new Trie.Trie();
            var i = 0;
            double processed = 0;
            Console.Write("\rPlease wait. Processing words list...{0:P2}. Words processed: {1}", 0, i);
            foreach (var word in words)
            {
                processed += word.Length + 2;
                var perc = processed/fileLength;
                if (!_stopWords.Contains(word))
                {
                    list.Add(new HashSet<string>());
                    _trie.Insert(word, i);
                    i++;
                }
                Console.Write("\rPlease wait. Processing words list...{0:P2}. Words processed: {1}", perc, i);
            }
            Console.WriteLine();

            return list;
        }

        public void Search(string text)
        {
            var index = _trie.Search(text);
            if (index == -1)
            {
                Console.WriteLine("No Results.");
                return;
            }
            Console.WriteLine("Found {0} results.", _occurenceList[index].Count);
            foreach (var url in _occurenceList[index])
            {
                Console.WriteLine(url);
            }
        }

        public void Index(Uri uri, int maxDepth)
        {
            var processed = new HashSet<Uri>();
            Console.WriteLine("Started Indexing...");
            Index(uri, 0, processed, maxDepth);
        }
        
        private void Index(Uri uri, int depth, HashSet<Uri> processed, int maxDepth)
        {
            Console.WriteLine(uri);
            var links = new Queue<Uri>();
            var page = new XmlDocument();
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };
            try
            {
                var request = WebRequest.CreateHttp(uri);
                request.Timeout = 1000;
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var reader = XmlReader.Create(stream, settings);
                        page.Load(reader);
                    }
                }
            }
            catch
            {
            }
            var processedWords = new HashSet<string>();
            foreach (XmlElement node in page.SelectNodes("//*[text()]"))
            {
                if (_ignoreTags.Contains(node.Name))
                {
                    continue;
                }
                if (node.Name == "a" && depth < maxDepth)
                {
                    var link = node.Attributes["href"].Value;
                    if (!link.StartsWith("#"))
                    {
                        if (link.StartsWith("/"))
                        {
                            var l = new Uri(uri, link);
                            if (!processed.Contains(l))
                            {
                                links.Enqueue(l);
                            }
                        }
                    }
                }
                var text = node.InnerText;
                foreach (var split in text.Split(Terminals, StringSplitOptions.RemoveEmptyEntries))
                {
                    var word = split.ToLower();
                    if (!_stopWords.Contains(word) && !processedWords.Contains(word))
                    {
                        var index = _trie.Search(word);
                        if (index != -1)
                        {
                            _occurenceList[index].Add(uri.ToString());
                        }
                        processedWords.Add(word);
                    }
                }
            }
            while (links.Count > 0)
            {
                Index(links.Dequeue(), depth + 1, processed, maxDepth);
            }
        }
    }
}