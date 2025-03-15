using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Models;
using ElectricityShop.Domain.Interfaces;
using MediatR;

namespace ElectricityShop.Application.Features.Users.Queries
{
    public class GetUserProfileQuery : IRequest<UserDto>
    {
        public Guid UserId { get; set; }

        public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetUserProfileQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

                if (user == null)
                {
                    throw new Exception($"User with id {request.UserId} not found.");
                }

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