using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTechnologies
{
    public class MaterialCalculator
    {
        public static int Calculate(int productTypeId, int materialTypeId, int requiredQuantity, int stockQuantity,
                                    double param1, double param2, double productCoefficient, CompanyEntities db)
        {
            try
            {
                // проверка на валидные значения
                if (productTypeId <= 0 || materialTypeId <= 0 || requiredQuantity <= 0 || param1 <= 0 || param2 <= 0)
                    return -1;

                // проверяем существование типа продукции
                var productType = db.Тип_продукта.FirstOrDefault(p => p.Код == productTypeId);
                if (productType == null) return -1;

                // проверяем существование типа материала
                var materialType = db.Тип_материала.FirstOrDefault(m => m.Код == materialTypeId);
                if (materialType == null) return -1;

                // количество продукции, которое нужно произвести с учетом наличия на складе
                int quantityToProduce = Math.Max(requiredQuantity - stockQuantity, 0);
                if (quantityToProduce == 0) return 0;

                // расчет материала на одну единицу продукции
                double materialPerUnit = param1 * param2 * productCoefficient;

                // процент брака материала
                double wastePercentage = materialType.Процент_брака_материала_ ?? 0.0;

                // с учетом брака
                double materialWithWaste = materialPerUnit * (1 + wastePercentage);

                // общее количество материала
                return (int)Math.Ceiling(materialWithWaste * quantityToProduce);
            }
            catch
            {
                return -1;
            }
        }
    }
}
