using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace ConsoleTestApp
{
    public class PostItem
    {
        public string Title { get; set; }
        public string Category
        {
            get { return string.Join(" -> ", Categories); }
        }

        public ICollection<string> Categories { get; set; }
        public string Creator { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }

        public PostItem()
        {
            Categories = new HashSet<string>();
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            string rssURL = @"https://www.discoverveloguide.com/blog?format=rss";

            WebClient wclient = new WebClient();
            string rssData = string.Empty;

            List<PostItem> postItems = new List<PostItem>();

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.57 Safari/537.17";
                    var stream = client.OpenRead(rssURL);
                    StreamReader reader = new StreamReader(stream);
                    string feed = reader.ReadToEnd();

                    XmlDocument rssXmlDoc = new XmlDocument();
                    XmlNamespaceManager namespaces = new XmlNamespaceManager(rssXmlDoc.NameTable);
                    namespaces.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");
                    namespaces.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

                    rssXmlDoc.LoadXml(feed);

                    XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

                    StringBuilder rssContent = new StringBuilder();

                    foreach (XmlNode rssNode in rssNodes)
                    {
                        var postItem = new PostItem();

                        XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                        var title = rssSubNode?.InnerText;

                        rssSubNode = rssNode.SelectSingleNode("link");
                        var link = rssSubNode?.InnerText;

                        rssSubNode = rssNode.SelectSingleNode("description");
                        var description = rssSubNode?.InnerText;

                        rssSubNode = rssNode.SelectSingleNode("pubDate");
                        var publishDate = rssSubNode?.InnerText;

                        rssSubNode = rssNode.SelectSingleNode("//dc:creator", namespaces);
                        var creator = rssSubNode?.InnerText;

                        rssSubNode = rssNode.SelectSingleNode("//content:encoded", namespaces);
                        var content = rssSubNode?.InnerText;

                        //Handling categories list
                        var categories = rssNode.SelectNodes("category");
                        foreach (XmlNode cat in categories)
                        {
                            postItem.Categories.Add(cat?.InnerText);
                        }

                        postItem.Title = title;
                        postItem.Creator = creator;
                        postItem.Link = link;
                        postItem.Description = description;
                        postItem.PublishDate = Convert.ToDateTime(publishDate);
                        postItem.Content = content;

                        postItems.Add(postItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}