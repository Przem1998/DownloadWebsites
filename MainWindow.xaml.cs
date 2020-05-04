using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AsyncMethods
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      

        public IList<Website> WebSitePathsList = new List<Website>();
        WebClient wb = new WebClient();
        Website selectedWebsite;

        //check that clicked multipage downloading
        bool MultiPage = false;
        private int downloadedFiles = 0;

        FolderBrowserDialog browserDialog = new System.Windows.Forms.FolderBrowserDialog();
        private string FinalPathSaveWebsites = "";

        public MainWindow()
        {
            InitializeComponent();
        
            WebSitePathsList.Add(new Website("https://www.olx.pl/"));
            WebSitePathsList.Add(new Website("https://www.onet.pl/"));
            WebSitePathsList.Add(new Website("https://www.wikipedia.org/"));
            lbWebsites.ItemsSource = WebSitePathsList;
            wb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
            wb.DownloadFileCompleted += new AsyncCompletedEventHandler(CompletedDownload);
        }

        private void CompletedDownload(object sender, AsyncCompletedEventArgs e)
        {
            if (Completed.IsVisible == false) Completed.Visibility = Visibility;
            if (e.Cancelled) Completed.Content = "Błąd pobierania";
            else
            {
                Completed.Content = "Pobieranie zakończono sukcesem";
                DownloadFilesCount.Content = ++downloadedFiles;
            }
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadprogress.Value = e.ProgressPercentage;
            DownloadPercetns.Content = e.ProgressPercentage + "%";
        }


        private async void DownloadWebPage(string url)
        {

            int slashs = url.IndexOf("/") + 2;
            string nameHtmlFile = url.Substring(slashs, url.Length - slashs);
            nameHtmlFile = Regex.Replace(nameHtmlFile, "/", "_");
           
            bool exsitsFile = File.Exists(FinalPathSaveWebsites + "\\" + nameHtmlFile + ".html");
            if (exsitsFile)
            {
                int i = 1;
                while (exsitsFile!=false)
                {
                    if (nameHtmlFile.Contains(")"))
                    {
                        nameHtmlFile = nameHtmlFile.Substring(0, nameHtmlFile.Length - 3);
                        nameHtmlFile += "(" + (++i) + ")";
                    }
                    else nameHtmlFile += "(" + (++i) + ")";

                    exsitsFile = File.Exists(FinalPathSaveWebsites + "\\" + nameHtmlFile + ".html");
                   
                }
            }
           
                try
                {
                    await wb.DownloadFileTaskAsync(new Uri(url), FinalPathSaveWebsites+"\\"+nameHtmlFile + ".html");
                }
                catch (FormatException)
                {
                System.Windows.MessageBox.Show("Wprowadzono błędny adres strony internetowej");
                }
            if (MultiPage)
            {
                if (lbWebsites.SelectedIndex != WebSitePathsList.Count-1)
                {
                    lbWebsites.SelectedIndex += 1;
                    lbWebsites.Focus();
                }
                else MultiPage = false;
            }

        }
        private void DownloadWeb_Click(object sender, RoutedEventArgs e)
        {
            if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               
                FinalPathSaveWebsites = browserDialog.SelectedPath;
                selectedWebsite = (Website)lbWebsites.SelectedItem;
                if (selectedWebsite != null) DownloadWebPage(selectedWebsite.Url);
                else System.Windows.MessageBox.Show("Nie wybrano strony do pobrania");
            }
        
        }

        private void DownloadWebs_Click(object sender, RoutedEventArgs e)
        {

            if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
              
                FinalPathSaveWebsites = browserDialog.SelectedPath;
                lbWebsites.SelectedIndex = 0;
                lbWebsites.Focus();
                selectedWebsite = (Website)lbWebsites.SelectedItem;
                MultiPage = true;
                DownloadWebPage(selectedWebsite.Url);
            }
          
        }

        private void LbWebsites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiPage) {
                selectedWebsite = (Website)lbWebsites.SelectedItem;
                DownloadWebPage(selectedWebsite.Url);
            }
        }
    }
}
