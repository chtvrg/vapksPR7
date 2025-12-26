using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using vapksPR7.Entity;
using vapksPR7.Helper;

namespace vapksPR7
{
    public partial class MainWindow : Window
    {
        ObservableCollection<BookItem> books;
        ICollectionView booksView;

        public MainWindow()
        {
            InitializeComponent();
            LoadLists();
            LoadBooks();
        }

        void LoadLists()
        {
            var ctx = helper.GetContext();
            GenreBox.ItemsSource = ctx.genres.ToList();
            StatusBox.ItemsSource = ctx.status.ToList();
        }

        void LoadBooks()
        {
            var ctx = helper.GetContext();
            books = new ObservableCollection<BookItem>(
                ctx.books.ToList().Select(b => new BookItem
                {
                    Id = b.id_book,
                    Name = b.name,
                    Author = (b.author.lastName + " " + b.author.name + " " + b.author.middleName).Trim(),
                    Year = (int)b.year,
                    Genre = b.genres.genre,
                    Status = b.process.Any() ? b.process.First().status.status1 : "Не начата"
                })
            );

            booksView = CollectionViewSource.GetDefaultView(books);
            BooksGrid.ItemsSource = booksView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new BookWindow();
            win.Owner = this;
            win.ShowDialog();
            if (win.IsSaved) LoadBooks();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem == null) return;
            var item = (BookItem)BooksGrid.SelectedItem;
            var ctx = helper.GetContext();
            var book = ctx.books.First(b => b.id_book == item.Id);

            var win = new BookWindow(book);
            win.Owner = this;
            win.ShowDialog();
            if (win.IsSaved) LoadBooks();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem == null) return;
            var item = (BookItem)BooksGrid.SelectedItem;
            var ctx = helper.GetContext();
            var book = ctx.books.First(b => b.id_book == item.Id);

            foreach (var p in book.process.ToList())
                ctx.process.Remove(p);

            ctx.books.Remove(book);
            ctx.SaveChanges();
            LoadBooks();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            booksView.Filter = obj =>
            {
                var b = obj as BookItem;
                if (b == null) return false;

                if (!string.IsNullOrWhiteSpace(NameBox.Text) && !b.Name.ToLower().Contains(NameBox.Text.ToLower()))
                    return false;

                if (!string.IsNullOrWhiteSpace(AuthorLastBox.Text) && !b.Author.ToLower().Contains(AuthorLastBox.Text.ToLower()))
                    return false;

                if (!string.IsNullOrWhiteSpace(AuthorNameBox.Text) && !b.Author.ToLower().Contains(AuthorNameBox.Text.ToLower()))
                    return false;

                if (!string.IsNullOrWhiteSpace(AuthorMiddleBox.Text) && !b.Author.ToLower().Contains(AuthorMiddleBox.Text.ToLower()))
                    return false;

                if (int.TryParse(YearBox.Text, out int year) && b.Year != year)
                    return false;

                if (GenreBox.SelectedItem != null && b.Genre != ((genres)GenreBox.SelectedItem).genre)
                    return false;

                if (StatusBox.SelectedItem != null && b.Status != ((status)StatusBox.SelectedItem).status1)
                    return false;

                return true;
            };
            booksView.Refresh();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            NameBox.Clear();
            AuthorLastBox.Clear();
            AuthorNameBox.Clear();
            AuthorMiddleBox.Clear();
            YearBox.Clear();
            GenreBox.SelectedItem = null;
            StatusBox.SelectedItem = null;

            booksView.Filter = null;
            booksView.Refresh();
        }
    }
}
