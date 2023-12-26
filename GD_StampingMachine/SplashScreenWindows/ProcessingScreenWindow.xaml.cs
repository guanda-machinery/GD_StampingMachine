using DevExpress.Xpf.Core;

namespace GD_CommonLibrary.SplashScreenWindows
{
    /// <summary>
    /// ProcessingScreenWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessingScreenWindow : SplashScreenWindow
    {
        public ProcessingScreenWindow()
        {
            InitializeComponent();
            var orgintopmost = this.Topmost;
            this.Topmost = true;
            this.Topmost = orgintopmost;
        }
    }
}
