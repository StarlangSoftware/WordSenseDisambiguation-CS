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
        
        /**
        * <summary>Checks
        * 1. the previous two words and the current word; the previous, current and next word, current and the next
        * two words for a three word multiword expression that occurs in the Turkish wordnet.
        * 2. the previous word and current word; current word and the next word for a two word multiword expression that
        * occurs in the Turkish wordnet.
        * 3. the current word
        * and sets the most frequent sense for that multiword expression or word.</summary>
        * <param name="sentence"> The sentence for which word sense disambiguation will be determined automatically.</param>
        */
        protected override bool AutoLabelSingleSemantics(AnnotatedSentence.AnnotatedSentence sentence)
        {
            var done = false;
            AnnotatedWord twoPrevious = null, previous = null, current, twoNext = null, next = null;
            for (var i = 0; i < sentence.WordCount(); i++)
            {
                current = (AnnotatedWord) sentence.GetWord(i);
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
                    if (twoPrevious != null && twoPrevious.GetParse() != null && previous.GetParse() != null) 
                    {
                        var literals = _turkishWordNet.ConstructIdiomLiterals(twoPrevious.GetParse(), previous.GetParse(), current.GetParse(), twoPrevious.GetMetamorphicParse(), previous.GetMetamorphicParse(), current.GetMetamorphicParse(), _fsm);
                        if (literals.Count > 0) 
                        {
                            var bestSynset = MostFrequent(literals);
                            if (bestSynset != null)
                            {
                                current.SetSemantic(bestSynset.GetId());
                                done = true;
                                continue;
                            }
                        }
                    }
                    if (previous != null && previous.GetParse() != null && next != null && next.GetParse() != null) 
                    {
                        var literals = _turkishWordNet.ConstructIdiomLiterals(previous.GetParse(), current.GetParse(), next.GetParse(), previous.GetMetamorphicParse(), current.GetMetamorphicParse(), next.GetMetamorphicParse(), _fsm);
                        if (literals.Count > 0) 
                        {
                            var bestSynset = MostFrequent(literals);
                            if (bestSynset != null) 
                            {
                                current.SetSemantic(bestSynset.GetId());
                                done = true;
                                continue;
                            }
                        }
                    }
                    if (next != null && next.GetParse() != null && twoNext != null && twoNext.GetParse() != null) 
                    {
                        var literals = _turkishWordNet.ConstructIdiomLiterals(current.GetParse(), next.GetParse(), twoNext.GetParse(), current.GetMetamorphicParse(), next.GetMetamorphicParse(), twoNext.GetMetamorphicParse(), _fsm);
                        if (literals.Count > 0) 
                        {
                            var bestSynset = MostFrequent(literals);
                            if (bestSynset != null) 
                            {
                                current.SetSemantic(bestSynset.GetId());
                                done = true;
                                continue;
                            }
                        }
                    }
                    if (previous != null && previous.GetParse() != null) 
                    {
                        var literals = _turkishWordNet.ConstructIdiomLiterals(previous.GetParse(), current.GetParse(), previous.GetMetamorphicParse(), current.GetMetamorphicParse(), _fsm);
                        if (literals.Count > 0) 
                        {
                            var bestSynset = MostFrequent(literals);
                            if (bestSynset != null) 
                            {
                                current.SetSemantic(bestSynset.GetId());
                                done = true;
                                continue;
                            }
                        }
                    }
                    if (current.GetSemantic() == null && next != null && next.GetParse() != null) 
                    {
                        var literals = _turkishWordNet.ConstructIdiomLiterals(current.GetParse(), next.GetParse(), current.GetMetamorphicParse(), next.GetMetamorphicParse(), _fsm);
                        if (literals.Count > 0) 
                        {
                            var bestSynset = MostFrequent(literals);
                            if (bestSynset != null) 
                            {
                                current.SetSemantic(bestSynset.GetId());
                                done = true;
                                continue;
                            }
                        }
                    }
                    var singleLiterals = _turkishWordNet.ConstructLiterals(current.GetParse().GetWord().GetName(), current.GetParse(), current.GetMetamorphicParse(), _fsm);
                    if (current.GetSemantic() == null && singleLiterals.Count > 0) 
                    {
                        var bestSynset = MostFrequent(singleLiterals);
                        if (bestSynset != null) 
                        {
                            current.SetSemantic(bestSynset.GetId());
                            done = true;
                        }
                    }
                }
            }
            return done;
        }
        
        /**
        <summary>Determines the SynSet containing the literal with the lowest sense number.</summary>
        <param name="literals">an ArrayList of Literal objects.</param>
        <returns>the SynSet containing the literal with the lowest sense number, or null if the input list is empty.</returns>
        */
        private SynSet MostFrequent(List<Literal> literals) {
            if (literals.Count == 1) {
                return _turkishWordNet.GetSynSetWithId(literals[0].GetSynSetId());
            }
            int minSense = int.MaxValue;
            SynSet bestSynset = null;
            foreach (var literal in literals)  {
                if(literal.GetSense() < minSense) {
                    minSense = literal.GetSense();
                    bestSynset = _turkishWordNet.GetSynSetWithId(literal.GetSynSetId());
                }
            }
            return bestSynset;
        }
        
    }
}