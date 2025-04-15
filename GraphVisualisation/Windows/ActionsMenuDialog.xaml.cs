using System.Windows;
namespace GraphVisualisation
{
    /// <summary>
    /// Interaction logic for ActionsMenuDialog.xaml
    /// </summary>
    public partial class ActionsMenuDialog : Window
    {
        public int Action {  get; private set; }
        public ActionsMenuDialog()
        {
            InitializeComponent();
        }

        private void AddButtonClick (object sender, RoutedEventArgs e)
        {
            Action = 1;
            DialogResult = true;
            Close();
        }
        private void SubButtonClick(object sender, RoutedEventArgs e)
        {
            Action = 2;
            DialogResult = true;
            Close();
        }
        private void ImportButtonClick(object sender, RoutedEventArgs e)
        {
            Action = 3;
            DialogResult = true;
            Close();
        }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Action = 4;
            DialogResult = true;
            Close();
        }
        
        private void DeepFirstSearchClick(object sender, RoutedEventArgs e)
        {
            Action = 5;
            DialogResult = true;
            Close();
        }
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Action = 6;
            DialogResult = false;
            Close();
        }
        
    }
}
