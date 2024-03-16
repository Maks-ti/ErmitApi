
namespace ErmitApi.DAL.Models
{
    public interface IBaseEntity<IdType>
    {
        public IdType Id {  get; set; }
    }
}
