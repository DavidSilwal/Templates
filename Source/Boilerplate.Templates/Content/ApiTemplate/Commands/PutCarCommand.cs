namespace ApiTemplate.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using ApiTemplate.Repositories;
    using ApiTemplate.ViewModels;
    using Boilerplate.Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class PutCarCommand : IPutCarCommand
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICarRepository carRepository;
        private readonly IMapper<Models.Car, Car> carToCarMapper;
        private readonly IMapper<SaveCar, Models.Car> saveCarToCarMapper;

        public PutCarCommand(
            IHttpContextAccessor httpContextAccessor,
            ICarRepository carRepository,
            IMapper<Models.Car, Car> carToCarMapper,
            IMapper<SaveCar, Models.Car> saveCarToCarMapper)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.carRepository = carRepository;
            this.carToCarMapper = carToCarMapper;
            this.saveCarToCarMapper = saveCarToCarMapper;
        }

        public async Task<IActionResult> ExecuteAsync(int carId, SaveCar saveCar, CancellationToken cancellationToken)
        {
            var car = await this.carRepository.Get(carId, cancellationToken);
            if (car == null)
            {
                return new NotFoundResult();
            }

            if (car.HasPreconditionFailed(this.httpContextAccessor.HttpContext.Request))
            {
                return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
            }

            this.saveCarToCarMapper.Map(saveCar, car);
            car = await this.carRepository.Update(car, cancellationToken);
            var carViewModel = this.carToCarMapper.Map(car);

            return new OkObjectResult(carViewModel);
        }
    }
}
