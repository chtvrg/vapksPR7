using System.Linq;
using System.Windows;
using vapksPR7.Entity;
using vapksPR7.Helper;

namespace vapksPR7
{
    public partial class BookWindow : Window
    {
        public books CurrentBook { get; set; }
        public bool IsSaved { get; set; } = false;

        public BookWindow(books book = null)
        {
            InitializeComponent();

            var ctx = helper.GetContext();
            GenreBox.ItemsSource = ctx.genres.ToList();
            StatusBox.ItemsSource = ctx.status.ToList();

            if (book != null)
            {
                CurrentBook = book;
                NameBox.Text = book.name;
                YearBox.Text = book.year.ToString();
                AuthorLastBox.Text = book.author.lastName;
                AuthorNameBox.Text = book.author.name;
                AuthorMiddleBox.Text = book.author.middleName;
                GenreBox.SelectedItem = book.genres;
                StatusBox.SelectedItem = book.process.FirstOrDefault()?.status;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(AuthorLastBox.Text) ||
                string.IsNullOrWhiteSpace(AuthorNameBox.Text) ||
                !int.TryParse(YearBox.Text, out int year) ||
                GenreBox.SelectedItem == null ||
                StatusBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            var ctx = helper.GetContext();

            var author = ctx.author.FirstOrDefault(a =>
                a.lastName == AuthorLastBox.Text &&
                a.name == AuthorNameBox.Text &&
                a.middleName == AuthorMiddleBox.Text);

            if (author == null)
            {
                author = new author
                {
                    lastName = AuthorLastBox.Text,
                    name = AuthorNameBox.Text,
                    middleName = AuthorMiddleBox.Text
                };
                ctx.author.Add(author);
                ctx.SaveChanges();
            }

            if (CurrentBook == null)
            {
                CurrentBook = new books();
                ctx.books.Add(CurrentBook);
            }

            CurrentBook.name = NameBox.Text;
            CurrentBook.year = year;
            CurrentBook.id_author = author.id_author;
            CurrentBook.id_genre = ((genres)GenreBox.SelectedItem).id_genre;

            var proc = CurrentBook.process.FirstOrDefault();
            if (proc == null)
            {
                proc = new process
                {
                    id_book = CurrentBook.id_book,
                    id_status = ((status)StatusBox.SelectedItem).id_status
                };
                ctx.process.Add(proc);
            }
            else
            {
                proc.id_status = ((status)StatusBox.SelectedItem).id_status;
            }

            ctx.SaveChanges();
            IsSaved = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
