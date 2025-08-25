using RestWithAspNet.Model;

namespace RestWithAspNet.Services
{
    public interface IPersonService
    {
        public Person Create(Person person);

        public Person? FindById(long id);

        public List<Person> FindAll();

        public Person Update(Person person);

        public void Delete(long id);
    }
}