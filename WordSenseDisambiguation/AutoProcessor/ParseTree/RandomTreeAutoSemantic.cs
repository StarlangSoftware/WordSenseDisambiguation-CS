using System;
using System.Collections.Generic;
using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public class RandomTreeAutoSemantic : TreeAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public RandomTreeAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        protected override bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree)
        {
            var random = new Random(1);
            var nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            for (var i = 0; i < leafList.Count; i++){
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, leafList, i);
                if (synSets.Count > 0){
                    leafList[i].GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS, synSets[random.Next(synSets.Count)].GetId());
                }
            }
            return true;
        }
    }
}