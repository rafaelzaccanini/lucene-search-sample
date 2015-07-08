using System.Collections.Generic;
namespace Core.DomainModel
{
    public class MedicalSpecialty
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MedicalConsultory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int Number { get; set; }
        public MedicalSpecialty MedicalSpecialty { get; set; }
    }

    public class MedicalConsultoryRepository
    {
        public IList<MedicalConsultory> GetAll()
        {
            var card = new MedicalSpecialty() { Id = 1, Name = "Cardiologia" };
            var odon = new MedicalSpecialty() { Id = 2, Name = "Odontologia" };
            var pedi = new MedicalSpecialty() { Id = 3, Name = "Pediatria" };

            return new List<MedicalConsultory>() 
            {
                new MedicalConsultory(){Id = 1, Name = "Saúde Dental", City = "São Paulo", Address = "Rua César Castro", Number = 200, MedicalSpecialty = odon},
                new MedicalConsultory(){Id = 2, Name = "Saúde do Coração", City = "Rio de Janeiro", Address = "Av. Brasil", Number = 1560, MedicalSpecialty = card},
                new MedicalConsultory(){Id = 3, Name = "Baby Care", City = "São Paulo", Address = "Rua da Graça", Number = 100, MedicalSpecialty = pedi},
                new MedicalConsultory(){Id = 4, Name = "Clínica Infantil Cuidados Gerais", City = "Santa Catarina", Address = "Rua Castro Neves Torres", Number = 5220, MedicalSpecialty = pedi},
                new MedicalConsultory(){Id = 5, Name = "Saúde Conosco", City = "São Paulo", Address = "Rua Antonio Melo", Number = 1900, MedicalSpecialty = odon},
                new MedicalConsultory(){Id = 6, Name = "Cardiologia Brasil", City = "Minas Gerais", Address = "Av. Graça Foster", Number = 62, MedicalSpecialty = card},
                new MedicalConsultory(){Id = 7, Name = "Sorriso Feliz", City = "Acre", Address = "Av. Varela Castro", Number = 11, MedicalSpecialty = odon}
            };
        }
    }
}
