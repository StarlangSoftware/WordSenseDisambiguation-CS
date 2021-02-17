using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using MorphologicalAnalysis;
using NUnit.Framework;
using WordSenseDisambiguation.AutoProcessor.ParseTree;

namespace Test
{
    public class TestMostFrequent
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
        public void TestAccuracy()
        {
            int correct = 0, total = 0;
            var mostFrequentTreeAutoSemantic = new MostFrequentTreeAutoSemantic(wordNet, fsm);
            var treeBank1 = new TreeBankDrawable("../../../new-trees/");
            var treeBank2 = new TreeBankDrawable("../../../old-trees/");
            for (var i = 0; i < treeBank1.Size(); i++)
            {
                var parseTree1 = treeBank1.Get(i);
                var parseTree2 = treeBank2.Get(i);
                mostFrequentTreeAutoSemantic.AutoSemantic(parseTree1);
                var nodeDrawableCollector1 =
                    new NodeDrawableCollector((ParseNodeDrawable) parseTree1.GetRoot(), new IsTurkishLeafNode());
                var leafList1 = nodeDrawableCollector1.Collect();
                var nodeDrawableCollector2 =
                    new NodeDrawableCollector((ParseNodeDrawable) parseTree2.GetRoot(), new IsTurkishLeafNode());
                var leafList2 = nodeDrawableCollector2.Collect();
                for (var j = 0; j < leafList1.Count; j++)
                {
                    total++;
                    var parseNode1 = leafList1[j];
                    var parseNode2 = leafList2[j];
                    if (parseNode1.GetLayerData(ViewLayerType.SEMANTICS) != null && parseNode1
                        .GetLayerData(ViewLayerType.SEMANTICS).Equals(parseNode2.GetLayerData(ViewLayerType.SEMANTICS)))
                    {
                        correct++;
                    }
                }
            }

            Assert.AreEqual(475, total);
            Assert.AreEqual(260, correct);
        }

    }
}