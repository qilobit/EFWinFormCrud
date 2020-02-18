using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //TestLINQ();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        static void TestLINQ() 
        {
            Console.WriteLine("---------------");
            Person[] people = {
                new Person("Juan", 40),
                new Person("Clint", 44),
                new Person("Clyde", 30),
                new Person("Pedro", 50),
                new Person("Miriam", 24),
                new Person("Jose", 16),
                new Person("Ernesto", 72),
            };

            (from person in people
            where person.age > 30
            select person)
            .ToList()
            .ForEach(p => Console.WriteLine($"Person: {p.name}"));


        }
    }

    class Person 
    {
        public Person(string name, int age) 
        {
            this.name = name;
            this.age = age;
        }
        public string name { get; set; }
        public int age { get; set; }
    }
}
