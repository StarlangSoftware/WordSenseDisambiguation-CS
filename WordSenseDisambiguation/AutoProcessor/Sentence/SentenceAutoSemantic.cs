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

        public void AutoSemantic(AnnotatedSentence.AnnotatedSentence sentence)
        {
            if (AutoLabelSingleSemantics(sentence))
            {
                sentence.Save();
            }
        }
    }
}