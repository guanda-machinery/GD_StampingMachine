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

       private static List<CultureInfo> _supportedCultures;

        public static List<CultureInfo> SupportedCultures
        {
            get
            {
                if(_supportedCultures == null)
                    _supportedCultures = GetSupportedCultures();
                return _supportedCultures;
            }
        }

        private static List<CultureInfo> GetSupportedCultures()
        {
            var CultList = new List<CultureInfo>();
            if (!_isFoundInstalledCultures)
            {
                CultureInfo cultureInfo = new CultureInfo("");
                List<string> files = Directory.GetFiles(string.Format("{0}\\{1}", System.Windows.Forms.Application.StartupPath, _culturesFolder))
                    .Where(s => s.Contains(_resourcePrefix) && s.ToLower().EndsWith("xaml")).ToList();

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

                if (CultList.Count > 0 && Properties.Settings.Default.DefaultCulture == null)
                {
                    ChangeCulture(Properties.Settings.Default.DefaultCulture);
                }

                _isFoundInstalledCultures = true;
            }
            return CultList;
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            if (SupportedCultures.Contains(culture))
            {
                string LoadedFileName = string.Format("{0}\\{1}\\{2}.{3}.xaml", System.Windows.Forms.Application.StartupPath, _culturesFolder
                    , _resourcePrefix, culture.Name);

                FileStream fileStream = new FileStream(@LoadedFileName, FileMode.Open);

                ResourceDictionary resourceDictionary = XamlReader.Load(fileStream) as ResourceDictionary;

                Application.Current.MainWindow.Resources.MergedDictionaries.Add(resourceDictionary);

                Properties.Settings.Default.DefaultCulture = culture;
                Properties.Settings.Default.Save();
            }
        }

    }

}