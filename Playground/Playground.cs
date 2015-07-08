using System;
using Core.DomainIndex;
using Core.DomainModel;
using System.Collections.Generic;

namespace Playground
{
    public class Playground
    {
        public static void Main()
        {
            var index = new MedicalIndex(true);

            var results = index.Search("Clinica de Odontologia");
            PrintAll(results);

            results = index.Search("Consultórios de Cardiologia");
            PrintAll(results);

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
