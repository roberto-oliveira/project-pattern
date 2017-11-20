using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Linq.Expressions;

namespace Generics
{
    public abstract class GenericDao<TEntity, TDataContext> where TEntity : class where TDataContext : DataContext, new()
    {
        private static readonly TDataContext _dataContext = new TDataContext();
        private static readonly JavaScriptSerializer jss = new JavaScriptSerializer();

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void Insert(TEntity entity)
        {
            _dataContext.GetTable<TEntity>().InsertOnSubmit(entity);
            _dataContext.SubmitChanges();
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void Update(TEntity entity)
        {
            _dataContext.Refresh(RefreshMode.KeepCurrentValues, entity);
            _dataContext.SubmitChanges();
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public void Delete(TEntity entity)
        {
            _dataContext.GetTable<TEntity>().DeleteOnSubmit(entity);
            _dataContext.SubmitChanges();
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public void DeleteAttach(TEntity entity)
        {
            _dataContext.GetTable<TEntity>().Attach(entity);
            _dataContext.GetTable<TEntity>().DeleteOnSubmit(entity);
            _dataContext.SubmitChanges();
        }

        public IQueryable<TEntity> GetGeneric(Expression<Func<TEntity, bool>> lbdEx)
        {
            return _dataContext.GetTable<TEntity>().Where(lbdEx).AsQueryable();
        }

        public virtual string GetGenericReturnJson(Expression<Func<TEntity, bool>> lbdEx)
        {
            var query = _dataContext.GetTable<TEntity>().Where(lbdEx).AsQueryable();
            var json = jss.Serialize(query);

            return json;
        }

        public virtual string GetListEntitiesFormatJson(List<TEntity> listEntities)
        {
            var query = _dataContext.GetTable<TEntity>().ToList();
            var json = jss.Serialize(query);

            return json;
        }
    }
}
