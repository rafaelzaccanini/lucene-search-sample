using System;
using Core;
using Core.DomainModel;
using System.Collections.Generic;

namespace Playground
{
    public class Playground
    {
        public static void Main()
        {
            var index = new MedicalSearch();

            index.UpdateIndexs();

            // Perfect search
            //var search = "Odontologia no Acre";
            //PrintAll(index.Search(search));
            //Console.Read();

            // Incorrect search using spellchecker
            var search = "Odonttoloia no Accre";
            Console.WriteLine("You've written: " + search);
            
            search = index.GetSugestion(search);
            Console.WriteLine("Did you mean: " + search);
            Console.WriteLine("");
            
            PrintAll(index.Search(search));
            Console.Read();
            
        }

        public static void PrintAll(IList<MedicalConsultory> results)
        {
            foreach (var item in results)
            {
                Console.WriteLine("Id: " + item.Id);
                Console.WriteLine("Name: " + item.Name);
                Console.WriteLine("City: " + item.City);
                Console.WriteLine("Address: " + item.Address);
                Console.WriteLine("Number: " + item.Number);
                Console.WriteLine("Specialty Id: " + item.MedicalSpecialty.Id);
                Console.WriteLine("Specialty Name: " + item.MedicalSpecialty.Name);

                Console.WriteLine("");
            }
        }
    }
}
