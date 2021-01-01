using System.Collections.Generic;
using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public class TurkishTreeAutoSemantic : TreeAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public TurkishTreeAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        protected override bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree)
        {
            var modified = false;
            var nodeDrawableCollector =
                new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            foreach (var parseNode in leafList)
            {
                var info = parseNode.GetLayerInfo();
                if (info.GetLayerData(ViewLayerType.INFLECTIONAL_GROUP) != null)
                {
                    var meanings = new List<SynSet>[info.GetNumberOfWords()];
                    for (var i = 0; i < info.GetNumberOfWords(); i++)
                    {
                        meanings[i] = _turkishWordNet.ConstructSynSets(
                            info.GetMorphologicalParseAt(i).GetWord().GetName(), info.GetMorphologicalParseAt(i),
                            info.GetMetamorphicParseAt(i), _fsm);
                    }

                    switch (info.GetNumberOfWords())
                    {
                        case 1:
                            if (meanings[0].Count == 1)
                            {
                                modified = true;
                                parseNode.GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS,
                                    meanings[0][0].GetId());
                            }

                            break;
                        case 2:
                            if (meanings[0].Count == 1 && meanings[1].Count == 1)
                            {
                                modified = true;
                                parseNode.GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS,
                                    meanings[0][0].GetId() + "$" + meanings[1][0].GetId());
                            }

                            break;
                        case 3:
                            if (meanings[0].Count == 1 && meanings[1].Count == 1 && meanings[2].Count == 1)
                            {
                                modified = true;
                                parseNode.GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS,
                                    meanings[0][0].GetId() + "$" + meanings[1][0].GetId() + "$" +
                                    meanings[2][0].GetId());
                            }

                            break;
                    }
                }
            }

            return modified;
        }
    }
}