using System;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Eventing.Reader;

namespace factory_team5
{
    public class DllAuth
    {
        public static (bool superuser, bool log, string id, string workshop_id) login()
        {

            XDocument xdoc = XDocument.Load("auth_users.xml");
            bool b = true, c = true, x = true, y = true;
            int count = 0, auth_value = 0;
            string id = "", workshop_id = "";
            bool superuser = false;
            while (x)
            {
                XDocument xdoc1 = XDocument.Load("last_auth_user.xml");
                foreach (XElement xel in xdoc1.Element("last_user").Elements("user"))
                {
                    count++;
                    try
                    {
                        id = xel.Element("id").Value;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                if (count > 0)
                {
                    Console.Clear();
                    Console.WriteLine($"Здравствуйте {xdoc1.Element("last_user").Element("user").Element("login").Value}! Войти в этот аккаунт /1 или в другой /2 ?");
                    try
                    {
                        auth_value = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Некорректный ввод. Попробуйте еще раз\n");
                        continue;
                    }
                    if (auth_value == 1)
                    {
                        while (c)
                        {
                            Console.WriteLine("Введите пароль");
                            string password = Console.ReadLine();
                            foreach (XElement xel in xdoc.Element("users").Elements("user"))
                            {
                                if (xel.Attribute("id").Value == id & xel.Element("password").Value == password)
                                {
                                    c = false;
                                    if (password == "superuser") superuser = true;
                                    x = false;
                                    workshop_id = xel.Element("workshop_id").Value;
                                    return (superuser, true, id, workshop_id);
                                }
                            }
                            if (c == true) Console.WriteLine("Пароль введен неправильно. Попробуйте еще раз");
                        }
                    }
                    else if (auth_value == 2)
                    {
                        logout();
                        count = 0;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ввод. Попробуйте еще раз\n");
                        continue;
                    }
                }
                else
                {
                    while (b)
                    {
                        while (y)
                        {
                            Console.WriteLine("Введите логин");
                            string login = Console.ReadLine().ToLower();
                            foreach (XElement xel in xdoc.Element("users").Elements("user"))
                            {
                                if (xel.Element("login").Value.ToLower() == login)
                                {
                                    id = xel.Attribute("id").Value;
                                    xdoc1.Element("last_user").Add(new XElement("user",
                                                                    new XElement("login", login),
                                                                    new XElement("id", id)));
                                    xdoc1.Save("last_auth_user.xml");
                                    b = false;
                                    if (login == "superuser") superuser = true;
                                    y = false;
                                }
                            }
                            if (y) Console.WriteLine("Логин введен неправильно. Попробуйте еще раз");
                        }
                        if (b == true) Console.WriteLine("Логин введен неправильно. Попробуйте еще раз");
                        while (c)
                        {
                            Console.WriteLine("Введите пароль");
                            string password = Console.ReadLine();
                            foreach (XElement xel in xdoc.Element("users").Elements("user"))
                            {
                                if (xel.Attribute("id").Value == id & xel.Element("password").Value == password)
                                {
                                    c = false;
                                    if (password == "superuser") superuser = true;
                                    x = false;
                                    workshop_id = xel.Element("workshop_id").Value;
                                    return (superuser, true, id, workshop_id);
                                }
                            }
                            if (c == true) Console.WriteLine("Пароль введен неправильно. Попробуйте еще раз");
                        }
                        break;
                    }
                }

            }
            return (superuser, false, id, workshop_id);
        }


        public static void logout()
        {
            XDocument xdoc = XDocument.Load("last_auth_user.xml");
            xdoc.Element("last_user").Element("user").Remove();
            xdoc.Save("last_auth_user.xml");
        }
    }
    public class DllFunctionsCreate
    {

        //Функция для проверки ввода айди/номера
        public static (string exept, int number) check_number(string id, string type = "", string id1 = "", string nnew = "")
        {
            int number = 0;
            bool b = true;
            string exept = "";

            do
            {
                try
                {
                    Console.WriteLine($"Введите {nnew}{id}{type}: ");
                    number = Convert.ToInt32(Console.ReadLine());
                    exept = "";
                    if (number != 0) b = false;
                }
                catch (Exception e)
                {
                    exept = Convert.ToString(e);
                    Console.WriteLine($"Некорректный ввод {id1}!\nПопробуйте еще раз");
                }
            }
            while (b);
            var result = (exept, number);
            return result;
        }
        public static int get_id(string path, string root_el, string el)
        {
            XDocument xdoc = XDocument.Load(path);
            XElement lastElement = xdoc.Element(root_el).Elements(el).ToList().Last();
            return Convert.ToInt32(lastElement.Attribute("id").Value) + 1;
        }

        public static void create_workshop()
        {
            XDocument xdoc = XDocument.Load("workshops.xml");
            XElement workshops = xdoc.Element("workshops");
            bool b = true;
            // проверка ввода и получение номера цеха
            int number = check_number("номер", "цеха", "номера").number;

            Console.WriteLine("Введите тип продукта");
            string type = Console.ReadLine();
            workshops.Add(new XElement("workshop",
                            new XAttribute("id", number),
                            new XElement("type", type)));
            xdoc.Save("workshops.xml");
            Console.WriteLine("Успешно");
        }

        public static void create_staff()
        {
            XDocument xdoc_workshops = XDocument.Load("workshops.xml");

            XDocument xdoc_staff = XDocument.Load("staff.xml");
            XElement staff = xdoc_staff.Element("staff");

            bool b = true, c = true;
            string fullname = "";
            var birthday = "";
            //цикл, в котором проевряется правильность ввода и наличия цеха в xml
            while (b)
            {
                Console.WriteLine("Введите ФИО: ");
                fullname = Console.ReadLine();

                Console.WriteLine("Введите дату рождения в формате дд.мм.гггг: ");
                birthday = Convert.ToString(DllInfo.inputDate()).Substring(0, 10);

                int salary = DllFunctionsCreate.check_number("оклад сотрудника").number;
                do
                {
                    var num_exept = check_number("номер", "цеха", "номера");
                    foreach (XElement workshopElement in xdoc_workshops.Element("workshops").Elements("workshop"))
                    {
                        if (Convert.ToInt32(workshopElement.Attribute("id").Value) == num_exept.number)
                        {
                            staff.Add(new XElement("worker",
                                new XAttribute("id", get_id("staff.xml", "staff", "worker")),
                                new XElement("fullname", fullname),
                                new XElement("birthday", birthday),
                                new XElement("salary", salary),
                                new XElement("number", num_exept.number)));
                            xdoc_staff.Save("staff.xml");
                            Console.WriteLine("Успешно");

                            c = false;
                            b = false;
                            break;
                        }
                        else c = true;
                    }
                    if (c & num_exept.exept == "")
                    {
                        Console.WriteLine("Такого цеха не существует");
                    }
                }
                while (c);
            }

        }
        public static void create_product(bool superuser, string workshop_id)
        {
            XDocument xdoc_workshop = XDocument.Load("workshops.xml");
            XDocument xdoc = XDocument.Load("products.xml");
            XDocument xdoc_staff = XDocument.Load("staff.xml");
            XElement products = xdoc.Element("products");
            bool b = true, c = true, x = true;
            Console.WriteLine("Введите наименование продукции");
            string name = Console.ReadLine();
            string type = "";
            if (superuser)
            {
                bool j = true;
                while (j)
                {
                    Console.WriteLine("Введите тип продукции");
                    type = Console.ReadLine();
                    foreach (XElement xel in xdoc_workshop.Element("workshops").Elements("workshop"))
                    {
                        if (xel.Element("type").Value == type)
                        {
                            type = xel.Element("type").Value;
                            j = false;
                        }
                    }
                    if (j) Console.WriteLine("Такого типа нет");
                }
            }
            else
            {
                foreach (XElement xel in xdoc_workshop.Element("workshops").Elements("workshop"))
                {
                    if (xel.Attribute("id").Value == workshop_id)
                    {
                        type = xel.Element("type").Value;
                    }
                }
            }
            int cost = DllFunctionsCreate.check_number("цену за единицу продукции").number;
            string employee_fullname = "";
            // цикл с проверкой на наличие сотрудника в xml
            if (superuser)
            {
                while (b)
                {
                    Console.WriteLine("Введите ФИО сотрудника выпустившего продукцию");
                    employee_fullname = Console.ReadLine();
                    foreach (XElement xel in xdoc_staff.Element("staff").Elements("worker"))
                    {
                        if (employee_fullname.ToLower() == xel.Element("fullname").Value.ToLower())
                        {
                            employee_fullname = xel.Element("fullname").Value;
                            c = true;
                            b = false;
                            break;
                        }
                        else { b = true; c = false; }
                    }
                    if (!c) { Console.WriteLine("Сотрудник не найден"); }
                }
            }
            else
            {
                while (x)
                {
                    Console.WriteLine("Введите ФИО сотрудника выпустившего продукцию");
                    employee_fullname = Console.ReadLine();
                    foreach (XElement xel in xdoc_staff.Element("staff").Elements("worker"))
                    {
                        if (employee_fullname.ToLower() == xel.Element("fullname").Value.ToLower())
                        {
                            if (xel.Element("number").Value == workshop_id)
                            {
                                employee_fullname = xel.Element("fullname").Value;
                                x = false;
                                break;
                            }
                        }
                    }
                    if (x) Console.WriteLine("Сотрудник не найден в вашем цехе");
                }
            }
            var num_exept = check_number("серийный номер", "продукции", "серийного номера");
            string serial_number = "SNP" + Convert.ToString(num_exept.number);
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            string dt_day = Convert.ToString(dt.Day);
            string dt_month = Convert.ToString(dt.Month);
            if (dt.Month < 10 & dt.Day < 10)
            {
                dt_day = "0" + dt_day;
                dt_month = "0" + dt_month;
            }
            else if (dt.Month < 10)
            {
                dt_month = "0" + dt_month;
            }
            else if (dt.Day < 10)
            {
                dt_day = "0" + dt_day;
            }
            string date = Convert.ToString(dt_day) + "." + Convert.ToString(dt_month) + "." + Convert.ToString(dt.Year)
                        + " " + Convert.ToString(dt.Hour) + ":" + Convert.ToString(dt.Minute) + ":" + Convert.ToString(dt.Second);
            products.Add(new XElement("product",
                            new XAttribute("id", get_id("products.xml", "products", "product")),
                            new XElement("name", name),
                            new XElement("type", type),
                            new XElement("cost", cost),
                            new XElement("employee_fullname", employee_fullname),
                            new XElement("serial_number", serial_number),
                            new XElement("date", date)));

            DllSalary.add_percent_to_salary(employee_fullname, cost);
            xdoc.Save("products.xml");

        }
        public static void add_defective_products(bool superuser, string workshop_id)
        {
            XDocument xdoc_products = XDocument.Load("products.xml");
            XDocument xdoc_defective_products = XDocument.Load("defective_products.xml");
            XDocument xdoc_workshop = XDocument.Load("workshops.xml");

            bool b = true;

            if (superuser)
            {
                DllInfo.get_info("products.xml", "products", "product");
                while (b)
                {
                    int id = check_number("ID").number;
                    foreach (XElement xel in xdoc_products.Element("products").Elements("product"))
                    {
                        if (Convert.ToInt32(xel.Attribute("id").Value) == id)
                        {
                            xdoc_defective_products.Element("defective_products").Add(new XElement("defective_product",
                                                                                         new XAttribute("id", xel.Attribute("id").Value),
                                                                                        new XElement("name", xel.Element("name").Value),
                                                                                        new XElement("type", xel.Element("type").Value),
                                                                                        new XElement("cost", xel.Element("cost").Value),
                                                                                        new XElement("employee_fullname", xel.Element("employee_fullname").Value),
                                                                                        new XElement("serial_number", xel.Element("serial_number").Value),
                                                                                        new XElement("date", xel.Element("date").Value)));
                            xel.Remove();
                            xdoc_products.Save("products.xml");
                            xdoc_defective_products.Save("defective_products.xml");
                            Console.WriteLine("Удалено");
                            b = false;
                        }
                        else b = true;
                    }
                    if (b) Console.WriteLine("Сотрудник не найден");
                }
            }
            else
            {
                string product_type = "";
                foreach (XElement xel_workshop in xdoc_workshop.Element("workshops").Elements("workshop"))
                {
                    if (workshop_id == xel_workshop.Attribute("id").Value)
                    {
                        product_type = xel_workshop.Element("type").Value;
                    }
                }
                foreach(XElement xel_p1 in xdoc_products.Element("products").Elements("product"))
                {
                    if (xel_p1.Element("type").Value == product_type)
                    {
                        DllInfo.product_output(xel_p1);
                    }
                }

                while (b)
                {
                    int id = check_number("ID").number;
                    foreach (XElement xel_p in xdoc_products.Element("products").Elements("product"))
                    {
                        if (id == Convert.ToInt32(xel_p.Attribute("id").Value))
                        {
                            xdoc_defective_products.Element("defective_products").Add(new XElement("defective_product",
                                                                                        new XAttribute("id", xel_p.Attribute("id").Value),
                                                                                        new XElement("name", xel_p.Element("name").Value),
                                                                                        new XElement("type", xel_p.Element("type").Value),
                                                                                        new XElement("cost", xel_p.Element("cost").Value),
                                                                                        new XElement("employee_fullname", xel_p.Element("employee_fullname").Value),
                                                                                        new XElement("serial_number", xel_p.Element("serial_number").Value),
                                                                                        new XElement("date", xel_p.Element("date").Value)));
                            xel_p.Remove();
                            xdoc_products.Save("products.xml");
                            xdoc_defective_products.Save("defective_products.xml");
                            Console.WriteLine("Удалено");
                            b = false;
                        }
                        else b = true;
                    }
                    if (b) Console.WriteLine("Сотрудник не найден");
                }
            }
        }
    }
    public class DllFunctionUpdate
    {
        public static void update_staff()
        {
            XDocument xdoc_staff = XDocument.Load("staff.xml");
            XDocument xdoc_workshops = XDocument.Load("workshops.xml");
            DllInfo.get_info("staff.xml", "staff", "worker");
            bool b = true, c = true, k = true;
            //  цикл do while в котором проверяется наличие сотрудника в xml
            do
            {
                var num_exept_for_id = DllFunctionsCreate.check_number("ID", "сотрудника", "id");
                foreach (XElement staffElement in xdoc_staff.Element("staff").Elements("worker"))
                {
                    if (Convert.ToInt32(staffElement.Attribute("id").Value) == num_exept_for_id.number)
                    {
                        // цикл while в котором проверяется наличие введенного цеха в xml
                        while (k)
                        {
                            c = false;
                            var num_exept_for_workshop = DllFunctionsCreate.check_number("номер", "цеха", "номера", "новый ");
                            foreach (XElement xel in xdoc_workshops.Element("workshops").Elements("workshop"))
                            {
                                if (Convert.ToInt32(xel.Attribute("id").Value) == num_exept_for_workshop.number)
                                {
                                    staffElement.Element("number").Value = Convert.ToString(num_exept_for_workshop.number);
                                    xdoc_staff.Save("staff.xml");
                                    Console.WriteLine("Успешно");
                                    b = false;
                                    k = false;
                                    break;
                                }
                                else b = true;
                            }
                            if (b & num_exept_for_workshop.exept == "")
                            {
                                Console.WriteLine("Такого цеха не существует");
                                k = true;
                            }

                        }
                    }
                    else c = true;


                }
                if (c)
                {
                    Console.WriteLine("Такого сотрудника нет");
                }
            }
            while (b);

        }
    }

    public class DllSalary
    {
        public static void add_percent_to_salary(string employee_fullmname, int cost)
        {
            XDocument xdoc_staff = XDocument.Load("staff.xml");
            XDocument xdoc_products = XDocument.Load("products.xml");

            foreach(XElement xel in xdoc_staff.Element("staff").Elements("worker"))
            {
                if(xel.Element("fullname").Value == employee_fullmname)
                {
                    double salary = Convert.ToInt32(xel.Element("salary").Value) + cost / 5;
                    xel.Element("salary").Value = Convert.ToString(salary);
                    Console.WriteLine(xel.Element("salary"));
                }
            }
            xdoc_staff.Save("staff.xml");
        }
    }
    public class DllInfo
    {
        // метод для оторбражения кол-ва выпущенной продукции за все время
        public static int product_count()
        {
            int count = 0;
            XDocument xdoc = XDocument.Load("products.xml");
            foreach (XElement xel in xdoc.Element("products").Elements("product"))
            {
                count++;
            }
            return count;
        }
        public static DateTime inputDate()
        {
            DateTime date;
            string input;

            do
            {
                input = Console.ReadLine();
                if (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, DateTimeStyles.None, out date))
                {
                    Console.WriteLine("Некорректный ввод даты");
                }
            }
            while (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, DateTimeStyles.None, out date));

            return date;
        }
        public static void factory_statistics(bool superuser, string workshop_id)
        {
            XDocument xdoc = XDocument.Load("products.xml");
            XDocument xdoc_workshop = XDocument.Load("workshops.xml");
            bool b = true, c = true;
            string workshop_type = "";
            if (!superuser)
            {
                foreach (XElement xel_workshop in xdoc_workshop.Element("workshops").Elements("workshop"))
                {
                    if (xel_workshop.Attribute("id").Value == workshop_id)
                    {
                        workshop_type = xel_workshop.Element("type").Value;
                    }
                }
            }
            while (b)
            {
                if (superuser) Console.WriteLine("Выберите в каком формате вы хотите посмотреть статистику\nДата 1\nДиапазон дат 2\nВся статистика 3\nСтатистика по цехам 4\nВ главное меню 0\n");

                else Console.WriteLine("Выберите в каком формате вы хотите посмотреть статистику\nДата 1\nДиапазон дат 2\nВся статистика 3\nВ главное меню 0\n");

                int value = Convert.ToInt32(Console.ReadLine());
                switch (value)
                {
                    case 1:
                        Console.WriteLine("Введите дату: ");
                        var date = Convert.ToString(inputDate()).Substring(0, 10);
                        foreach (XElement xel in xdoc.Element("products").Elements("product"))
                        {
                            if (date == Convert.ToString(xel.Element("date")).Substring(6, 10))
                            {
                                if (superuser)
                                {
                                    product_output(xel);
                                }
                                else
                                {
                                    if (xel.Element("type").Value == workshop_type)
                                    {
                                        product_output(xel);
                                    }
                                }
                            }

                        }
                        break;
                    case 2:
                        Console.WriteLine("Введите 1 дату: ");
                        var date1 = inputDate();
                        Console.WriteLine("Введите 2 дату: ");
                        var date2 = inputDate();
                        foreach (XElement xel in xdoc.Element("products").Elements("product"))
                        {
                            if (date1 <= DateTime.Parse(Convert.ToString(xel.Element("date")).Substring(6, 10)) & date2 >= DateTime.Parse(Convert.ToString(xel.Element("date")).Substring(6, 10)))
                            {
                                if (superuser)
                                {
                                    product_output(xel);
                                }
                                else
                                {
                                    if (xel.Element("type").Value == workshop_type)
                                    {
                                        product_output(xel);
                                    }
                                }
                            }
                        }
                        break;
                    case 3:
                        foreach (XElement xel in xdoc.Element("products").Elements("product"))
                        {

                            if (superuser)
                            {
                                product_output(xel);
                            }
                            else
                            {
                                if (xel.Element("type").Value == workshop_type)
                                {
                                    product_output(xel);
                                }
                            }
                        }
                        break;
                    case 4:
                        if (!superuser)
                        {
                            Console.WriteLine("Неправильный ввод");
                            break;
                        }
                        else
                        {
                            XDocument workshops = XDocument.Load("workshops.xml");
                            XDocument staff = XDocument.Load("staff.xml");
                            XDocument products = XDocument.Load("products.xml");
                            while (c)
                            {
                                var num_exept = DllFunctionsCreate.check_number("номер", "цеха", "номера");

                                var workshop = workshops.Descendants("workshop")
                                    .FirstOrDefault(w => Convert.ToInt32(w.Attribute("id").Value) == num_exept.number);

                                if (workshop == null)
                                {
                                    Console.WriteLine($"Цех с номером {num_exept.number} не найден.");
                                }
                                else
                                {
                                    Console.WriteLine($"Статистика по цеху: {workshop.Element("type").Value}");

                                    var workersInW = staff.Descendants("worker")
                                        .Where(w => w.Element("number").Value == workshop.Attribute("id").Value);

                                    foreach (var worker in workersInW)
                                    {
                                        Console.WriteLine($"Работник: {worker.Element("fullname").Value}");

                                        //var productsInW = products.Descendants("product")
                                        //    .Where(p => p.Element("employee_fullname").Value == worker.Element("fullname").Value &&
                                        //                p.Element("type").Value == workshop.Element("type").Value);

                                        int productCount = 0;
                                        foreach (var product in products.Element("products").Elements("product"))
                                        {
                                            if (product.Element("employee_fullname").Value == worker.Element("fullname").Value)
                                            {
                                                Console.WriteLine($"Продукт: {product.Element("name").Value}");

                                                productCount++;
                                                c = true;
                                            }
                                        }
                                        if (c)
                                        {
                                            Console.WriteLine($"Выпущено продуктов: {productCount}");
                                            Console.WriteLine();
                                            c = false;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case 0:
                        b = false;
                        break;
                    default:
                        Console.WriteLine("Неправильный ввод");
                        break;
                }
            }

        }
        public static void get_info(string path, string root_el, string el)
        {
            XDocument xdoc = XDocument.Load(path);

            foreach (XElement xel in xdoc.Element(root_el).Elements(el))
            {
                if (path == "staff.xml")
                {
                    Console.WriteLine($"ID: {xel.Attribute("id").Value}");
                    Console.WriteLine($"ФИО: {xel.Element("fullname").Value}");
                    Console.WriteLine($"День рождения: {xel.Element("birthday").Value}");
                    Console.WriteLine($"Оклад: {xel.Element("salary").Value}");
                    Console.WriteLine($"Номер цеха, в котором работает сотрудник: {xel.Element("number").Value}");
                }
                else if (path == "products.xml")
                {
                    product_output(xel);
                }
                else if (path == "workshops.xml")
                {
                    Console.WriteLine($"Номер цеха: {xel.Attribute("id").Value}");
                    Console.WriteLine($"Тип выпускаемой продукции: {xel.Element("type").Value}");
                }
            }
        }
        public static void get_info_about_product(bool superuser, string workshop_id)
        {
            XDocument xdoc = XDocument.Load("products.xml");
            XDocument xdoc_workshop = XDocument.Load("workshops.xml");
            XDocument xdoc_staff = XDocument.Load("staff.xml");
            bool c = true, b = true;
            string employee_fullname = "";
            // цикл для проверки наличия введенного серийного номера продукта в xml
            while (c)
            {
                Console.WriteLine("Введите серийный номер продукта: ");
                string serial_number = Console.ReadLine();

                foreach (XElement xel in xdoc.Element("products").Elements("product"))
                {
                    if (xel.Element("serial_number").Value == serial_number)
                    {
                        if (!superuser)
                        {
                            employee_fullname = xel.Element("employee_fullname").Value;
                            foreach (XElement xel1 in xdoc_staff.Element("staff").Elements("worker"))
                            {
                                if (xel1.Element("fullname").Value == employee_fullname & xel1.Element("number").Value == workshop_id)
                                {
                                    product_output(xel);
                                    b = false;
                                    c = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (XElement xel2 in xdoc.Element("products").Elements("product"))
                            {
                                if (xel2.Element("serial_number").Value == serial_number)
                                {
                                    product_output(xel2);
                                    b = false;
                                    c = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (b) Console.WriteLine("Результаты не найдены");
            }
        }
        public static void get_info_about_worker(bool superuser, string workshop_id)
        {
            XDocument xdoc = XDocument.Load("products.xml");
            XDocument xdoc_staff = XDocument.Load("staff.xml");
            bool b = true, c = true;
            // цикл в котором проверяется наличие сотрудника в xml
            while (c)
            {
                Console.WriteLine("Введите ФИО сотрудника");
                string employee_fullname = Console.ReadLine().ToLower();
                foreach (XElement xel in xdoc.Element("products").Elements("product"))
                {
                    if (!superuser)
                    {
                        if (xel.Element("employee_fullname").Value.ToLower() == employee_fullname)
                        {
                            foreach (XElement xel1 in xdoc_staff.Element("staff").Elements("worker"))
                            {
                                if (employee_fullname == xel1.Element("fullname").Value.ToLower() & xel1.Element("number").Value == workshop_id)
                                {
                                    product_output(xel);
                                    c = false;
                                    b = false;
                                    break;

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (XElement xel2 in xdoc.Element("products").Elements("product"))
                        {
                            if (xel.Element("employee_fullname").Value.ToLower() == employee_fullname)
                            {
                                product_output(xel);
                                c = false;
                                b = false;
                                break;
                            }
                        }
                    }
                }
                if (b) Console.WriteLine("Результаты не найдены. Попробуйте еще раз");
            }
        }
        public static void product_output(XElement xel)
        {
            XDocument xdoc = XDocument.Load("products.xml");
            Console.WriteLine($"ID: {xel.Attribute("id").Value}");
            Console.WriteLine($"Наименование продукции: {xel.Element("name").Value}");
            Console.WriteLine($"Тип продукции: {xel.Element("type").Value}");
            Console.WriteLine($"Цена за единицу продукции: {xel.Element("cost").Value}");
            Console.WriteLine($"ФИО сотрудника, выпустившего продукцию: {xel.Element("employee_fullname").Value}");
            Console.WriteLine($"Серийный номер продукции: {xel.Element("serial_number").Value}");
            Console.WriteLine($"Дата и время выпуска продукции: {xel.Element("date").Value}");
        }
    }
    public class DllDelete
    {
        public static void delete_staff()
        {
            DllInfo.get_info("staff.xml", "staff", "worker");
            XDocument xdoc_delete = XDocument.Load("deleted_staff.xml");
            XDocument xdoc = XDocument.Load("staff.xml");
            bool b = true;
            // цикл в котором проверяется наличие сотрудника в xml
            while (b)
            {
                var num_exept = DllFunctionsCreate.check_number("ID", "сотрудника", "ID");
                foreach (XElement xel in xdoc.Element("staff").Elements("worker"))
                {
                    if (Convert.ToInt32(xel.Attribute("id").Value) == num_exept.number)
                    {
                        xdoc_delete.Element("deleted_staff").Add(new XElement("deleted_worker",
                                                            new XAttribute("id", xel.Attribute("id").Value),
                                                            new XElement("fullname", xel.Element("fullname").Value),
                                                            new XElement("birthday", xel.Element("birthday").Value),
                                                            new XElement("number", xel.Element("number").Value)));
                        xel.Remove();
                        xdoc.Save("staff.xml");
                        xdoc_delete.Save("deleted_staff.xml");
                        Console.WriteLine("Удаленo");
                        b = false;
                        break;
                    }
                }
                if (b & num_exept.exept == "") Console.WriteLine("Такого сотрудника нет");
            }
        }
        public static void delete_workshop()
        {
            DllInfo.get_info("workshops.xml", "workshops", "workshop");
            XDocument xdoc = XDocument.Load("workshops.xml");
            XDocument xdoc_delete = XDocument.Load("deleted_workshops.xml");
            bool b = true;
            // цикл в котором проверяется наличие цеха в xml
            while (b)
            {
                var num_exept = DllFunctionsCreate.check_number("номер", "цеха", "номера");
                foreach (XElement xel in xdoc.Element("workshops").Elements("workshop"))
                {
                    if (Convert.ToInt32(xel.Attribute("id").Value) == num_exept.number)
                    {
                        xdoc_delete.Element("deleted_workshops").Add(new XElement("deleted_workshop",
                                                             new XAttribute("id", xel.Attribute("id").Value),
                                                             new XElement("type", xel.Element("type").Value)));
                        xel.Remove();
                        xdoc.Save("workshops.xml");
                        xdoc_delete.Save("deleted_workshops.xml");
                        Console.WriteLine("Удалено");
                        b = false;
                        break;
                    }
                }
                if (b & num_exept.exept == "") Console.WriteLine("Такого завода нет");
            }
        }
    }
    public class DllColor
    {
        public static string colorFile = "color.xml";

        public static void dllcolorconsole()
        {
            Console.WriteLine("Черный цвет /1");
            Console.WriteLine("Синий цвет /2");
            Console.WriteLine("Зеленый цвет /3");
            Console.WriteLine("Серый цвет /4");
            Console.WriteLine("Белый цвет /5");
            Console.WriteLine("Вернуться назад /6");
            int color_console = Convert.ToInt32(Console.ReadLine());
            switch (color_console)
            {
                case 1:
                    Console.BackgroundColor = ConsoleColor.Black;
                    SaveColorSettings();
                    Console.Clear();
                    dllcolor();
                    break;
                case 2:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    SaveColorSettings();
                    Console.Clear();
                    dllcolor();
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.Green;
                    SaveColorSettings();
                    Console.Clear();
                    dllcolor();
                    break;
                case 4:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    SaveColorSettings();
                    Console.Clear();
                    dllcolor();
                    break;
                case 5:
                    Console.BackgroundColor = ConsoleColor.White;
                    SaveColorSettings();
                    Console.Clear();
                    dllcolor();
                    break;
                case 6:
                    dllcolor();
                    break;
            }
        }

        public static void dllcolorfont()
        {
            Console.WriteLine("Черный цвет /1");
            Console.WriteLine("Синий цвет /2");
            Console.WriteLine("Серый цвет /3");
            Console.WriteLine("Зеленый цвет /4");
            Console.WriteLine("Белый цвет /5");
            Console.WriteLine("Вернуться назад /6");
            int color_font = Convert.ToInt32(Console.ReadLine());
            switch (color_font)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Black;
                    SaveColorSettings();
                    dllcolor();
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    SaveColorSettings();
                    dllcolor();
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Green;
                    SaveColorSettings();
                    dllcolor();
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    SaveColorSettings();
                    dllcolor();
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.White;
                    SaveColorSettings();
                    dllcolor();
                    break;
                case 6:
                    dllcolor();
                    break;
            }
        }

        public static void SaveColorSettings()
        {
            XElement root = new XElement("Colors",
                new XElement("ConsoleBackground", Console.BackgroundColor),
                new XElement("ConsoleForeground", Console.ForegroundColor)
            );

            XDocument doc = new XDocument(root);
            doc.Save(colorFile);
        }

        public static void LoadColorSettings()
        {
            if (File.Exists(colorFile))
            {
                XDocument doc = XDocument.Load(colorFile);
                XElement root = doc.Root;
                if (root != null)
                {
                    ConsoleColor background = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), root.Element("ConsoleBackground").Value);
                    ConsoleColor foreground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), root.Element("ConsoleForeground").Value);
                    Console.BackgroundColor = background;
                    Console.ForegroundColor = foreground;
                }
            }
        }

        public static void dllcolor()
        {
            Console.WriteLine("Изменить цвет консоли /1");
            Console.WriteLine("Изменить цвет шрифта /2");
            Console.WriteLine("Вернуться в меню /3");
            int color_menu = Convert.ToInt32(Console.ReadLine());
            switch (color_menu)
            {
                case 1:
                    dllcolorconsole();
                    break;
                case 2:
                    dllcolorfont();
                    break;
                case 3:
                    // Вернуться в меню или выполнить другое действие
                    break;
            }
        }
    }
    public class DllMenu
    {
        public static (bool, bool) superuser_panel(bool superuser, string workshop_id)
        {
            bool c = true, x = false;
            while (c)
            {
                Console.WriteLine($"На данный момент заводом выпущено {DllInfo.product_count()} единиц продукции\n");
                bool b = true;
                while (b)
                {
                    Console.WriteLine("Добавить работника  /1");
                    Console.WriteLine("Добавить цех /2");
                    Console.WriteLine("Редактировать данные о сотруднике /3");
                    Console.WriteLine("Добавить данные о выпущенной продукции завода /4");
                    Console.WriteLine("Поиск продукции по серийному номеру /5");
                    Console.WriteLine("Просмотреть статистику о сотруднике /6");
                    Console.WriteLine("Посмотреть статистику завода /7");
                    Console.WriteLine("Удалить сотрудника /8");
                    Console.WriteLine("Удалить цех /9");
                    Console.WriteLine("Настройки цвета /10");
                    Console.WriteLine("Выйти из аккаунта /11");
                    Console.WriteLine("Выйти из программы /0");
                    int menu_value = 0;
                    try
                    {
                        menu_value = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("\nНекорректный ввод. Попробуйте еще раз\n");
                        continue;
                    }
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
                            DllFunctionsCreate.create_product(superuser, workshop_id);
                            continue;
                        case 5:
                            DllInfo.get_info_about_product(superuser, workshop_id);
                            continue;
                        case 6:
                            DllInfo.get_info_about_worker(superuser, workshop_id);
                            continue;
                        case 7:
                            DllInfo.factory_statistics(superuser, workshop_id);
                            continue;
                        case 8:
                            DllDelete.delete_staff();
                            continue;
                        case 9:
                            DllDelete.delete_workshop();
                            continue;
                        case 10:
                            DllColor.dllcolor();
                            continue;
                        case 11:
                            DllAuth.logout();
                            b = false;
                            c = false;
                            break;
                        case 12:
                            DllFunctionsCreate.add_defective_products(superuser, workshop_id);
                            continue;
                        case 0:
                            b = false;
                            c = false;
                            x = true;
                            break;
                        default:
                            Console.WriteLine("\nНекорректный ввод. Попробуйте еще раз\n");
                            continue;
                    }
                }
            }
            return (false, x);
        }
        public static (bool, bool) admin_panel(bool superuser, string workshop_id)
        {
            bool c = true, x = false;
            while (c)
            {
                Console.WriteLine($"На данный момент заводом выпущено {DllInfo.product_count()} единиц продукции\n");
                bool b = true;
                while (b)
                {
                    Console.WriteLine("Добавить данные о выпущенной продукции завода /1");
                    Console.WriteLine("Поиск продукции по серийному номеру /2");
                    Console.WriteLine("Просмотреть статистику о сотруднике /3");
                    Console.WriteLine("Посмотреть статистику цеха /4");
                    Console.WriteLine("Настройки цвета /5");
                    Console.WriteLine("Выйти из аккаунта /6");
                    Console.WriteLine("Выйти из программы /0");
                    int menu_value = 0;
                    try
                    {
                        menu_value = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("\nНекорректный ввод. Попробуйте еще раз\n");
                        continue;
                    }
                    switch (menu_value)
                    {
                        case 1:
                            DllFunctionsCreate.create_product(superuser, workshop_id);
                            continue;
                        case 2:
                            DllInfo.get_info_about_product(superuser, workshop_id);
                            continue;
                        case 3:
                            DllInfo.get_info_about_worker(superuser, workshop_id);
                            continue;
                        case 4:
                            DllInfo.factory_statistics(superuser, workshop_id);
                            continue;
                        case 5:
                            DllColor.dllcolor();
                            continue;
                        case 6:
                            DllFunctionsCreate.add_defective_products(superuser, workshop_id);
                            continue;
                        case 7:
                            DllAuth.logout();
                            b = false;
                            c = false;
                            continue;
                        case 0:
                            b = false;
                            c = false;
                            x = true;
                            break;
                        default:
                            Console.WriteLine("\nНекорректный ввод. Попробуйте еще раз\n");
                            continue;
                    }
                }
            }
            return (false, x);
        }
    }
}
