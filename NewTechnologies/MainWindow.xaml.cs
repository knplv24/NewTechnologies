using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NewTechnologies
{
    public partial class MainWindow : Window
    {
        CompanyEntities db = new CompanyEntities();

        public MainWindow()
        {
            InitializeComponent();
            LoadRequests();
        }

        // загрузка списка заявок
        private void LoadRequests()
        {
            try
            {
                var grouped = db.Поставка_продукта
                    .GroupBy(r => r.Код_партнера) 
                    .ToList();

                var result = grouped.Select(g =>
                {
                    var first = g.First(); 

                    return new RequestViewModel
                    {
                        // основные данные партнера
                        PartnerId = first.Партнер.Код,
                        PartnerType = first.Партнер.Тип_партнера1?.Наименование ?? "—",
                        PartnerName = first.Партнер?.Наименование_партнера ?? "—",
                        Director = first.Партнер?.Директор ?? "—",
                        Address = first.Партнер?.Юридический_адрес_партнера ?? "—",
                        Phone = first.Партнер?.Телефон_партнера ?? "—",
                        Email = first.Партнер?.Электронная_почта_партнера ?? "—",
                        Rating = first.Партнер?.Рейтинг ?? 0,

                        // расчет общей стоимости продукции
                        Cost = Math.Round(
                            g.Sum(x =>
                                (x.Количество_продукции ?? 0) *
                                (decimal)(x.Продукт?.Минимальная_стоимость_для_партнера ?? 0)
                            ),
                            2 // округление до 2 знаков после запятой
                        )
                    };
                })
                .ToList();

                RequestsList.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заявок:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // обработчик двойного клика
        private void RequestsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RequestsList.SelectedItem is RequestViewModel selected)
            {
                var editWindow = new RequestEditWindow(selected);
                editWindow.ShowDialog();
                LoadRequests();
            }
        }

        // добавление новой заявки
        private void AddRequestButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new RequestEditWindow();
            editWindow.ShowDialog();
            LoadRequests();
        }

        // редактировани выбранной заявки
        private void EditRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is RequestViewModel selected)
            {
                var editWindow = new RequestEditWindow(selected);
                editWindow.ShowDialog();
                LoadRequests();
            }
            else
            {
                MessageBox.Show("Выберите заявку для редактирования.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // расчет материала
        private void OpenProductsButton_Click(object sender, RoutedEventArgs e)
        {
            PartnerRequestsMaterialForm prodwin = new PartnerRequestsMaterialForm();
            prodwin.Show();
            this.Close();
        }
    }
}
