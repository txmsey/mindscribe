using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MindScribe
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MindScribe");
        private DateTime currentDate;
        private int selectedYear;
        private int selectedMonth;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime SelectedDate
        {
            get => currentDate;
            set
            {
                if (currentDate != value)
                {
                    currentDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Initialize with current date
            currentDate = DateTime.Today;
            selectedYear = currentDate.Year;
            selectedMonth = currentDate.Month;

            DataContext = this;

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // Start in fullscreen
            this.WindowState = WindowState.Maximized;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void YearButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(((Button)sender).Content.ToString(), out int year))
            {
                selectedYear = year;
                yearView.Visibility = Visibility.Collapsed;
                monthView.Visibility = Visibility.Visible;
                backButton.Visibility = Visibility.Visible;
            }
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(((Button)sender).Content.ToString(), out int month))
            {
                selectedMonth = month;
                monthView.Visibility = Visibility.Collapsed;
                ShowDayButtons();
                dayView.Visibility = Visibility.Visible;
            }
        }

        private void ShowDayButtons()
        {
            dayButtonsGrid.Children.Clear();

            int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var dayButton = new Button
                {
                    Content = day.ToString(),
                    Style = (Style)FindResource("SquareButton"),
                    Tag = day
                };
                dayButton.Click += DayButton_Click;
                dayButtonsGrid.Children.Add(dayButton);
            }
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int day)
            {
                try
                {
                    SelectedDate = new DateTime(selectedYear, selectedMonth, day);

                    dayView.Visibility = Visibility.Collapsed;
                    journalView.Visibility = Visibility.Visible;

                    backButton.Visibility = Visibility.Visible;
                    saveButton.Visibility = Visibility.Visible;
                    newButton.Visibility = Visibility.Visible;

                    LoadJournalForCurrentDate();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("Invalid date selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GetJournalFileName(DateTime date)
        {
            return Path.Combine(appDataPath, $"journal_{date:yyyyMMdd}.txt");
        }

        private void LoadJournalForCurrentDate()
        {
            string fileName = GetJournalFileName(currentDate);
            if (File.Exists(fileName))
            {
                TextRange range = new TextRange(
                    journalRichTextBox.Document.ContentStart,
                    journalRichTextBox.Document.ContentEnd);

                range.Text = File.ReadAllText(fileName);
            }
            else
            {
                journalRichTextBox.Document.Blocks.Clear();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(
                journalRichTextBox.Document.ContentStart,
                journalRichTextBox.Document.ContentEnd);

            string fileName = GetJournalFileName(currentDate);
            File.WriteAllText(fileName, range.Text);
            MessageBox.Show("Journal entry saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            journalRichTextBox.Document.Blocks.Clear();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (journalView.Visibility == Visibility.Visible)
            {
                journalView.Visibility = Visibility.Collapsed;
                dayView.Visibility = Visibility.Visible;
                saveButton.Visibility = Visibility.Collapsed;
                newButton.Visibility = Visibility.Collapsed;
            }
            else if (dayView.Visibility == Visibility.Visible)
            {
                dayView.Visibility = Visibility.Collapsed;
                monthView.Visibility = Visibility.Visible;
            }
            else if (monthView.Visibility == Visibility.Visible)
            {
                monthView.Visibility = Visibility.Collapsed;
                yearView.Visibility = Visibility.Visible;
                backButton.Visibility = Visibility.Collapsed;
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            TextSelection selection = journalRichTextBox.Selection;
            if (!selection.IsEmpty)
            {
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            TextSelection selection = journalRichTextBox.Selection;
            if (!selection.IsEmpty)
            {
                selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            TextSelection selection = journalRichTextBox.Selection;
            if (!selection.IsEmpty)
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }

        private void StrikethroughButton_Click(object sender, RoutedEventArgs e)
        {
            TextSelection selection = journalRichTextBox.Selection;
            if (!selection.IsEmpty)
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }
    }
}