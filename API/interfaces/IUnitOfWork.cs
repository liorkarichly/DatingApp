using System.Threading.Tasks;

namespace API.interfaces
{
    public interface IUnitOfWork
    {
        
      IUserRepository UserRepository {get; }

      IMessageRepository MessageRepository { get; }

      ILikeRepository LikeRepository {get; }

      IPhotoRepository PhotoRepository {get; }

      Task<bool> Complete();

      bool HasChanges();//Use this one for to see if entity framework has been tracking or has any changes, we'll need to use that in one specific place.
         
    }
}