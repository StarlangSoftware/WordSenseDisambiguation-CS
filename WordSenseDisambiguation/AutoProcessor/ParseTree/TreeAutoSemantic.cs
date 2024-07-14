using System.Collections.Generic;
using AnnotatedTree;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public abstract class TreeAutoSemantic
    {
        protected abstract bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree);

        /// <summary>
        /// The method constructs all possible senses for the word at position index in the given parse tree. The method checks
        /// the previous two words and the current word; the previous, current and next word, current and the next
        /// two words to add three word multiword sense (that occurs in the Turkish wordnet) to the result list. The
        /// method then check the previous word and current word; current word and the next word to add a two word multiword
        /// sense to the result list. Lastly, the method adds all possible senses of the current word to the result list.
        /// </summary>
        /// <param name="wordNet">Turkish wordnet</param>
        /// <param name="fsm">Turkish morphological analyzer</param>
        /// <param name="leafList">Leaves of the parse tree to be semantically disambiguated.</param>
        /// <param name="index">Position of the word to be disambiguated.</param>
        /// <returns>All possible senses for the word at position index in the given parse tree.</returns>
        protected List<SynSet> GetCandidateSynSets(WordNet.WordNet wordNet, FsmMorphologicalAnalyzer fsm,
            List<ParseNodeDrawable> leafList, int index)
        {
            LayerInfo twoPrevious = null, previous = null, current, twoNext = null, next = null;
            var synSets = new List<SynSet>();
            current = leafList[index].GetLayerInfo();
            if (index > 1)
            {
                twoPrevious = leafList[index - 2].GetLayerInfo();
            }

            if (index > 0)
            {
                previous = leafList[index - 1].GetLayerInfo();
            }

            if (index != leafList.Count - 1)
            {
                next = leafList[index + 1].GetLayerInfo();
            }

            if (index < leafList.Count - 2)
            {
                twoNext = leafList[index + 2].GetLayerInfo();
            }

            synSets = wordNet.ConstructSynSets(current.GetMorphologicalParseAt(0).GetWord().GetName(),
                current.GetMorphologicalParseAt(0), current.GetMetamorphicParseAt(0), fsm);
            if (twoPrevious?.GetMorphologicalParseAt(0) != null && previous?.GetMorphologicalParseAt(0) != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(twoPrevious.GetMorphologicalParseAt(0),
                    previous.GetMorphologicalParseAt(0), current.GetMorphologicalParseAt(0),
                    twoPrevious.GetMetamorphicParseAt(0), previous.GetMetamorphicParseAt(0),
                    current.GetMetamorphicParseAt(0), fsm));
            }

            if (previous?.GetMorphologicalParseAt(0) != null && next?.GetMorphologicalParseAt(0) != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(previous.GetMorphologicalParseAt(0),
                    current.GetMorphologicalParseAt(0), next.GetMorphologicalParseAt(0),
                    previous.GetMetamorphicParseAt(0), current.GetMetamorphicParseAt(0),
                    next.GetMetamorphicParseAt(0), fsm));
            }

            if (next?.GetMorphologicalParseAt(0) != null && twoNext?.GetMorphologicalParseAt(0) != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(current.GetMorphologicalParseAt(0),
                    next.GetMorphologicalParseAt(0), twoNext.GetMorphologicalParseAt(0),
                    current.GetMetamorphicParseAt(0), next.GetMetamorphicParseAt(0),
                    twoNext.GetMetamorphicParseAt(0), fsm));
            }

            if (previous?.GetMorphologicalParseAt(0) != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(previous.GetMorphologicalParseAt(0),
                    current.GetMorphologicalParseAt(0),
                    previous.GetMetamorphicParseAt(0), current.GetMetamorphicParseAt(0), fsm));
            }

            if (next?.GetMorphologicalParseAt(0) != null)
            {
                synSets.AddRange(wordNet.ConstructIdiomSynSets(current.GetMorphologicalParseAt(0),
                    next.GetMorphologicalParseAt(0),
                    current.GetMetamorphicParseAt(0), next.GetMetamorphicParseAt(0), fsm));
            }

            return synSets;
        }

        /// <summary>
        /// The method tries to semantic annotate as many words in the parse tree as possible.
        /// </summary>
        /// <param name="parseTree">Parse tree to be semantically disambiguated.</param>
        public void AutoSemantic(ParseTreeDrawable parseTree)
        {
            if (AutoLabelSingleSemantics(parseTree))
            {
                parseTree.Save();
            }
        }
    }
}