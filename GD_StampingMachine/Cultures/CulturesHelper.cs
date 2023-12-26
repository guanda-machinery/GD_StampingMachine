using DevExpress.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
namespace GD_StampingMachine
{
    public class CulturesHelper
    {
        private static bool _isFoundInstalledCultures = false;

        private static string _resourcePrefix = "StringResources";

        private static string _culturesFolder = "Cultures";

        //private static List<CultureInfo> _supportedCultures;
        //private static List<CultureInfo> SupportedCultures =>_supportedCultures ??= GetSupportedCultures();


        public static List<CultureInfo> GetSupportedCultures()
        {
            var CultList = new List<CultureInfo>();
            // if (!_isFoundInstalledCultures)
            //{
            CultureInfo cultureInfo = new CultureInfo("");
            List<string> files = Directory.GetFiles(string.Format("{0}\\{1}", System.Windows.Forms.Application.StartupPath, _culturesFolder))
                .Where(s => s.Contains(_resourcePrefix) && Path.GetExtension(s).Equals(".xaml", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (string file in files)
            {
                try
                {
                    string cultureName = file.Substring(file.IndexOf(".") + 1).Replace(".xaml", "");
                    cultureInfo = CultureInfo.GetCultureInfo(cultureName);
                    if (cultureInfo != null)
                    {
                        CultList.Add(cultureInfo);
                    }
                }
                catch (ArgumentException)
                {
                }

            }

            /*if (CultList.Count > 0 && Properties.Settings.Default.DefaultCulture == null)
            {
                ChangeCulture(Properties.Settings.Default.DefaultCulture);
            }*/

            //_isFoundInstalledCultures = true;
            //}
            return CultList;

        }



        public static void ChangeCulture(CultureInfo newCulture)
        {
            if (GetSupportedCultures().Contains(newCulture))
            {
                var newResourceDictionary = LoadResourceDictionary(newCulture);
                if (newResourceDictionary != null)
                {
                    try
                    {
                        //用source 找index 刪掉舊的字典
                        var oldCulture = LoadResourceDictionary(Properties.Settings.Default.DefaultCulture);
                        if ((oldCulture != null))
                        {
                            Application.Current.Resources.MergedDictionaries.Remove(x => x.Source == oldCulture.Source);
                        }

                        Application.Current.Resources.MergedDictionaries.Add(newResourceDictionary);

                        Properties.Settings.Default.DefaultCulture = newCulture;
                        Properties.Settings.Default.Save();
                    }
                    catch
                    {

                    }


                }
            }
        }

        public static ResourceDictionary LoadResourceDictionary(CultureInfo culture)
        {
            if (!GetSupportedCultures().Contains(culture))
                return null;

            var cultureFName = _resourcePrefix + "." + culture.Name + ".xaml";
            string source = Path.Combine(_culturesFolder, cultureFName);
            string LoadedFileName = Path.Combine(System.Windows.Forms.Application.StartupPath, source);

            FileStream fileStream = new FileStream(LoadedFileName, FileMode.Open);
            ResourceDictionary resourceDictionary = XamlReader.Load(fileStream) as ResourceDictionary;
            resourceDictionary.Source = new System.Uri(source, UriKind.Relative);
            return resourceDictionary;
        }




    }

}