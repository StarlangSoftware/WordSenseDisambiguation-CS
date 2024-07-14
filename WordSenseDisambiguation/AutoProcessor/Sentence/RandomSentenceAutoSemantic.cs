using System;
using AnnotatedSentence;
using MorphologicalAnalysis;

namespace WordSenseDisambiguation.AutoProcessor.Sentence
{
    public class RandomSentenceAutoSemantic : SentenceAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        /// <summary>
        /// Constructor for the {@link RandomSentenceAutoSemantic} class. Gets the Turkish wordnet and Turkish fst based
        /// morphological analyzer from the user and sets the corresponding attributes.
        /// </summary>
        /// <param name="turkishWordNet">Turkish wordnet</param>
        /// <param name="fsm">Turkish morphological analyzer</param>
        public RandomSentenceAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        /// <summary>
        /// The method annotates the word senses of the words in the sentence randomly. The algorithm processes target
        /// words one by one. First, the algorithm constructs an array of all possible senses for the target word to
        /// annotate. Then it chooses a sense randomly.
        /// </summary>
        /// <param name="sentence">Sentence to be annotated.</param>
        /// <returns>True.</returns>
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