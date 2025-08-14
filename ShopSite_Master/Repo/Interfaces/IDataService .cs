namespace MyShopSite.Repo.Interfaces
{
    public interface IDataService
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<T> GetByIdAsync<T>(object key) where T : class;
        Task<T> CreateAsync<T>(T entity) where T : class;
        Task<T> UpdateAsync<T>(T entity) where T : class;
        Task<bool> DeleteAsync<T>(object key) where T : class;
        IQueryable<T> Query<T>() where T : class;
        IQueryable<T> GetQuery<T>() where T : class;
        Task AddAsync<T>(T entity) where T : class;

        Task SaveAsync();
    }
}
