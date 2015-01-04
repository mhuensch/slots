using System;
using System.Linq;

namespace Foogenda.Framework
{
	public interface IRestApiController<TEntity>
	{
		IQueryable<TEntity> Get(string args = null);

		TEntity Get(Guid id);

		TEntity Post(TEntity value);

		TEntity Put(Guid id, TEntity value);

		void Delete(Guid id);
	}
}
