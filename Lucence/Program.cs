using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

using LuceneDirectory = Lucene.Net.Store.RAMDirectory;

const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

//Open the Directory using a Lucene Directory class
string indexName = "example_index";
string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);

using LuceneDirectory indexDir = new RAMDirectory();

// Create an analyzer to process the text 
Analyzer standardAnalyzer = new RussianAnalyzer(luceneVersion);

//Create an index writer
IndexWriterConfig indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer);
indexConfig.OpenMode = OpenMode.CREATE;                             // create/overwrite index
using IndexWriter writer = new IndexWriter(indexDir, indexConfig);

//Add three documents to the index
var doc = new Document
{
    new TextField("title", "Этот текст очень важен для нас всех", Field.Store.YES),
    new StringField("domain", "www.apache.org", Field.Store.YES)
};
writer.AddDocument(doc);

doc = new Document
{
    new TextField("title", "Привет, пишу этот текст из Америки", Field.Store.YES),
    new StringField("domain", "lucenenet.apache.org", Field.Store.YES)
};
writer.AddDocument(doc);

doc = new Document
{
    new TextField("title", "Как дела мазафака, тексты нынче тяжело даютяс", Field.Store.YES),
    new StringField("domain", "www.giftoasis.com", Field.Store.YES)
};
writer.AddDocument(doc);

//Flush and commit the index data to the directory
writer.Commit();

using DirectoryReader reader = writer.GetReader(applyAllDeletes: true);
IndexSearcher searcher = new IndexSearcher(reader);

QueryParser parser = new QueryParser(luceneVersion, "title", standardAnalyzer);
Query query = parser.Parse("привет");
TopDocs topDocs = searcher.Search(query, n: 3);         //indicate we want the first 3 results


Console.WriteLine($"Matching results: {topDocs.TotalHits}");

for (int i = 0; i < topDocs.TotalHits; i++)
{
    //read back a doc from results
    var resultDoc = searcher.Doc(topDocs.ScoreDocs[i].Doc);

    string domain = resultDoc.Get("domain");
    Console.WriteLine($"Domain of result {i + 1}: {domain}");
}