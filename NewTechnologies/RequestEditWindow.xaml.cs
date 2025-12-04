using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NewTechnologies
{
    public partial class RequestEditWindow : Window
    {
        private RequestViewModel request;
        private List<ProductSelectionItem> productItems;
        CompanyEntities db = new CompanyEntities();

        public RequestEditWindow()
        {
            InitializeComponent();
            LoadPartnerTypes();
            LoadProducts();
        }

        // конструктор для редактирования существующего запроса
        public RequestEditWindow(RequestViewModel existingRequest) : this()
        {
            request = existingRequest;
            LoadData();
        }

        // загрузка типа партнера
        private void LoadPartnerTypes()
        {
            PartnerTypeCombo.ItemsSource = db.Тип_партнера
                .Select(t => t.Наименование)
                .ToList();
        }

        // загрузка продуктов
        private void LoadProducts()
        {
            // получение всех продуктов
            var allProducts = db.Продукт
                .Select(p => new ProductSelectionItem
                {
                    ProductId = p.Код,
                    ProductName = p.Наименование_продукции,
                    IsSelected = false,
                    Quantity = 1
                })
                .ToList();

            productItems = allProducts;
            ProductsGrid.ItemsSource = productItems;
        }

        // загрузка данных существующего запроса в форму
        private void LoadData()
        {
            if (request == null) return;

            PartnerTypeCombo.SelectedItem = request.PartnerType;
            PartnerNameBox.Text = request.PartnerName;
            DirectorBox.Text = request.Director;
            AddressBox.Text = request.Address;
            RatingBox.Text = request.Rating.ToString();
            PhoneBox.Text = request.Phone;
            EmailBox.Text = request.Email;

            // загружаем существующие поставки и их количество
            var supplies = db.Поставка_продукта
                .Where(s => s.Код_партнера == request.PartnerId)
                .ToList();

            foreach (var row in supplies)
            {
                var item = productItems.FirstOrDefault(p => p.ProductId == row.Продукция);
                if (item != null)
                {
                    item.IsSelected = true;
                    item.Quantity = row.Количество_продукции ?? 1;
                }
            }

            ProductsGrid.Items.Refresh(); // обновляем отображение таблицы
        }

        // кнопка сохранить
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // проверка корректности рейтинга
                if (!int.TryParse(RatingBox.Text, out int rating))
                    throw new Exception("Рейтинг должен быть числом.");

                Партнер partner;

                if (request != null)
                {
                    // редактируем существующего партнера
                    partner = db.Партнер.First(p => p.Код == request.PartnerId);
                }
                else
                {
                    // создаем нового партнера
                    partner = new Партнер();
                    db.Партнер.Add(partner);
                }

                // заполняем основные поля партнера
                partner.Наименование_партнера = PartnerNameBox.Text;
                partner.Директор = DirectorBox.Text;
                partner.Юридический_адрес_партнера = AddressBox.Text;
                partner.Телефон_партнера = PhoneBox.Text;
                partner.Электронная_почта_партнера = EmailBox.Text;
                partner.Рейтинг = rating;

                // определяем тип партнера
                var type = db.Тип_партнера
                    .FirstOrDefault(t => t.Наименование == PartnerTypeCombo.SelectedItem.ToString());
                partner.Тип_партнера = type?.Код;

                db.SaveChanges();

                // сохранение продуктов
                var existing = db.Поставка_продукта
                    .Where(s => s.Код_партнера == partner.Код)
                    .ToList();

                var selected = productItems.Where(p => p.IsSelected).ToList();

                // удаляем продукты, которые были убраны
                foreach (var old in existing)
                {
                    if (!selected.Any(s => s.ProductId == old.Продукция))
                        db.Поставка_продукта.Remove(old);
                }

                // добавляем новые и обновляем существующие
                foreach (var sel in selected)
                {
                    var existingSupply = existing.FirstOrDefault(s => s.Продукция == sel.ProductId);

                    if (existingSupply == null)
                    {
                        // добавляем новую поставку
                        db.Поставка_продукта.Add(new Поставка_продукта
                        {
                            Код_партнера = partner.Код,
                            Продукция = sel.ProductId,
                            Количество_продукции = sel.Quantity
                        });
                    }
                    else
                    {
                        // обновляем количество существующей поставки
                        existingSupply.Количество_продукции = sel.Quantity;
                    }
                }

                db.SaveChanges();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message);
            }
        }

        // кнопка отмена
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // класс для представления продукта в таблице с выбором и количеством
    public class ProductSelectionItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsSelected { get; set; }
        public int Quantity { get; set; }  
    }
}
