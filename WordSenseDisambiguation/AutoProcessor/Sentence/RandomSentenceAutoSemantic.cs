using System;
using AnnotatedSentence;
using MorphologicalAnalysis;

namespace WordSenseDisambiguation.AutoProcessor.Sentence
{
    public class RandomSentenceAutoSemantic : SentenceAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public RandomSentenceAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        protected override bool AutoLabelSingleSemantics(AnnotatedSentence.AnnotatedSentence sentence)
        {
            var random = new Random(1);
            for (var i = 0; i < sentence.WordCount(); i++) {
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, sentence, i);
                if (synSets.Count > 0){
                    ((AnnotatedWord) sentence.GetWord(i)).SetSemantic(synSets[random.Next(synSets.Count)].GetId());
                }
            }
            return true;
        }
    }
}