using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.IO;

namespace Core
{
    public class IndexBase
    {
        public FSDirectory _indexDirectoryPath = FSDirectory.Open(new DirectoryInfo(Configs.IndexPath));

        public void CreateIndex()
        {
            if (!IndexExists())
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                var indexWriter = new IndexWriter(_indexDirectoryPath, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                indexWriter.Dispose();
            }
        }

        private bool IndexExists()
        {
            return IndexReader.IndexExists(_indexDirectoryPath);
        }
    }
}
