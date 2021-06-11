using System;
using System.Linq;

namespace ConsoleApp1
{
    public class DogShelterService
    {
        private readonly INotificationClient _notificationClient;
        private readonly IDogRepositry _dogRepositry;

        public DogShelterService(INotificationClient notificationClient, IDogRepositry dogRepositry)
        {
            _notificationClient = notificationClient;
            _dogRepositry = dogRepositry;
        }

        public int GetDogsNumber(DogType dogType)
        {
            _notificationClient.SendNotification($"Someone is interested in {dogType} dogs!");
            var numberOfDogs = _dogRepositry.GetDogsCount(dogType);
            if (numberOfDogs <= 0)
            {
                _notificationClient.SendNotification($"We have no {dogType} dogs left!!!");
            }

            return numberOfDogs;
        }
        
        public Dog GetOldestDog(DogType dogType)
        {
            Dog oldestDog = null;
            if (_dogRepositry.GetDogsCount(dogType) > 0)
            {
                var dogsOfType = _dogRepositry.GetDogs(dogType);
                oldestDog = dogsOfType.OrderByDescending(d => d.Age).FirstOrDefault();
            }
            
            return oldestDog;
        }
        
        public void FeedDogs(DogType dogType)
        {
            if (_dogRepositry.GetDogsCount(dogType) > 0)
            {
                var dogsOfType = _dogRepositry.GetDogs(dogType);
                foreach (var dog in dogsOfType)
                {
                    _notificationClient.SendNotification($"Another {dogType} has been fed!");
                }
            }
        }
    }
}