using System.Configuration;
using System.Data;
using System.Windows;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.Diagram.Theming;
using Syncfusion.Windows.Forms;

namespace testing_Program
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxcdHVWRWddWUB1Wko=");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SfSkinManager.ApplyStylesOnApplication = true; // Applies theme globally

        }
    }

}
