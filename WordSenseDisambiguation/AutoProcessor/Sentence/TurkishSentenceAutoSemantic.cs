using AnnotatedSentence;
using MorphologicalAnalysis;

namespace WordSenseDisambiguation.AutoProcessor.Sentence
{
    public class TurkishSentenceAutoSemantic : SentenceAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        /**
         * <summary> Constructor for the {@link TurkishSentenceAutoSemantic} class. Gets the Turkish wordnet and Turkish fst based
         * morphological analyzer from the user and sets the corresponding attributes.</summary>
         * <param name="turkishWordNet">Turkish wordnet</param>
         * <param name="fsm">Turkish morphological analyzer</param>
         */
        public TurkishSentenceAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        /**
         * <summary> The method checks
         * 1. the previous two words and the current word; the previous, current and next word, current and the next
         * two words for a three word multiword expression that occurs in the Turkish wordnet.
         * 2. the previous word and current word; current word and the next word for a two word multiword expression that
         * occurs in the Turkish wordnet.
         * 3. the current word
         * if it has only one sense. If there is only one sense for that multiword expression or word; it sets that sense.</summary>
         * <param name="sentence">The sentence for which word sense disambiguation will be determined automatically.</param>
         */
        protected override void AutoLabelSingleSemantics(AnnotatedSentence.AnnotatedSentence sentence)
        {
            AnnotatedWord twoPrevious = null, previous = null;
            AnnotatedWord twoNext = null, next = null;
            for (var i = 0; i < sentence.WordCount(); i++)
            {
                var current = (AnnotatedWord) sentence.GetWord(i);
                if (i > 1)
                {
                    twoPrevious = (AnnotatedWord) sentence.GetWord(i - 2);
                }

                if (i > 0)
                {
                    previous = (AnnotatedWord) sentence.GetWord(i - 1);
                }

                if (i != sentence.WordCount() - 1)
                {
                    next = (AnnotatedWord) sentence.GetWord(i + 1);
                }

                if (i < sentence.WordCount() - 2)
                {
                    twoNext = (AnnotatedWord) sentence.GetWord(i + 2);
                }

                if (current.GetSemantic() == null && current.GetParse() != null)
                {
                    if (previous != null && twoPrevious != null && twoPrevious.GetParse() != null &&
                        previous.GetParse() != null)
                    {
                        var idioms = _turkishWordNet.ConstructIdiomSynSets(twoPrevious.GetParse(),
                            previous.GetParse(), current.GetParse(), twoPrevious.GetMetamorphicParse(),
                            previous.GetMetamorphicParse(), current.GetMetamorphicParse(), _fsm);
                        if (idioms.Count == 1)
                        {
                            current.SetSemantic(idioms[0].GetId());
                            continue;
                        }
                    }

                    if (previous != null && previous.GetParse() != null && next != null && next.GetParse() != null)
                    {
                        var idioms = _turkishWordNet.ConstructIdiomSynSets(previous.GetParse(),
                            current.GetParse(), next.GetParse(), previous.GetMetamorphicParse(),
                            current.GetMetamorphicParse(), next.GetMetamorphicParse(), _fsm);
                        if (idioms.Count == 1)
                        {
                            current.SetSemantic(idioms[0].GetId());
                            continue;
                        }
                    }

                    if (next != null && next.GetParse() != null && twoNext != null && twoNext.GetParse() != null)
                    {
                        var idioms = _turkishWordNet.ConstructIdiomSynSets(current.GetParse(),
                            next.GetParse(), twoNext.GetParse(), current.GetMetamorphicParse(),
                            next.GetMetamorphicParse(), twoNext.GetMetamorphicParse(), _fsm);
                        if (idioms.Count == 1)
                        {
                            current.SetSemantic(idioms[0].GetId());
                            continue;
                        }
                    }

                    if (previous != null && previous.GetParse() != null)
                    {
                        var idioms = _turkishWordNet.ConstructIdiomSynSets(previous.GetParse(),
                            current.GetParse(), previous.GetMetamorphicParse(), current.GetMetamorphicParse(), _fsm);
                        if (idioms.Count == 1)
                        {
                            current.SetSemantic(idioms[0].GetId());
                            continue;
                        }
                    }

                    if (current.GetSemantic() == null && next != null && next.GetParse() != null)
                    {
                        var idioms = _turkishWordNet.ConstructIdiomSynSets(current.GetParse(),
                            next.GetParse(), current.GetMetamorphicParse(), next.GetMetamorphicParse(), _fsm);
                        if (idioms.Count == 1)
                        {
                            current.SetSemantic(idioms[0].GetId());
                            continue;
                        }
                    }

                    var meanings = _turkishWordNet.ConstructSynSets(current.GetParse().GetWord().GetName(),
                        current.GetParse(), current.GetMetamorphicParse(), _fsm);
                    if (current.GetSemantic() == null && meanings.Count == 1)
                    {
                        current.SetSemantic(meanings[0].GetId());
                    }
                }
            }
        }
    }
}