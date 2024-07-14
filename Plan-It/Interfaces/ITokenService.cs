
using Plan_It.Models;

namespace Plan_It.Interfaces{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}