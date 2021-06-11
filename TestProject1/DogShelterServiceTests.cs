using System.Collections.Generic;
using ConsoleApp1;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace TestProject1
{
    public class DogShelterServiceTests
    {
        private Mock<IDogRepositry> _dogRepoMock;
        private Mock<INotificationClient> _notificationMock;
        private DogShelterService _dogShelterService;

        [SetUp]
        public void Setup()
        {
            _dogRepoMock = new Mock<IDogRepositry>();
            _notificationMock = new Mock<INotificationClient>();
            _dogShelterService = new DogShelterService(_notificationMock.Object, _dogRepoMock.Object);
        }

        [Test]
        [TestCase(DogType.Bulldog, 5133)]
        [TestCase(DogType.Lablador, 1)]
        [TestCase(DogType.Crossbreed, 0)]
        [TestCase(DogType.Poodle, 0)]
        public void GetDogsNumber_Success(DogType dogType, int numberOfDogsExpected)
        {
            //assign
            var dogsToReturnFromRepo = new List<Dog>();
            _dogRepoMock.Setup(r => r.GetDogsCount(dogType)).Returns(numberOfDogsExpected);
            var shouldSecondNotificationFire = numberOfDogsExpected <= 0;
            
            //act
            var result = _dogShelterService.GetDogsNumber(dogType);
            
            //assert
            result.Should().Be(numberOfDogsExpected);
            
            /* this is the whitebox part*/
            _dogRepoMock.Verify(r => r.GetDogsCount(dogType), Times.Once);
            _notificationMock.Verify(r => 
                r.SendNotification($"Someone is interested in {dogType} dogs!"), Times.Once);
            _notificationMock.Verify(r => 
                    r.SendNotification($"We have no {dogType} dogs left!!!"), shouldSecondNotificationFire ? Times.Once : Times.Never);
        }
        
        [Test]
        [TestCase(DogType.Bulldog, 999)]
        [TestCase(DogType.Lablador, 1)]
        [TestCase(DogType.Lablador, 0)]
        public void GetOldestDog_DogsAvaialble_Success(DogType dogType, int oldestAge)
        {
            //assign
            var dogsToReturnFromRepo = new List<Dog>();
            for (int i = 0; i <= oldestAge; i++)
            {
                dogsToReturnFromRepo.Add(new Dog() { Age = i });
            }
            _dogRepoMock.Setup(r => r.GetDogs(dogType)).Returns(dogsToReturnFromRepo);
            _dogRepoMock.Setup(r => r.GetDogsCount(dogType)).Returns(oldestAge+1);
            
            //act
            var result = _dogShelterService.GetOldestDog(dogType);
            
            //assert
            result?.Age.Should().Be(oldestAge);

            /* this is the whitebox part*/
            _dogRepoMock.Verify(r => r.GetDogsCount(dogType), Times.Once);
            _dogRepoMock.Verify(r => r.GetDogs(dogType), Times.Once);
        }
        
        [Test]
        [TestCase(DogType.Bulldog)]
        [TestCase(DogType.Lablador)]
        public void GetOldestDog_DogsNotAvaialble_Success(DogType dogType)
        {
            //assign
            var dogsToReturnFromRepo = new List<Dog>();
            _dogRepoMock.Setup(r => r.GetDogs(dogType)).Returns(dogsToReturnFromRepo);
            
            //act
            var result = _dogShelterService.GetOldestDog(dogType);
            
            //assert
            result.Should().Be(null);

            /* this is the whitebox part*/
            _dogRepoMock.Verify(r => r.GetDogsCount(dogType), Times.Once);
            _dogRepoMock.Verify(r => r.GetDogs(dogType), Times.Never);
        }
    }
}