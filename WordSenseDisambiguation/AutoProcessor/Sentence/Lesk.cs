using System;
using System.Collections.Generic;
using System.Globalization;
using AnnotatedSentence;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.Sentence
{
    public class Lesk: SentenceAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public Lesk(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }
        
        private int Intersection(SynSet synSet, AnnotatedSentence.AnnotatedSentence sentence){
            string[] words1;
            if (synSet.GetExample() != null){
                words1 = (synSet.GetLongDefinition() + " " + synSet.GetExample()).Split(" ");
            } else {
                words1 = synSet.GetLongDefinition().Split(" ");
            }
            var words2 = sentence.ToWords().Split(" ");
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

        protected override bool AutoLabelSingleSemantics(AnnotatedSentence.AnnotatedSentence sentence)
        {
            var random = new Random(1);
            var done = false;
            for (var i = 0; i < sentence.WordCount(); i++) {
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, sentence, i);
                var maxIntersection = -1;
                for (var j = 0; j < synSets.Count; j++){
                    var synSet = synSets[j];
                    var intersectionCount = Intersection(synSet, sentence);
                    if (intersectionCount > maxIntersection){
                        maxIntersection = intersectionCount;
                    }
                }
                var maxSynSets = new List<SynSet>();
                for (var j = 0; j < synSets.Count; j++){
                    var synSet = synSets[j];
                    if (Intersection(synSet, sentence) == maxIntersection){
                        maxSynSets.Add(synSet);
                    }
                }
                if (maxSynSets.Count > 0){
                    done = true;
                    ((AnnotatedWord) sentence.GetWord(i)).SetSemantic(maxSynSets[random.Next(maxSynSets.Count)].GetId());
                }
            }
            return done;
        }
    }
}