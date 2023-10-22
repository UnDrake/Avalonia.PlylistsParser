using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Avalonia.PlaylistsParser.Models
{
    public class Playlist
    {
        public string? PlaylistName { get; set; }
        public string? Avatar { get; private set; }
        public string? Description { get; set; }
        public List<Song>? Songs { get; set; }

        public static Playlist Search(string url)
        {
            var playlist = new Playlist();
            using (IWebDriver driver = new ChromeDriver())
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    Thread.Sleep(6000);
                    
                    /* Використано Thread.Sleep(), через те, що за використання коду, вказаного нижче, 
                    браузер закривається одразу після знаходження div[@id='container'], не виконуючи дії 
                    по прокрутці сторінки, та знаходженні необхідної інформації, навіть якщо 
                    використовувати try-catch-finally замість using */
                    
                    // var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    // wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@id='container']")));
                    
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);

                    var playlistNode = doc.DocumentNode.SelectNodes("//music-detail-header")[1];
                    playlist.Avatar = playlistNode.GetAttributeValue("image-src", "");
                    playlist.PlaylistName = WebUtility.HtmlDecode(playlistNode.GetAttributeValue("headline", ""));
                    playlist.Description = WebUtility.HtmlDecode(string.Concat(playlistNode.GetAttributeValue("secondary-text", ""), "\n",
                        playlistNode.GetAttributeValue("tertiary-text", "")));
                    playlist.Songs = new List<Song>();

                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    var start = 0.0;
                    var end = (long)js.ExecuteScript("return document.body.scrollHeight;");
                    var height = (long)js.ExecuteScript("return screen.height;");

                    while (start < end)
                    {
                        var songsNode = doc.DocumentNode.SelectNodes("//div[@class='content']").Skip(18);
                        foreach (var s in songsNode)
                        {
                            var name = WebUtility.HtmlDecode(GetProperty(s, "col1"));
                            var artist = WebUtility.HtmlDecode(GetProperty(s, "col2"));
                            var album = WebUtility.HtmlDecode(GetProperty(s, "col3"));
                            var duration = WebUtility.HtmlDecode(GetProperty(s, "col4"));
                            var song = new Song(
                                name,
                                string.IsNullOrEmpty(artist) ? playlistNode.GetAttributeValue("primary-text", "") : artist,
                                string.IsNullOrEmpty(album) ? playlist.PlaylistName : album,
                                duration
                            );
                            if (!playlist.Songs.Contains(song))
                            {
                                playlist.Songs.Add(song);
                            }
                        }

                        js.ExecuteScript($"window.scrollTo({start}, {start + 2 * height});");
                        doc.LoadHtml(driver.PageSource);
                        start += 2 * height;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return playlist;
        }

        private static string GetProperty(HtmlNode node, string data)
        {
            var str = node.SelectSingleNode($"div[@class='{data}']");
            return str.InnerText != "" ? str.SelectSingleNode("music-link").GetAttributeValue("title", "") : "";
        }
    }
}
