using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Models;
using ElectricityShop.Domain.Interfaces;
using MediatR;

namespace ElectricityShop.Application.Features.Users.Commands
{
    public class UpdateProfileCommand : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

                if (user == null)
                {
                    throw new Exception($"User with id {request.UserId} not found.");
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();

                return new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString()
                };
            }
        }
    }
}