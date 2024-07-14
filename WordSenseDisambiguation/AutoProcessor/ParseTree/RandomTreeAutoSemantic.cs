using System;
using System.Collections.Generic;
using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public class RandomTreeAutoSemantic : TreeAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        /// <summary>
        /// Constructor for the {@link RandomSentenceAutoSemantic} class. Gets the Turkish wordnet and Turkish fst based
        /// morphological analyzer from the user and sets the corresponding attributes.
        /// </summary>
        /// <param name="turkishWordNet">Turkish wordnet</param>
        /// <param name="fsm">Turkish morphological analyzer</param>
        public RandomTreeAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        /// <summary>
        /// The method annotates the word senses of the words in the parse tree randomly. The algorithm processes target
        /// words one by one. First, the algorithm constructs an array of all possible senses for the target word to
        /// annotate. Then it chooses a sense randomly.
        /// </summary>
        /// <param name="parseTree">Parse tree to be annotated.</param>
        /// <returns>True.</returns>
        protected override bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree)
        {
            var random = new Random(1);
            var nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            for (var i = 0; i < leafList.Count; i++){
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, leafList, i);
                if (synSets.Count > 0){
                    leafList[i].GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS, synSets[random.Next(synSets.Count)].GetId());
                }
            }
            return true;
        }
    }
}