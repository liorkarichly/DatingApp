using System.Threading.Tasks;
using API.interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext r_DataContext;
        private readonly IMapper r_Mapper;

        public UnitOfWork(DataContext i_DataContext, IMapper i_Mapper)
        {

            this.r_DataContext = i_DataContext;
            this.r_Mapper = i_Mapper;

        }

        public IUserRepository UserRepository => new UserRepository(r_DataContext,r_Mapper);

        public IMessageRepository MessageRepository => new MessageRepository(r_DataContext, r_Mapper);

        public ILikeRepository LikeRepository => new LikesRepository(r_DataContext);

        public async Task<bool> Complete()
        {
            
            return await r_DataContext.SaveChangesAsync() > 0;

        }

        public bool HasChanges()
        {
            
            return r_DataContext.ChangeTracker.HasChanges();/* Save changes on tracker about every entites that found in tracker*/
        }
    }
}