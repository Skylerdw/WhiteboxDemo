using System.Collections.Generic;

namespace ConsoleApp1
{
    public interface IDogRepositry
    {
        List<Dog> GetDogs(DogType dogType);
        int GetDogsCount(DogType dogType);
    }
}