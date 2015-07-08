using Core.DomainModel;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System.Collections.Generic;
using System.Linq;

namespace Core.DomainIndex
{
    public class MedicalIndex : IndexBase
    {
        private MedicalConsultoryRepository _repository;

        public MedicalIndex() : this(false) { }

        public MedicalIndex(bool updateIndexs) 
        {
            CreateIndex();
            _repository = new MedicalConsultoryRepository();

            if(updateIndexs)
                UpdateIndexs();
        }

        public void UpdateIndexs()
        {
            foreach (var item in _repository.GetAll())
                UpdateIndex(item);
        }

        private void UpdateIndex(MedicalConsultory item)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_indexDirectoryPath, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var searchQuery = new TermQuery(new Term("Id", item.Id.ToString()));
                writer.DeleteDocuments(searchQuery);

                var doc = new Document();

                doc.Add(new Field("Id", item.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Name", item.Name, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("City", item.City, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Address", item.Address, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Number", item.Number.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("SpecialtyId", item.MedicalSpecialty.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("SpecialtyName", item.MedicalSpecialty.Name, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);

                analyzer.Close();
                writer.Dispose();
            }
        }

        public IList<MedicalConsultory> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return new List<MedicalConsultory>();

            using (var searcher = new IndexSearcher(_indexDirectoryPath, false))
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                var finalQuery = new BooleanQuery();
                
                //var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, new[] { "SpecialtyName", "City" }, analyzer);
                //string[] terms = searchText.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                //foreach (string term in terms)
                //    finalQuery.Add(parser.Parse(term), Occur.SHOULD);

                var parser = new QueryParser(Version.LUCENE_30, "SpecialtyName", analyzer);
                var query = parser.Parse(searchText.Trim());
                
                var hits = searcher.Search(query, searcher.MaxDoc).ScoreDocs;
                //var hits = searcher.Search(finalQuery, null, hits_limit, Sort.INDEXORDER).ScoreDocs;

                var results = hits.Select(hit => MapMedicalConsultory(hit, searcher)).ToList();
                
                analyzer.Close();
                searcher.Dispose();
                
                return results;
            }
        }

        private MedicalConsultory MapMedicalConsultory(ScoreDoc hit, IndexSearcher searcher)
        {
            var doc = searcher.Doc(hit.Doc);

            return new MedicalConsultory
            {
                Id = System.Convert.ToInt32(doc.Get("Id")),
                Name = doc.Get("Name"),
                City = doc.Get("City"),
                Address = doc.Get("Address"),
                Number = System.Convert.ToInt32(doc.Get("Number")),
                MedicalSpecialty = new MedicalSpecialty
                {
                    Id = System.Convert.ToInt32(doc.Get("SpecialtyId")),
                    Name = doc.Get("SpecialtyName"),
                }
            };
        }

        
    }
}
