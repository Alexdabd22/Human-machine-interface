using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media;
using Forms = System.Windows.Forms;  

namespace TextEditor
{
    public partial class MainWindow : Window
    {
        // Колекція для вибору розміру шрифту
        public double[] FontSizes
        {
            get
            {
                return new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 40, 48, 60, 72 };
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _fontSize.ItemsSource = FontSizes;
            AddHeadersAndFooters();

            // новий абзац після колонтитула
            Paragraph newParagraph = new Paragraph();
            _richTextBox.Document.Blocks.Add(newParagraph); 

          
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _richTextBox.Focus(); 
                _richTextBox.CaretPosition = newParagraph.ContentStart; 
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        // Відкриття файлу
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog  
            {
                Filter = "Document files (*.rtf)|*.rtf"
            };

            if (dlg.ShowDialog() == true)
            {
                TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
                using (FileStream file = new FileStream(dlg.FileName, FileMode.Open))
                {
                    textRange.Load(file, DataFormats.Rtf);  
                }
            }
        }

        // Збереження файлу
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog  
            {
                Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                {
                    TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
                    textRange.Save(fs, DataFormats.Rtf);  
                }
            }
        }

        // Зміна кольору тексту
        private void SelectTextColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new Forms.ColorDialog(); 
            if (colorDialog.ShowDialog() == Forms.DialogResult.OK)
            {
                var brush = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                var selection = _richTextBox.Selection;
                if (selection != null && !selection.IsEmpty)
                {
                    selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                }
            }
        }

        // Друк документу
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(_richTextBox, "Printing Document");
            }
        }

        private void FontFamili_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (_richTextBox != null && ((System.Windows.Controls.ComboBox)sender).SelectedItem != null)
            {
                _richTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, ((System.Windows.Controls.ComboBox)sender).SelectedItem);
            }
        }

        private void FontSize_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (_richTextBox != null && ((System.Windows.Controls.ComboBox)sender).SelectedItem != null)
            {
                _richTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, ((System.Windows.Controls.ComboBox)sender).SelectedItem);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_richTextBox != null && !_richTextBox.Document.ContentStart.GetTextInRun(LogicalDirection.Forward).Trim().Equals(""))
            {
                var result = System.Windows.MessageBox.Show("Do you want to save changes before closing?", "Confirmation", MessageBoxButton.YesNoCancel);  
                if (result == MessageBoxResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            System.Windows.Application.Current.Shutdown();
        }

        private void CenterText_Click(object sender, RoutedEventArgs e)
        {
            if (!_richTextBox.Selection.IsEmpty)
            {
                var paragraph = _richTextBox.Selection.Start.Paragraph;
                if (paragraph != null)
                {
                    paragraph.TextAlignment = TextAlignment.Center;  // Вирівнюємо текст по центру
                }
            }
            else
            {
                MessageBox.Show("Please select some text to center.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        // Додавання колонтитулів
        private void AddHeadersAndFooters()
        {
            FlowDocument doc = _richTextBox.Document;

            // Створення верхнього колонтитулу
            Paragraph header = new Paragraph(new Run("Заголовок документа"));
            header.TextAlignment = TextAlignment.Center;
            header.FontSize = 14;
            header.FontWeight = FontWeights.Bold;

            // Створення нижнього колонтитулу
            Paragraph footer = new Paragraph(new Run("Сторінка 1"));
            footer.TextAlignment = TextAlignment.Right;
            footer.FontSize = 12;
            footer.FontStyle = FontStyles.Italic;


            doc.Blocks.InsertBefore(doc.Blocks.FirstBlock, header);  
            doc.Blocks.Add(footer);  
        }


        private void SetRightIndent_Click(object sender, RoutedEventArgs e)
        {
            if (_richTextBox.Selection != null && !_richTextBox.Selection.IsEmpty)
            {
            
                if (int.TryParse(IndentValue.Text, out int indentValue))
                {
                    if (indentValue < 1 || indentValue > 50)
                    {
                        MessageBox.Show("Будь ласка, введіть значення відступу між 1 і 50.", "Неправильне значення", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

             
                    TextRange selectedTextRange = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);

               
                    Paragraph paragraph = selectedTextRange.Start.Paragraph;
                    if (paragraph != null)
                    {
                        paragraph.TextAlignment = TextAlignment.Right;
                        paragraph.Margin = new Thickness(0, 0, indentValue, 0);
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, введіть коректне числове значення для відступу.", "Невірний ввід", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Виберіть текст, щоб встановити відступ праворуч.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
