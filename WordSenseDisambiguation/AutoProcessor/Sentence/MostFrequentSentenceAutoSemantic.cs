using System.Collections.Generic;
using System.Globalization;
using AnnotatedSentence;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.Sentence
{
    public class MostFrequentSentenceAutoSemantic : SentenceAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        public MostFrequentSentenceAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
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

        protected override bool AutoLabelSingleSemantics(AnnotatedSentence.AnnotatedSentence sentence)
        {
            for (var i = 0; i < sentence.WordCount(); i++) {
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, sentence, i);
                if (synSets.Count > 0){
                    var best = MostFrequent(synSets, ((AnnotatedWord) sentence.GetWord(i)).GetParse().GetWord().GetName());
                    if (best != null){
                        ((AnnotatedWord) sentence.GetWord(i)).SetSemantic(best.GetId());
                    }
                }
            }
            return true;
        }
    }
}