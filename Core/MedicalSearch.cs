using Core.DomainModel;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core
{
    public class MedicalSearch
    {
        #region constructor

        private MedicalConsultoryRepository _repository;
        private Version _version;
        private FSDirectory _indexDirectoryPath;

        public MedicalSearch() 
        {
            _repository = new MedicalConsultoryRepository();
            _version = Version.LUCENE_30;
            _indexDirectoryPath = FSDirectory.Open(new DirectoryInfo(@"C:\LuceneIndex"));
        }

        #endregion

        public string GetSuggestion(string searchText)
        {
            string result = string.Empty;

            using (var reader = IndexReader.Open(_indexDirectoryPath, true))
            {
                var spellchecker = new SpellChecker.Net.Search.Spell.SpellChecker(_indexDirectoryPath);

                spellchecker.IndexDictionary(new SpellChecker.Net.Search.Spell.LuceneDictionary(reader, "SpecialtyName"));
                //spellchecker.IndexDictionary(new SpellChecker.Net.Search.Spell.LuceneDictionary(reader, "City"));

                searchText.Split(' ')
                          .ToList()
                          .ForEach(word =>
                          {
                              result += (spellchecker.SuggestSimilar(word, 1).FirstOrDefault() ?? word) + " ";
                          });
            }

            return result.Trim();
        }

        public IList<MedicalConsultory> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return new List<MedicalConsultory>();

            using (var searcher = new IndexSearcher(_indexDirectoryPath, true))
            {
                var analyzer = new StandardAnalyzer(_version);

                // Multi field search
                var query = new BooleanQuery();
                var parser = new MultiFieldQueryParser(_version, new[] { "SpecialtyName", "City" }, analyzer);
                
                searchText.RemoveIrrelevantTerms()
                          .Split(' ')
                          .ToList()
                          .ForEach(word =>
                          {
                              query.Add(parser.Parse(word), Occur.SHOULD);
                          });
                var hits = searcher.Search(query, null, searcher.MaxDoc, Sort.RELEVANCE).ScoreDocs;

                // Simple field search
                //var parser = new QueryParser(Version.LUCENE_30, "SpecialtyName", analyzer);
                //var query = parser.Parse(searchText.Trim());
                //var hits = searcher.Search(query, searcher.MaxDoc).ScoreDocs;

                var results = hits.Select(hit => MapMedicalConsultory(hit, searcher)).ToList();
                
                analyzer.Close();
                searcher.Dispose();
                
                return results;
            }
        }

        public void UpdateIndexs()
        {
            foreach (var item in _repository.GetAll())
                UpdateIndex(item);
        }


        private void UpdateIndex(MedicalConsultory item)
        {
            var analyzer = new StandardAnalyzer(_version);
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
