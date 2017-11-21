using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using GenericMapper.dbml;
using RepositoryDao;

namespace MusicWeb
{
    public partial class Index : System.Web.UI.Page
    {
        private JavaScriptSerializer _jss = new JavaScriptSerializer();

        private List<Album> _albums = new List<Album>();
        private readonly Album _album = new Album();
        private readonly Guid _id = Guid.NewGuid();
        private readonly AlbumDao _albumDao = new AlbumDao();


        protected void Page_Load(object sender, EventArgs e)
        {
            GetListAlbums();

            var listaAlbums = new JavaScriptSerializer().Serialize(_albums);

            var jsonListEntitiesFormatJson = _albumDao.GetListEntitiesFormatJson(_albums);

            var json = _albumDao.GetGenericReturnJson(a => a.Nome.Contains("You"));

            //var teste = _albumDao.GetGeneric(a => a.Id == 1);

            gvReturnDataJsonFormat.DataSource = _jss.Deserialize<List<Album>>(jsonListEntitiesFormatJson);
            gvReturnDataJsonFormat.DataBind();

        }

        private void GetListAlbums()
        {
            _albums = _albumDao.GetGeneric(a => a.Nome.Contains("You")).ToList();

            this.gvAlbums.DataSource = _albums;
            this.gvAlbums.DataBind();
        }
    }
}
