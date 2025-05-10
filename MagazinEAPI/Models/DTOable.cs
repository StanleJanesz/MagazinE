using System.Runtime.CompilerServices;

namespace MagazinEAPI.Models
{
    public interface DTOable<T> where T : class
    {
        T ToDTO();
    }
}
