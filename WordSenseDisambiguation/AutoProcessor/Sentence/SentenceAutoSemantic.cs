using System.Collections.Generic;
using AnnotatedSentence;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.Sentence
{
    public abstract class SentenceAutoSemantic
    {
        /**
         * <summary> The method should set the senses of all words, for which there is only one possible sense.</summary>
         * <param name="sentence">The sentence for which word sense disambiguation will be determined automatically.</param>
         */
        protected abstract bool AutoLabelSingleSemantics(AnnotatedSentence.AnnotatedSentence sentence);

        /// <summary>
        /// The method constructs all possible senses for the word at position index in the given sentence. The method checks
        /// the previous two words and the current word; the previous, current and next word, current and the next
        /// two words to add three word multiword sense (that occurs in the Turkish wordnet) to the result list. The
        /// method then check the previous word and current word; current word and the next word to add a two word multiword
        /// sense to the result list. Lastly, the method adds all possible senses of the current word to the result list.
        /// </summary>
        /// <param name="wordNet">Turkish wordnet</param>
        /// <param name="fsm">Turkish morphological analyzer</param>
        /// <param name="sentence">Sentence to be semantically disambiguated.</param>
        /// <param name="index">Position of the word to be disambiguated.</param>
        /// <returns>All possible senses for the word at position index in the given sentence.</returns>
        protected List<SynSet> GetCandidateSynSets(WordNet.WordNet wordNet, FsmMorphologicalAnalyzer fsm,
            AnnotatedSentence.AnnotatedSentence sentence, int index)
        {
            AnnotatedWord twoPrevious = null, previous = null, current, twoNext = null, next = null;
            var synSets = new List<SynSet>();
            current = (AnnotatedWord) sentence.GetWord(index);
            if (index > 1)
            {
                twoPrevious = (AnnotatedWord) sentence.GetWord(index - 2);
            }

            if (index > 0)
            {
                previous = (AnnotatedWord) sentence.GetWord(index - 1);
            }

            if (index != sentence.WordCount() - 1)
            {
                next = (AnnotatedWord) sentence.GetWord(index + 1);
            }

            if (index < sentence.WordCount() - 2)
            {
                twoNext = (AnnotatedWord) sentence.GetWord(index + 2);
            }

            synSets = wordNet.ConstructSynSets(current.GetParse().GetWord().GetName(),
                current.GetParse(), current.GetMetamorphicParse(), fsm);
            if (twoPrevious?.GetParse() != null && previous?.GetParse() != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(twoPrevious.GetParse(), previous.GetParse(),
                    current.GetParse(),
                    twoPrevious.GetMetamorphicParse(), previous.GetMetamorphicParse(), current.GetMetamorphicParse(),
                    fsm));
            }

            if (previous?.GetParse() != null && next?.GetParse() != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(previous.GetParse(), current.GetParse(), next.GetParse(),
                    previous.GetMetamorphicParse(), current.GetMetamorphicParse(), next.GetMetamorphicParse(), fsm));
            }

            if (next?.GetParse() != null && twoNext?.GetParse() != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(current.GetParse(), next.GetParse(), twoNext.GetParse(),
                    current.GetMetamorphicParse(), next.GetMetamorphicParse(), twoNext.GetMetamorphicParse(), fsm));
            }

            if (previous?.GetParse() != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(previous.GetParse(), current.GetParse(),
                    previous.GetMetamorphicParse(), current.GetMetamorphicParse(), fsm));
            }

            if (next?.GetParse() != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(current.GetParse(), next.GetParse(),
                    current.GetMetamorphicParse(), next.GetMetamorphicParse(), fsm));
            }

            return synSets;
        }

        /// <summary>
        /// The method tries to semantic annotate as many words in the sentence as possible.
        /// </summary>
        /// <param name="sentence">Sentence to be semantically disambiguated.</param>
        public void AutoSemantic(AnnotatedSentence.AnnotatedSentence sentence)
        {
            if (AutoLabelSingleSemantics(sentence))
            {
                sentence.Save();
            }
        }
    }
}