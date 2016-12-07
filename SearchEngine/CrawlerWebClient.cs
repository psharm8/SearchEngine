// Author : Puneet Sharma
// Copyright (C) 2016, Stevens Institute of Technology
// Created : 12/07/2016 03:29

using System;
using System.Net;

namespace SearchEngine
{
    public class CrawlerWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var req= base.GetWebRequest(address);
            req.Timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
            return req;
        }
    }
}