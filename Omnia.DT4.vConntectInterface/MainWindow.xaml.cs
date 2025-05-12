using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace Omnia.DT4.vConntectInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sourceDirectory;
        string destinationDirectory;
        string baseAddress;
        public MainWindow()
        {
            InitializeComponent();
            sourceDirectory= ConfigurationManager.AppSettings["sourceDirectory"].ToString();
            destinationDirectory = ConfigurationManager.AppSettings["destinationDirectory"].ToString();
            baseAddress = ConfigurationManager.AppSettings["baseAddress"].ToString();

            txtSourceFiles.Text = sourceDirectory;
            txtDestinationFiles.Text = destinationDirectory;

            // Create the desination directory for processing files
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(txtDestinationFiles.Text);
            
        }

        private async void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            // Copy and create new file names for the upload process
            var newDirectory = Directory.CreateDirectory(@destinationDirectory + "\\" + DateTime.Now.ToString("dd-mm-yyyy-hh-mm"));
            string ab = "";
            foreach( var item in Directory.EnumerateFileSystemEntries(sourceDirectory))
            {
                if (ab == "")
                    ab = "A";
                else
                    ab = "B";

                File.Copy(item, newDirectory.FullName + "\\" + "Audio_" + ab + ".wav");
            }

            // upload files
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            var jsonContent = new StringContent("{\"userId\": 1, \"title\": \"foo\", \"body\": \"bar\"}",
                                                Encoding.UTF8,
                                                "application/json");

            
            try
            {
                var response = await httpClient.PostAsync("/posts", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();
                txtStatus.Text = responseString;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + " More to follow " + ex.StackTrace.ToString(), "Upload Critical Error", MessageBoxButton.OK,MessageBoxImage.Error);
                txtStatus.Text = ex.Message;
            }
            finally 
            {
                httpClient.Dispose();
            }

            
            


        }
    }
}