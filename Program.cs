using System;
using System.Xml.Linq;
//using DllFactory;
namespace factory_team5
{
    class Program
    {
        static void Main(string[] args)
        {
            DllColor.LoadColorSettings();
            bool b = true;
            while (b)
            {
                var data = DllAuth.login();
                Console.WriteLine(data);
                if (data.superuser)
                {
                    var result = DllMenu.superuser_panel(data.superuser, data.workshop_id);
                    if (result.Item2)
                    {
                        break;
                    }
                    if (result.Item1)
                    {
                        continue;
                    }
                }
                else
                {
                    var result = DllMenu.admin_panel(data.superuser, data.workshop_id);
                    if (result.Item2)
                    {
                        break;
                    }
                    if (result.Item1)
                    {
                        continue;
                    }
                }

            }
        }

    }
}
