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

        /// <summary>
        /// Constructor for the {@link Lesk} class. Gets the Turkish wordnet and Turkish fst based
        /// morphological analyzer from the user and sets the corresponding attributes.
        /// </summary>
        /// <param name="turkishWordNet">Turkish wordnet</param>
        /// <param name="fsm">Turkish morphological analyzer</param>
        public Lesk(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }
        
        /// <summary>
        /// Calculates the number of words that occur (i) in the definition or example of the given synset and (ii) in the
        /// given sentence.
        /// </summary>
        /// <param name="synSet">Synset of which the definition or example will be checked</param>
        /// <param name="sentence">Sentence to be annotated.</param>
        /// <returns>The number of words that occur (i) in the definition or example of the given synset and (ii) in the given
        /// sentence.</returns>
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

        /// <summary>
        /// The method annotates the word senses of the words in the sentence according to the simplified Lesk algorithm.
        /// Lesk is an algorithm that chooses the sense whose definition or example shares the most words with the target
        /// word’s neighborhood. The algorithm processes target words one by one. First, the algorithm constructs an array of
        /// all possible senses for the target word to annotate. Then for each possible sense, the number of words shared
        /// between the definition of sense synset and target sentence is calculated. Then the sense with the maximum
        /// intersection count is selected.
        /// </summary>
        /// <param name="sentence">Sentence to be annotated.</param>
        /// <returns>True, if at least one word is semantically annotated, false otherwise.</returns>
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