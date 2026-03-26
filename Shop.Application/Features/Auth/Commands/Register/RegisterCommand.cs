using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;
using Shop.Domain.Entities;

namespace Shop.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<Guid>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            throw new Exception("Email này đã được sử dụng!"); 
        }
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        var user = new User()
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            Role = Roles.User
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}