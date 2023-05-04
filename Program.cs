using System;
using System.Xml.Linq;
using CRUD_library;
namespace factory_team5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"На данный момент заводом выпущено {DllInfo.product_count()} единиц продукции\n");
            bool b = true;
            while (b) {
            Console.WriteLine("Добавить работника  /1");
            Console.WriteLine("Добавить цех /2");
            Console.WriteLine("Редактировать данные о сотруднике /3");
            Console.WriteLine("Добавить данные о выпущенной продукции завода /4");
            Console.WriteLine("Поиск продукции по серийному номеру /5");
            Console.WriteLine("Просмотреть статистику о сотруднике /6");
            Console.WriteLine("Посмотреть статистику завода /7");
            Console.WriteLine("Удалить сотрудника /8");
            Console.WriteLine("Удалить цех /9");
            Console.WriteLine("Выйти из программы /0");
            int menu_value = Convert.ToInt32(Console.ReadLine());
                switch (menu_value)
                {
                    case 1:
                        DllFunctionsCreate.create_staff();
                        continue;
                    case 2:
                        DllFunctionsCreate.create_workshop();
                        continue;
                    case 3:
                        DllFunctionUpdate.update_staff();
                        continue;
                    case 4:
                        DllFunctionsCreate.create_product();
                        continue;
                    case 5:
                        DllInfo.get_info_about_product();
                        continue;
                    case 6:
                        DllInfo.get_info_about_worker();
                        continue;
                    case 7:
                        DllInfo.factory_statistics();
                        continue;
                    case 8:
                        DllDelete.delete_staff();
                        continue;
                    case 9:
                        DllDelete.delete_workshop();
                        continue;
                    case 0:
                        b = false;
                        break;
                }

                }
        }
        
    }
}
