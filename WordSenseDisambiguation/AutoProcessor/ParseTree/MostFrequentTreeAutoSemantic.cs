using System;
using System.Collections.Generic;
using System.Globalization;
using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public class MostFrequentTreeAutoSemantic : TreeAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public MostFrequentTreeAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        private SynSet MostFrequent(List<SynSet> synSets, string root)
        {
            if (synSets.Count == 1)
            {
                return synSets[0];
            }

            var minSense = 50;
            SynSet best = null;
            foreach (var synSet in synSets)
            {
                for (int i = 0; i < synSet.GetSynonym().LiteralSize(); i++)
                {
                    if (synSet.GetSynonym().GetLiteral(i).GetName().ToLower(new CultureInfo("tr")).StartsWith(root)
                        || synSet.GetSynonym().GetLiteral(i).GetName().ToLower(new CultureInfo("tr")).EndsWith(" " + root))
                    {
                        if (synSet.GetSynonym().GetLiteral(i).GetSense() < minSense)
                        {
                            minSense = synSet.GetSynonym().GetLiteral(i).GetSense();
                            best = synSet;
                        }
                    }
                }
            }

            return best;
        }

        protected override bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree)
        {
            var nodeDrawableCollector =
                new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            for (int i = 0; i < leafList.Count; i++)
            {
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, leafList, i);
                if (synSets.Count > 0)
                {
                    var best = MostFrequent(synSets,
                        leafList[i].GetLayerInfo().GetMorphologicalParseAt(0).GetWord().GetName());
                    if (best != null)
                    {
                        leafList[i].GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS, best.GetId());
                    }
                }
            }

            return true;
        }
    }
}