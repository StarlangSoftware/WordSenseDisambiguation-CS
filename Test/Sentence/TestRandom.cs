using AnnotatedSentence;
using MorphologicalAnalysis;
using NUnit.Framework;
using WordSenseDisambiguation.AutoProcessor.Sentence;


namespace Test.Sentence
{
    public class TestRandom
    {
        FsmMorphologicalAnalyzer fsm;
        WordNet.WordNet wordNet;

        [SetUp]
        public void Setup()
        {
            fsm = new FsmMorphologicalAnalyzer();
            wordNet = new WordNet.WordNet();
        }

        [Test]
        public void TestAccuracy() {
            int correct = 0, total = 0;
            RandomSentenceAutoSemantic random = new RandomSentenceAutoSemantic(wordNet, fsm);
            AnnotatedCorpus corpus1 = new AnnotatedCorpus("../../../new-sentences");
            AnnotatedCorpus corpus2 = new AnnotatedCorpus("../../../old-sentences");
            for (int i = 0; i < corpus1.SentenceCount(); i++){
                var sentence1 = (AnnotatedSentence.AnnotatedSentence) corpus1.GetSentence(i);
                random.AutoSemantic(sentence1);
                var sentence2 = (AnnotatedSentence.AnnotatedSentence) corpus2.GetSentence(i);
                for (int j = 0; j < sentence1.WordCount(); j++){
                    total++;
                    AnnotatedWord word1 = (AnnotatedWord) sentence1.GetWord(j);
                    AnnotatedWord word2 = (AnnotatedWord) sentence2.GetWord(j);
                    if (word1.GetSemantic() != null && word1.GetSemantic().Equals(word2.GetSemantic())){
                        correct++;
                    }
                }
            }
            Assert.AreEqual(549, total);
            Assert.AreEqual(270, correct);
        }

    }
}