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
        }

        private void GetListAlbums()
        {
            _albums = _albumDao.GetList();
            this.gvAlbums.DataSource = _albums;
            this.gvAlbums.DataBind();
        }
    }
}