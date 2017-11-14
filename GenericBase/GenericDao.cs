using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GenericBase
{
    public abstract class GenericDao<TEntity, TDataContext>
        where TEntity : class where TDataContext : DataContext, new()
    {
        private readonly TDataContext _dataContext = new TDataContext();

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

        public virtual ITable GetClass()
        {
            return _dataContext.GetTable<TEntity>();
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public IQueryable<TEntity> GetTable()
        {
            return _dataContext.GetTable<TEntity>();
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public IList<TEntity> GetList()
        {
            return _dataContext.GetTable<TEntity>().ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public virtual List<TEntity> GetNormalList()
        {
            return _dataContext.GetTable<TEntity>().ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<TEntity> GetNormalListWithParameters(int mes, int ano)
        {
            return _dataContext.GetTable<TEntity>().ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public IEnumerable<TEntity> GetListIEnumerable()
        {
            return _dataContext.GetTable<TEntity>().ToList();
        }


        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public TEntity SearchById(int id)
        {
            var parametro = Expression.Parameter(typeof(TEntity), "item");

            var expressao = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Property(parametro,
                    typeof(TEntity)
                        .GetPrimaryKey().Name),
                Expression.Constant(id)), parametro);

            var item = GetTable().Where(expressao).SingleOrDefault();

            return item;
        }
        // Func recebe a entidade/classe (ex produto)
        // a ser usada na pesquisa, portanto, é dinâmica

        //public IQueryable<TEntity> Get(Func<TEntity, bool> predicate)
        //{
        //    //TEntity = é uma classe, ex Produtos, Clientes
        //    // predicate = é a expressão de filtro
        //    // p => p.Preco > 10
        //    // AsQueryable = converte para uma lista consultável
        //    // .Set<> referencia a entidade dinamicamente
        //    return ctx.Set<TEntity>().Where(predicate).AsQueryable();
        //}

        public IQueryable<TEntity> ConsultaGenerica(Func<TEntity, bool> expressionLambda)
        {
            return _dataContext.GetTable<TEntity>().Where(expressionLambda).AsQueryable();
        }

    }

    public static class Auxiliar
    {
        public static PropertyInfo GetPrimaryKey(this Type entityType)
        {
            foreach (var property in entityType.GetProperties().Where(property => property.IsPrimaryKey()))
            {
                if (property.PropertyType != typeof(int))
                {
                    throw new Exception($"Primary key, '{property.Name}', do tipo '{entityType}' não é int");
                }
                return property;
            }
            throw new ApplicationException($"Sem primary key definida para o tipo '{entityType.Name}'");
        }

        public static bool IsPrimaryKey(this PropertyInfo propertyInfo)
        {
            var columnAttribute = propertyInfo.GetAttributeOf<ColumnAttribute>();

            return columnAttribute != null && columnAttribute.IsPrimaryKey;
        }

        public static T GetAttributeOf<T>(this PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(T), true);
            if (attributes.Length == 0)
            {
                return default(T);
            }
            return (T)attributes[0];
        }

        [Serializable]
        public class PrimaryKeyNotFoundException : Exception
        {
            public PrimaryKeyNotFoundException() { }

            public PrimaryKeyNotFoundException(string message) : base(message) { }

            public PrimaryKeyNotFoundException(string message, Exception innerException) : base(message, innerException) { }

            protected PrimaryKeyNotFoundException(SerializationInfo info, StreamingContext streamingContext) : base(info, streamingContext) { }
        }

        public static void ClearControls(Control controle)
        {
            foreach (Control ctr in controle.Controls)
            {
                if (ctr.HasControls())
                {
                    ClearControls(ctr);
                }

                if (ctr is TextBox)
                {
                    var tb = (TextBox)ctr;
                    tb.Text = "";
                }

                if (ctr is DropDownList)
                {
                    var ddl = (DropDownList)ctr;
                    ddl.DataBind();
                }

                if (ctr is HtmlInputText)
                {
                    var hit = (HtmlInputText)ctr;
                    hit.Value = "";
                }
            }
        }

    }
}
