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
    public class Lesk : TreeAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public Lesk(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        private int Intersection(SynSet synSet, List<ParseNodeDrawable> leafList){
            string[] words1;
            if (synSet.GetExample() != null){
                words1 = (synSet.GetLongDefinition() + " " + synSet.GetExample()).Split(" ");
            } else {
                words1 = synSet.GetLongDefinition().Split(" ");
            }
            var words2 = new string[leafList.Count];
            for (var i = 0; i < leafList.Count; i++){
                words2[i] = leafList[i].GetLayerData(ViewLayerType.TURKISH_WORD);
            }
            var count = 0;
            foreach (var word1 in words1){
                foreach (var word2 in words2){
                    if (word1.ToLower(new CultureInfo("tr")).Equals(word2.ToLower(new CultureInfo("tr")))){
                        count++;
                    }
                }
            }
            return count;
        }

        protected override bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree)
        {
            var random = new Random(1);
            var nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            var done = false;
            for (var i = 0; i < leafList.Count; i++){
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, leafList, i);
                var maxIntersection = -1;
                for (var j = 0; j < synSets.Count; j++){
                    var synSet = synSets[j];
                    var intersectionCount = Intersection(synSet,leafList);
                    if (intersectionCount > maxIntersection){
                        maxIntersection = intersectionCount;
                    }
                }
                var maxSynSets = new List<SynSet>();
                for (var j = 0; j < synSets.Count; j++){
                    var synSet = synSets[j];
                    if (Intersection(synSet,leafList) == maxIntersection){
                        maxSynSets.Add(synSet);
                    }
                }
                if (maxSynSets.Count > 0){
                    leafList[i].GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS, maxSynSets[random.Next(maxSynSets.Count)].GetId());
                    done = true;
                }
            }
            return done;
        }
    }
}