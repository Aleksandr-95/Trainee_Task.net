using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Trainee_Task
{
	class Program
	{
		static void Main(string[] args)
		{
			for (; ; )
			{
				try
				{
					//List for crawling website links
					List<string> crawlingWebSite = new List<string>();
					//List for sitemap.xml links
					List<string> siteMapXML = new List<string>();
					Console.WriteLine("Input URL: (https://example.com)");
					string inputURL = Console.ReadLine();
					//creating a request to the website
					HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(inputURL);
					//getting a response
					HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
					//getting a html-code
					StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8);
					//html-reading
					string htmlCode = streamReader.ReadToEnd();
					streamReader.Close();
					webResponse.Close();
					//pattern to links
					string pattern = @"<a\s+(?:[^>]*?\s+)?href=([""'])(https?://\S+?(\.html)?)\1";
					Regex regex = new Regex(pattern);
					Console.WriteLine();
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine("List with urls without sitemap.xml:");
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine();
					int i = 1;
					//getting all links that fit the pattern
					foreach (Match match in regex.Matches(htmlCode))
					{
						//timing of links
						Stopwatch stopwatch = new Stopwatch();
						//start
						stopwatch.Start();
						string link = match.Groups[2].Value;
						//adding link to the list
						crawlingWebSite.Add(link);
						//stop
						stopwatch.Stop();
						Console.WriteLine($"{i}) {link}  \t{stopwatch.Elapsed.Milliseconds}ms");
						i++;
					}
					//creating a resource identifier
					Uri uri = new Uri(inputURL);
					//getting https://
					string protocol = uri.Scheme;
					//getting domain name
					string domain = uri.Host;
					string finalURL = $"{protocol}://{domain}";
					//creating link to sitemap.xml
					XmlReader xmlReader = XmlReader.Create(finalURL + "/sitemap.xml");
					//adding links from loc-node to the list
					while (xmlReader.Read())
						if (xmlReader.Name == "loc")
							siteMapXML.Add(xmlReader.ReadInnerXml());
					xmlReader.Close();
					//founded in sitemap, but not founded after crawling
					IEnumerable<string> sitemapContains = siteMapXML.Except(crawlingWebSite);
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site: " + sitemapContains.Count());
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine();
					i = 1;
					foreach (string link in sitemapContains)
					{
						Console.WriteLine(i + ") " + link);
						i++;
					}
					//founded after crawling, but not founded in sitemap
					IEnumerable<string> crawlingContains = crawlingWebSite.Except(siteMapXML);
					Console.WriteLine();
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml: " + crawlingContains.Count());
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine();
					i = 1;
					foreach (string link in crawlingContains)
					{
						Console.WriteLine(i + ") " + link);
						i++;
					}
					Console.WriteLine();
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine("Urls found after crawling a website: " + crawlingWebSite.Count());
					Console.WriteLine("Urls found in sitemap: " + siteMapXML.Count());
					Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					Console.WriteLine();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"{ex.Message}");
					Console.WriteLine("Try again\n");
				}
			}
		}
	}
}
