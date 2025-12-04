using System;
using System.Linq;
using System.Windows;

namespace NewTechnologies
{
    public partial class PartnerRequestsMaterialForm : Window
    {
        CompanyEntities db = new CompanyEntities();

        public PartnerRequestsMaterialForm()
        {
            InitializeComponent();
            LoadData();
        }

        // загрузка типа продукта и типа материала
        private void LoadData()
        {
            ProductTypeCombo.ItemsSource = db.Тип_продукта.ToList();
            ProductTypeCombo.DisplayMemberPath = "Тип_продукции";
            ProductTypeCombo.SelectedValuePath = "Код";

            MaterialTypeCombo.ItemsSource = db.Тип_материала.ToList();
            MaterialTypeCombo.DisplayMemberPath = "Тип_материала1";
            MaterialTypeCombo.SelectedValuePath = "Код";
        }

        // кнопка рассчитать
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductTypeCombo.SelectedValue == null || MaterialTypeCombo.SelectedValue == null)
                {
                    MessageBox.Show("Выберите тип продукции и материал!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // получение выбранных значений 
                int productTypeId = (int)ProductTypeCombo.SelectedValue;
                int materialTypeId = (int)MaterialTypeCombo.SelectedValue;

                // проверка корректности введенных чисел
                if (!int.TryParse(RequiredQuantityBox.Text, out int requiredQuantity) ||
                    !int.TryParse(StockQuantityBox.Text, out int stockQuantity) ||
                    !double.TryParse(Param1Box.Text, out double param1) ||
                    !double.TryParse(Param2Box.Text, out double param2))
                {
                    MessageBox.Show("Введите корректные значения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // вызов метода
                int result = MaterialCalculator.Calculate(productTypeId, materialTypeId, requiredQuantity,
                                                          stockQuantity, param1, param2, 1.0, db);

                // отображение результата
                ResultText.Text = result == -1 ? "Ошибка расчета!" : $"Необходимое количество материала: {result}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
