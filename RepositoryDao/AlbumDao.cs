using GenericBase;
using GenericMapper.dbml;
using System.Data.Linq;
using System.Collections.Generic;

namespace RepositoryDao
{
    public class AlbumDao : GenericDao<Album, MusicDataContext> {}
}
