using RestWithAspNet.Model;

namespace RestWithAspNet.Services.Implementations
{
    public class PersonServiceImplementation : IPersonService
    {
        private List<Person> persons = new List<Person>
            {
                new Person
                {
                    Id = 1,
                    FirstName = "Leandro",
                    LastName = "Costa",
                    Address = "Uberlândia - MG - Brasil",
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Lucas",
                    LastName = "Costa",
                    Address = "São Paulo - SP - Brasil",
                },
                new Person
                {
                    Id = 3,
                    FirstName = "Ana",
                    LastName = "Beatriz",
                    Address = "Rio de Janeiro - RJ - Brasil",
                },
                new Person
                {
                    Id = 4,
                    FirstName = "Juliana",
                    LastName = "Oliveira",
                    Address = "Belo Horizonte - MG - Brasil",
                }
            };

        public Person Create(Person person)
        {
            return person;
        }

        public void Delete(long id)
        {
        }

        public List<Person> FindAll()
        {
            return persons;
        }

        public Person? FindById(long id)
        {
            var person = persons.Find(p => p.Id == id);

            if (person != null)
            {
                return person;

            }

            return null;

        }

        public Person Update(Person person)
        {
            return person;
        }
    }
}