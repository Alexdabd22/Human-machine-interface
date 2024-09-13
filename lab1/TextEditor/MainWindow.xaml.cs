using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Windows.Input;

using RichTextBox = System.Windows.Controls.RichTextBox;
using DataFormats = System.Windows.DataFormats;
using ComboBox = System.Windows.Controls.ComboBox;

namespace TextEditor
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<TabItem> Tabs { get; set; }
        public TabItem SelectedTab { get; set; }

        private bool IsSaved { get; set; } = true;

        private RichTextBox CurrentRichTextBox
        {
            get
            {
                if (tabControl.SelectedItem is TabItem tabItem)
                {
                    return tabItem.Content as RichTextBox;
                }
                return null;
            }
        }

        public double[] FontSizes
        {
            get
            {
                return new double[] { 3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0,
                 9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0, 16.0, 17.0, 18.0,
                 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0,
                 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0, 80.0, 88.0, 96.0, 104.0, 112.0,
                 120.0, 128.0, 136.0, 144.0 };
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Tabs = new ObservableCollection<TabItem>();
            AddNewTab("New Document");
        }

        private void AddNewTab(string header)
        {
            var newRichTextBox = new RichTextBox
            {
                Document = new FlowDocument(new Paragraph(new Run("")))
            };

            var newTab = new TabItem
            {
                Header = header,
                Content = newRichTextBox
            };

            Tabs.Add(newTab);
            SelectedTab = newTab;

            newRichTextBox.Document = new FlowDocument(new Paragraph(new Run("")));
        }
        private void UpdateRichTextBoxDocument(RichTextBox richTextBox, FlowDocument document)
        {
            richTextBox.Document = document;
        }

        public ICommand CloseTabCommand => new RelayCommand<TabItem>(CloseTab);

        private void CloseTab(TabItem tab)
        {
            if (Tabs.Contains(tab))
            {
                Tabs.Remove(tab);
            }
        }

        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            public RelayCommand(Action<T> execute) => _execute = execute;

            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter) => _execute((T)parameter);

            public event EventHandler CanExecuteChanged;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab("New Document");
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Document files (*.rtf)|*.rtf";
            var result = dlg.ShowDialog();
            if (result == true && CurrentRichTextBox != null)
            {
                TextRange t = new TextRange(CurrentRichTextBox.Document.ContentStart, CurrentRichTextBox.Document.ContentEnd);
                using (FileStream file = new FileStream(dlg.FileName, FileMode.Open))
                {
                    t.Load(file, DataFormats.Rtf);
                }
                IsSaved = true;
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                RichTextBox currentRichTextBox = GetSelectedRichTextBox();
                if (currentRichTextBox != null)
                {
                    printDialog.PrintVisual(currentRichTextBox, "Printing Document");
                }
            }
        }

        private RichTextBox GetSelectedRichTextBox()
        {
            if (tabControl.SelectedItem is TabItem selectedTab && selectedTab.Content is RichTextBox richTextBox)
            {
                return richTextBox;
            }
            return null;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                var result = System.Windows.MessageBox.Show("Do you want to save changes?", "Message", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    // Save changes
                    SaveCurrentDocument();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    // Cancel closing
                    return;
                }
            }

            // Close the application or document
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentDocument();
        }

        private void SelectTextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                if (CurrentRichTextBox != null)
                {
                    TextSelection selection = CurrentRichTextBox.Selection;
                    if (selection != null && !selection.IsEmpty)
                    {
                        selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                    }
                }
            }
        }

        private void SaveCurrentDocument()
        {
            RichTextBox richTextBox = CurrentRichTextBox;
            if (richTextBox == null)
                return;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                {
                    TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    textRange.Save(fs, DataFormats.Rtf);
                }
                IsSaved = true;
            }
        }

        private void FontFamili_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentRichTextBox != null)
            {
                CurrentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, ((ComboBox)sender).SelectedItem);
            }
        }

        private void FontSize_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentRichTextBox != null)
            {
                CurrentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, ((ComboBox)sender).SelectedItem);
            }
        }
    }
}
